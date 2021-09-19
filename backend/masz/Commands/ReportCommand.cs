using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Models;
using masz.Repositories;
using Microsoft.Extensions.Logging;

namespace masz.Commands
{

    public class ReportCommand : BaseCommand<ReportCommand>
    {
        public ReportCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Report to moderators")]
        public async Task Report(ContextMenuContext ctx)
        {
            await Require(ctx, RequireCheckEnum.GuildRegistered);
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(ctx.Guild.Id);

            DiscordInteractionResponseBuilder response =  new DiscordInteractionResponseBuilder();
            response.IsEphemeral = true;

            if (String.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent("This guild has no webhook for internal notifications configured."));
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"<@{ctx.User.Id}> reported a message from <@{ctx.TargetMessage.Author.Id}> in <#{ctx.TargetMessage.ChannelId}>.\n{ctx.TargetMessage.JumpLink}");
            sb.Append("\n");

            if (! String.IsNullOrEmpty(ctx.TargetMessage.Content))
            {
                sb.Append("```\n");
                sb.Append(ctx.TargetMessage.Content.Substring(0, Math.Min(ctx.TargetMessage.Content.Length, 1024)));
                sb.Append("\n``` ");
            }

            if (ctx.TargetMessage.Attachments.Count > 0)
            {
                sb.Append("Attachments:\n");
                foreach (DiscordAttachment attachment in ctx.TargetMessage.Attachments.Take(5))
                {
                    sb.Append($"- <{attachment.Url}>\n");
                }
                if (ctx.TargetMessage.Attachments.Count > 5)
                {
                    sb.Append($"and {ctx.TargetMessage.Attachments.Count - 5} more...\n");
                }
            }

            try
            {
                await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, sb.ToString());
            } catch (Exception e)
            {
                _logger.LogError(e, "Failed to send internal notification to moderators for report command.");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent("Failed to send report."));
                return;
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent("Report sent."));
        }
    }
}
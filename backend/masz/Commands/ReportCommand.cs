using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Extensions;
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
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent(_translator.T().CmdNoWebhookConfigured()));
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(_translator.T().CmdReportContent(ctx.User, ctx.TargetMessage));

            if (! String.IsNullOrEmpty(ctx.TargetMessage.Content))
            {
                sb.Append("```\n");
                sb.Append(ctx.TargetMessage.Content.Truncate(1024));
                sb.Append("\n``` ");
            }

            if (ctx.TargetMessage.Attachments.Count > 0)
            {
                sb.AppendLine(_translator.T().Attachments());
                foreach (DiscordAttachment attachment in ctx.TargetMessage.Attachments.Take(5))
                {
                    sb.Append($"- <{attachment.Url}>\n");
                }
                if (ctx.TargetMessage.Attachments.Count > 5)
                {
                    sb.AppendLine(_translator.T().AndXMore(ctx.TargetMessage.Attachments.Count - 5));
                }
            }

            try
            {
                await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, sb.ToString());
            } catch (Exception e)
            {
                _logger.LogError(e, "Failed to send internal notification to moderators for report command.");
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent(_translator.T().CmdReportFailed()));
                return;
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response.WithContent(_translator.T().CmdReportSent()));
        }
    }
}
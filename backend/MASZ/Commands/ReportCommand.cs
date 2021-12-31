using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using System.Text;

namespace MASZ.Commands
{

    public class ReportCommand : BaseCommand<ReportCommand>
    {
        [Require(RequireCheckEnum.GuildRegistered)]
        [MessageCommand("Report to moderators")]
        public async Task Report(IMessage msg)
        {
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);

            if (string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                await Context.Interaction.RespondAsync(Translator.T().CmdNoWebhookConfigured(), ephemeral: true);
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine(Translator.T().CmdReportContent(Context.User, msg, msg.Channel as ITextChannel));

            if (!string.IsNullOrEmpty(msg.Content))
            {
                sb.Append("```\n");
                sb.Append(msg.Content.Truncate(1024));
                sb.Append("\n``` ");
            }

            if (msg.Attachments.Count > 0)
            {
                sb.AppendLine(Translator.T().Attachments());
                foreach (IAttachment attachment in msg.Attachments.Take(5))
                {
                    sb.Append($"- <{attachment.Url}>\n");
                }
                if (msg.Attachments.Count > 5)
                {
                    sb.AppendLine(Translator.T().AndXMore(msg.Attachments.Count - 5));
                }
            }

            try
            {
                await DiscordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, sb.ToString(), AllowedMentions.None);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to send internal notification to moderators for report command.");
                await Context.Interaction.RespondAsync(Translator.T().CmdReportFailed(), ephemeral: true);
                return;
            }

            await Context.Interaction.RespondAsync(Translator.T().CmdReportSent(), ephemeral: true);
        }
    }
}
using Discord;
using Discord.Interactions;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Models.Database;
using MASZ.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MASZ.Commands
{

    public class ReportCommand : BaseCommand<ReportCommand>
    {
        //[Require(RequireCheckEnum.GuildRegistered)]
        //[MessageCommand("Report to moderators")]
        //public async Task Report(IMessage msg)
        //{
        //    GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);

        //    if (string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
        //    {
        //        await Context.Interaction.RespondAsync(Translator.T().CmdNoWebhookConfigured(), ephemeral: true);
        //        return;
        //    }

        //    StringBuilder sb = new();
        //    sb.AppendLine(Translator.T().CmdReportContent(Context.User, msg, msg.Channel as ITextChannel));

        //    if (!string.IsNullOrEmpty(msg.Content))
        //    {
        //        sb.Append("```\n");
        //        sb.Append(msg.Content.Truncate(1024));
        //        sb.Append("\n``` ");
        //    }

        //    if (msg.Attachments.Count > 0)
        //    {
        //        sb.AppendLine(Translator.T().Attachments());
        //        foreach (IAttachment attachment in msg.Attachments.Take(5))
        //        {
        //            sb.Append($"- <{attachment.Url}>\n");
        //        }
        //        if (msg.Attachments.Count > 5)
        //        {
        //            sb.AppendLine(Translator.T().AndXMore(msg.Attachments.Count - 5));
        //        }
        //    }

        //    try
        //    {
        //        await DiscordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, sb.ToString(), AllowedMentions.None);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.LogError(e, "Failed to send internal notification to moderators for report command.");
        //        await Context.Interaction.RespondAsync(Translator.T().CmdReportFailed(), ephemeral: true);
        //        return;
        //    }

        //    await Context.Interaction.RespondAsync(Translator.T().CmdReportSent(), ephemeral: true);
        //}

        [Require(RequireCheckEnum.GuildRegistered)]
        [MessageCommand("Report to moderators")]
        public async Task Report(IMessage message)
        {
            GuildConfig config = await GuildConfigRepository.CreateDefault(ServiceProvider).GetGuildConfig(Context.Guild.Id);

            try
            {
                string reporterNickname = (await DiscordAPI.FetchMemberInfo(Context.Guild.Id, Context.User.Id, CacheBehavior.IgnoreButCacheOnError)).Nickname;
                string reportedNickname = (await DiscordAPI.FetchMemberInfo(Context.Guild.Id, Context.User.Id, CacheBehavior.IgnoreButCacheOnError)).Nickname;
                string content = message.Content.Truncate(1024);
                // TODO
                //IEnumerable<IAttachment> attachments = message.Attachments.Take(5);
                //int attachmentCount = message.Attachments.Count;

                VerifiedEvidence evidence = new()
                {
                    GuildId = Context.Guild.Id,
                    MessageId = message.Id,
                    ReportedAt = DateTime.UtcNow,
                    SentAt = message.Timestamp.DateTime,
                    ReportedContent = content,
                    ReporterUserId = Context.User.Id,
                    ReporterUsername = Context.User.Username,
                    ReporterNickname = reporterNickname,
                    ReporterDiscriminator = Context.User.DiscriminatorValue,
                    ReportedUserId = message.Author.Id,
                    ReportedUsername = message.Author.Username,
                    ReportedNickname = reportedNickname,
                    ReportedDiscriminator = message.Author.DiscriminatorValue,
                };

                await VerifiedEvidenceRepository.CreateDefault(ServiceProvider).CreateEvidence(evidence);
            } catch (Exception e)
            {
                Logger.LogError(e, "Failed to save evidence in database");
                await Context.Interaction.RespondAsync(Translator.T().CmdReportFailed(), ephemeral: true);
                return;
            }

            if (!config.ModInternalNotificationWebhook.IsNullOrEmpty())
            {
                try
                {
                    await DiscordAPI.ExecuteWebhook(config.ModInternalNotificationWebhook, null, GetNotificationContent(ref message), AllowedMentions.None);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to send internal notification to moderators for report command.");
                }
            }

            await Context.Interaction.RespondAsync(Translator.T().CmdReportSent(), ephemeral: true);
        }

        private string GetNotificationContent(ref IMessage message)
        {
            StringBuilder sb = new();
            sb.AppendLine(Translator.T().CmdReportContent(Context.User, message, message.Channel as ITextChannel));

            if (!message.Content.IsNullOrEmpty()) 
            {
                sb.Append("```\n");
                sb.Append(message.Content.Truncate(1024));
                sb.Append("\n``` ");
            }

            if(message.Attachments.Count > 0)
            {
                sb.AppendLine(Translator.T().Attachments());
                foreach(var attachment in message.Attachments.Take(5)) 
                {
                    sb.Append($"- <{attachment.Url}>\n");
                }
                if (message.Attachments.Count > 5)
                {
                    sb.AppendLine(Translator.T().AndXMore(message.Attachments.Count - 5));
                }
            }

            return sb.ToString();
        }
    }
}
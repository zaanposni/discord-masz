using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Extensions;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class MessageUpdatedAuditLog
    {
        public static DiscordEmbedBuilder HandleMessageUpdated(DiscordClient client, MessageUpdateEventArgs e, ITranslator translator)
        {
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);

            StringBuilder description = new StringBuilder();
            description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {e.Channel.Name} - {e.Channel.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{e.Message.Id}]({e.Message.JumpLink})");
            description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {e.Author.Username}#{e.Author.Discriminator} - {e.Author.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogCreated()}:** {e.Message.CreationTimestamp.DateTime.ToDiscordTS()}");

            embed.WithTitle(translator.T().GuildAuditLogMessageUpdatedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(e.Author.Username, e.Author.AvatarUrl, e.Author.AvatarUrl)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {e.Author.Id}");

            if (e.MessageBefore == null)
            {
                embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentBefore(), translator.T().GuildAuditLogNotFoundInCache());
            } else
            {
                if (! string.IsNullOrEmpty(e.MessageBefore.Content))
                {
                    embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentBefore(), e.MessageBefore.Content.Truncate(1024));
                }
            }

            if (! string.IsNullOrEmpty(e.Message.Content))
            {
                embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentNew(), e.Message.Content.Truncate(1024));
            }

            if (e.Message.Attachments.Count > 0)
            {
                StringBuilder attachmentInfo = new StringBuilder();
                int counter = 1;
                foreach (DiscordAttachment attachment in e.Message.Attachments.Take(5))
                {
                    attachmentInfo.AppendLine($"- [{counter}. {translator.T().Attachment()}]({attachment.Url})");
                    counter++;
                }
                if (e.Message.Attachments.Count > 5)
                {
                    attachmentInfo.AppendLine(translator.T().AndXMore(e.Message.Attachments.Count - 5));
                }
                embed.AddField(translator.T().Attachments(), attachmentInfo.ToString());
            }

            return embed;
        }
    }
}
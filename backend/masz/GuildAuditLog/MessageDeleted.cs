using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Extensions;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class MessageDeletedAuditLog
    {
        public static DiscordEmbedBuilder HandleMessageDeleted(DiscordClient client, MessageDeleteEventArgs e, ITranslator translator)
        {
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Red);

            StringBuilder description = new StringBuilder();
            description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {e.Channel.Name} - {e.Channel.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{e.Message.Id}]({e.Message.JumpLink})");
            if (e.Message.Author != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {e.Message.Author.Username}#{e.Message.Author.Discriminator} - {e.Message.Author.Mention}");
                embed.WithAuthor(e.Message.Author.Username, e.Message.Author.AvatarUrl, e.Message.Author.AvatarUrl)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {e.Message.Author.Id}");
            }
            if (e.Message.CreationTimestamp != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogCreated()}:** {e.Message.CreationTimestamp.DateTime.ToDiscordTS()}");
            }

            embed.WithTitle(translator.T().GuildAuditLogMessageDeletedTitle())
                 .WithDescription(description.ToString());

            if (! string.IsNullOrEmpty(e.Message.Content))
            {
                embed.AddField(translator.T().GuildAuditLogMessageDeletedContent(), e.Message.Content);
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
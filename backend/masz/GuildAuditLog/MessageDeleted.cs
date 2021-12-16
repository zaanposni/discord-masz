using Discord;
using MASZ.Extensions;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class MessageDeletedAuditLog
    {
        public static EmbedBuilder HandleMessageDeleted(IMessage message, ITranslator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Red);

            if (message.Channel is ITextChannel tchannel)
            {
                StringBuilder description = new();
                description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {message.Channel.Name} - {tchannel.Mention}");
                description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{message.Id}]({message.GetJumpUrl()})");
                if (message.Author != null)
                {
                    description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {message.Author.Username}#{message.Author.Discriminator} - {message.Author.Mention}");
                    embed.WithAuthor(message.Author)
                         .WithFooter($"{translator.T().GuildAuditLogUserID()}: {message.Author.Id}");
                }
                if (message.CreatedAt != default)
                {
                    description.AppendLine($"> **{translator.T().GuildAuditLogCreated()}:** {message.CreatedAt.DateTime.ToDiscordTS()}");
                }

                embed.WithTitle(translator.T().GuildAuditLogMessageDeletedTitle())
                     .WithDescription(description.ToString());

                if (!string.IsNullOrEmpty(message.Content))
                {
                    embed.AddField(translator.T().GuildAuditLogMessageDeletedContent(), message.Content.Truncate(1024));
                }

                if (message.Attachments.Count > 0)
                {
                    StringBuilder attachmentInfo = new();
                    int counter = 1;
                    foreach (IAttachment attachment in message.Attachments.Take(5))
                    {
                        attachmentInfo.AppendLine($"- [{counter}. {translator.T().Attachment()}]({attachment.Url})");
                        counter++;
                    }
                    if (message.Attachments.Count > 5)
                    {
                        attachmentInfo.AppendLine(translator.T().AndXMore(message.Attachments.Count - 5));
                    }
                    embed.AddField(translator.T().Attachments(), attachmentInfo.ToString());
                }
            }
            return embed;
        }
    }
}
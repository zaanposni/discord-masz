using Discord;
using MASZ.Extensions;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class MessageSentAuditLog
    {
        public static EmbedBuilder HandleMessageSent(IMessage message, Translator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Green);

            if (message.Channel is ITextChannel tchannel)
            {
                StringBuilder description = new();
                description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {tchannel.Name} - {tchannel.Mention}");
                description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{message.Id}]({message.GetJumpUrl()})");
                description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {message.Author.Username}#{message.Author.Discriminator} - {message.Author.Mention}");

                embed.WithTitle(translator.T().GuildAuditLogMessageSentTitle())
                     .WithDescription(description.ToString())
                     .WithAuthor(message.Author)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {message.Author.Id}");

                if (!string.IsNullOrEmpty(message.Content))
                {
                    embed.AddField(translator.T().GuildAuditLogMessageSentContent(), message.Content.Truncate(1024));
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
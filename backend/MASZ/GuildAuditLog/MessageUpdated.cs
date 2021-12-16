using Discord;
using MASZ.Extensions;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class MessageUpdatedAuditLog
    {
        public static async Task<EmbedBuilder> HandleMessageUpdated(Cacheable<IMessage, ulong> messageBefore, IMessage messageAfter, Translator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Orange);

            if (messageAfter.Channel is ITextChannel tchannel)
            {
                StringBuilder description = new();
                description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {tchannel.Name} - {tchannel.Mention}");
                description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{messageAfter.Id}]({messageAfter.GetJumpUrl()})");
                description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {messageAfter.Author.Username}#{messageAfter.Author.Discriminator} - {messageAfter.Author.Mention}");
                description.AppendLine($"> **{translator.T().GuildAuditLogCreated()}:** {messageAfter.CreatedAt.DateTime.ToDiscordTS()}");

                embed.WithTitle(translator.T().GuildAuditLogMessageUpdatedTitle())
                     .WithDescription(description.ToString())
                     .WithAuthor(messageAfter.Author)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {messageAfter.Author.Id}");

                var before = await messageBefore.GetOrDownloadAsync();

                if (before == null)
                {
                    embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentBefore(), translator.T().GuildAuditLogNotFoundInCache());
                }
                else
                {
                    if (!string.IsNullOrEmpty(before.Content))
                    {
                        embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentBefore(), before.Content.Truncate(1024));
                    }
                }

                if (!string.IsNullOrEmpty(messageAfter.Content))
                {
                    embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentNew(), messageAfter.Content.Truncate(1024));
                }

                if (messageAfter.Attachments.Count > 0)
                {
                    StringBuilder attachmentInfo = new();
                    int counter = 1;
                    foreach (IAttachment attachment in messageAfter.Attachments.Take(5))
                    {
                        attachmentInfo.AppendLine($"- [{counter}. {translator.T().Attachment()}]({attachment.Url})");
                        counter++;
                    }
                    if (messageAfter.Attachments.Count > 5)
                    {
                        attachmentInfo.AppendLine(translator.T().AndXMore(messageAfter.Attachments.Count - 5));
                    }
                    embed.AddField(translator.T().Attachments(), attachmentInfo.ToString());
                }
            }

            return embed;
        }
    }
}
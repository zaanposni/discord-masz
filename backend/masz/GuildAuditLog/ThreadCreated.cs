using Discord;
using Discord.WebSocket;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class ThreadCreatedAuditLog
    {
        public static EmbedBuilder HandleThreadCreated(SocketThreadChannel thread, ITranslator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Green);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {thread.Name} - {thread.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedParent()}:** {thread.ParentChannel.Name} - {thread.ParentChannel.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedCreator()}:** <@{thread.Owner}>");

            embed.WithTitle(translator.T().GuildAuditLogThreadCreatedTitle())
                 .WithDescription(description.ToString())
                 .WithFooter($"{translator.T().GuildAuditLogChannelId()}: {thread.Id}");

            return embed;
        }
    }
}
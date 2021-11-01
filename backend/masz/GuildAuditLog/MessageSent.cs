using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace masz.GuildAuditLog
{
    public static class MessageSentAuditLog
    {
        public static DiscordEmbedBuilder HandleMessageSent(DiscordClient client, MessageCreateEventArgs e)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Red);
        }
    }
}
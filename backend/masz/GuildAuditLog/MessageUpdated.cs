using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace masz.GuildAuditLog
{
    public static class MessageUpdatedAuditLog
    {
        public static DiscordEmbedBuilder HandleMessageUpdated(DiscordClient client, MessageUpdateEventArgs e)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Red);
        }
    }
}
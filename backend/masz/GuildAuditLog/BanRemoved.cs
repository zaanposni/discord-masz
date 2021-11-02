using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class BanRemovedAuditLog
    {
        public static DiscordEmbedBuilder HandleBanRemoved(DiscordClient client, GuildBanRemoveEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
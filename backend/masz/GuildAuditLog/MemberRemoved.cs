using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class MemberRemovedAuditLog
    {
        public static DiscordEmbedBuilder HandleMemberRemovedUpdated(DiscordClient client, GuildMemberRemoveEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
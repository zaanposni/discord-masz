using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class MemberJoinedAuditLog
    {
        public static DiscordEmbedBuilder HandleMemberJoined(DiscordClient client, GuildMemberAddEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
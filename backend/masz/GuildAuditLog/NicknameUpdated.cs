using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class NicknameUpdatedAuditLog
    {
        public static DiscordEmbedBuilder HandleNicknameUpdated(DiscordClient client, GuildMemberUpdateEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
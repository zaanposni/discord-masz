using Discord;

namespace MASZ.GuildAuditLog
{
    public static class UsernameUpdatedAuditLog
    {
        public static EmbedBuilder HandleUsernameUpdated()
        {
            return GuildAuditLogger.GenerateBaseEmbed(Color.Orange);
        }
    }
}
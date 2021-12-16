using Discord;

namespace MASZ.GuildAuditLog
{
    public static class NicknameUpdatedAuditLog
    {
        public static EmbedBuilder HandleNicknameUpdated()
        {
            return GuildAuditLogger.GenerateBaseEmbed(Color.Orange);
        }
    }
}
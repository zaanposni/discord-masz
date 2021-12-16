using Discord;

namespace MASZ.GuildAuditLog
{
    public static class AvatarUpdatedAuditLog
    {
        public static EmbedBuilder HandleAvatarUpdated()
        {
            return GuildAuditLogger.GenerateBaseEmbed(Color.Orange);
        }
    }
}
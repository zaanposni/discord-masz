using Discord;

namespace MASZ.GuildAuditLog
{
    public static class MemberRolesUpdatedAuditLog
    {
        public static EmbedBuilder HandleMemberRolesUpdated()
        {
            return GuildAuditLogger.GenerateBaseEmbed(Color.Orange);
        }
    }
}
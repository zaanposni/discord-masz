using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class AvatarUpdatedAuditLog
    {
        public static DiscordEmbedBuilder HandleAvatarUpdated(DiscordClient client, GuildMemberUpdateEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
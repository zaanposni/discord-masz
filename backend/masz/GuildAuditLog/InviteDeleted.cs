using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class InviteDeletedAuditLog
    {
        public static DiscordEmbedBuilder HandleInviteDeleted(DiscordClient client, InviteDeleteEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
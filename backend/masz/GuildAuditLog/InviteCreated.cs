using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class InviteCreatedAuditLog
    {
        public static DiscordEmbedBuilder HandleInviteCreated(DiscordClient client, InviteCreateEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class ThreadCreatedAuditLog
    {
        public static DiscordEmbedBuilder HandleThreadCreated(DiscordClient client, ThreadCreateEventArgs e, ITranslator translator)
        {
            return GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Orange);
        }
    }
}
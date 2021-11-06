using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class BanAddedAuditLog
    {
        public static DiscordEmbedBuilder HandleBanAdded(DiscordClient client, GuildBanAddEventArgs e, ITranslator translator)
        {
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Red);

            StringBuilder description = new StringBuilder();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {e.Member.Username}#{e.Member.Discriminator} - {e.Member.Mention}");

            embed.WithTitle(translator.T().GuildAuditLogBanAddedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(e.Member.Username, e.Member.AvatarUrl, e.Member.AvatarUrl)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {e.Member.Id}");

            return embed;
        }
    }
}
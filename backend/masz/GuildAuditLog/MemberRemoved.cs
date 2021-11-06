using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Extensions;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class MemberRemovedAuditLog
    {
        public static DiscordEmbedBuilder HandleMemberRemovedUpdated(DiscordClient client, GuildMemberRemoveEventArgs e, ITranslator translator)
        {
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Red);

            StringBuilder description = new StringBuilder();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {e.Member.Username}#{e.Member.Discriminator} - {e.Member.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{e.Member.Id}`");
            description.AppendLine($"> **{translator.T().GuildAuditLogMemberJoinedRegistered()}:** {e.Member.CreationTimestamp.DateTime.ToDiscordTS()}");

            embed.WithTitle(translator.T().GuildAuditLogMemberRemovedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(e.Member.Username, e.Member.AvatarUrl, e.Member.AvatarUrl)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {e.Member.Id}");

            return embed;
        }
    }
}
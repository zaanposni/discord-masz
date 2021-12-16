using Discord;
using MASZ.Extensions;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class MemberRemovedAuditLog
    {
        public static EmbedBuilder HandleMemberRemovedUpdated(IUser user, Translator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Red);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{user.Id}`");
            description.AppendLine($"> **{translator.T().GuildAuditLogMemberJoinedRegistered()}:** {user.CreatedAt.DateTime.ToDiscordTS()}");

            embed.WithTitle(translator.T().GuildAuditLogMemberRemovedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(user)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            return embed;
        }
    }
}
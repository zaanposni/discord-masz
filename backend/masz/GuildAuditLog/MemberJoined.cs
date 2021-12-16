using Discord;
using MASZ.Extensions;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class MemberJoinedAuditLog
    {
        public static EmbedBuilder HandleMemberJoined(IUser user, ITranslator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Green);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{user.Id}`");
            description.AppendLine($"> **{translator.T().GuildAuditLogMemberJoinedRegistered()}:** {user.CreatedAt.DateTime.ToDiscordTS()}");

            embed.WithTitle(translator.T().GuildAuditLogMemberJoinedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(user)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            return embed;
        }
    }
}
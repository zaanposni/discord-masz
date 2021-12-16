using Discord;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class BanRemovedAuditLog
    {
        public static EmbedBuilder HandleBanRemoved(IUser user, Translator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Green);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");

            embed.WithTitle(translator.T().GuildAuditLogBanRemovedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(user)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            return embed;
        }
    }
}
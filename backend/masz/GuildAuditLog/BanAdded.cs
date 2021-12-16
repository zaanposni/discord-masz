using Discord;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class BanAddedAuditLog
    {
        public static EmbedBuilder HandleBanAdded(IUser user, ITranslator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Red);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");

            embed.WithTitle(translator.T().GuildAuditLogBanAddedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(user)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            return embed;
        }
    }
}
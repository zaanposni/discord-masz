using Discord;
using MASZ.InviteTracking;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class InviteDeletedAuditLog
    {
        public static async Task<EmbedBuilder> HandleInviteDeleted(TrackedInvite invite, IGuildChannel channel, ITranslator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Red);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedURL()}:** {invite}");

            var inviter = await channel.Guild.GetUserAsync(invite.CreatorId);

            if (inviter != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {inviter.Username}#{inviter.Discriminator} - {inviter.Mention}");
                embed.WithAuthor(inviter)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {inviter.Id}");
            }
            if (channel is ITextChannel tChannel)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedTargetChannel()}:** {tChannel.Name} - {tChannel.Mention}");
            }

            embed.WithTitle(translator.T().GuildAuditLogInviteDeletedTitle())
                 .WithDescription(description.ToString());

            return embed;
        }
    }
}
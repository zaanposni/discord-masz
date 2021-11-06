using System.Text;
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
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Red);

            StringBuilder description = new StringBuilder();
            description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedURL()}:** {e.Invite.ToString()}");
            if (e.Invite.Inviter != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {e.Invite.Inviter.Username}#{e.Invite.Inviter.Discriminator} - {e.Invite.Inviter.Mention}");
                embed.WithAuthor(e.Invite.Inviter.Username, e.Invite.Inviter.AvatarUrl, e.Invite.Inviter.AvatarUrl)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {e.Invite.Inviter.Id}");
            }
            if (e.Channel != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedTargetChannel()}:** {e.Channel.Name} - {e.Channel.Mention}");
            }

            embed.WithTitle(translator.T().GuildAuditLogInviteDeletedTitle())
                 .WithDescription(description.ToString());

            return embed;
        }
    }
}
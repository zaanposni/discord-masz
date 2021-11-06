using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Extensions;
using masz.Services;

namespace masz.GuildAuditLog
{
    public static class InviteCreatedAuditLog
    {
        public static DiscordEmbedBuilder HandleInviteCreated(DiscordClient client, InviteCreateEventArgs e, ITranslator translator)
        {
            DiscordEmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(DiscordColor.Green);

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

            embed.WithTitle(translator.T().GuildAuditLogInviteCreatedTitle())
                 .WithDescription(description.ToString());

            if (e.Invite.MaxUses != 0)
            {
                embed.AddField(translator.T().GuildAuditLogInviteCreatedMaxUses(), $"`{e.Invite.MaxUses}`", true);
            }

            if (e.Invite.ExpiresAt.HasValue)
            {
                embed.AddField(translator.T().GuildAuditLogInviteCreatedExpiration(), e.Invite.ExpiresAt.Value.DateTime.ToDiscordTS(), true);
            }

            return embed;
        }
    }
}
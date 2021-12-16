using Discord;
using MASZ.Extensions;
using MASZ.Services;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public static class InviteCreatedAuditLog
    {
        public static EmbedBuilder HandleInviteCreated(IInviteMetadata invite, Translator translator)
        {
            EmbedBuilder embed = GuildAuditLogger.GenerateBaseEmbed(Color.Green);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedURL()}:** {invite}");
            if (invite.Inviter != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {invite.Inviter.Username}#{invite.Inviter.Discriminator} - {invite.Inviter.Mention}");
                embed.WithAuthor(invite.Inviter)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {invite.Inviter.Id}");
            }
            if (invite.Channel is ITextChannel tChannel)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedTargetChannel()}:** {tChannel.Name} - {tChannel.Mention}");
            }

            embed.WithTitle(translator.T().GuildAuditLogInviteCreatedTitle())
                 .WithDescription(description.ToString());

            if (invite.MaxUses != 0)
            {
                embed.AddField(translator.T().GuildAuditLogInviteCreatedMaxUses(), $"`{invite.MaxUses}`", true);
            }

            if (invite.CreatedAt.HasValue && invite.MaxAge.HasValue)
            {
                embed.AddField(translator.T().GuildAuditLogInviteCreatedExpiration(), invite.CreatedAt.GetValueOrDefault().AddSeconds(invite.MaxAge.GetValueOrDefault()).DateTime.ToDiscordTS(), true);
            }

            return embed;
        }
    }
}
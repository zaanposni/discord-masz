using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
using masz.Extensions;
using masz.Models;
using masz.Repositories;

namespace masz.Commands
{

    public class TrackCommand : BaseCommand<TrackCommand>
    {
        public TrackCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [SlashCommand("track", "Track an invite, its creator and its users.")]
        public async Task Track(InteractionContext ctx,  [Option("invite", "Either enter the invite code or the url")] string inviteCode)
        {
            await Require(ctx, RequireCheckEnum.GuildModerator);
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (! inviteCode.ToLower().Contains("https://discord.gg/"))
            {
                inviteCode = $"https://discord.gg/{inviteCode}";
            }

            List<UserInvite> invites = await InviteRepository.CreateDefault(_serviceProvider).GetInvitesByCode(inviteCode);
            invites = invites.Where(x => x.GuildId == ctx.Guild.Id).OrderByDescending(x => x.JoinedAt).ToList();

            DateTime? createdAt = null;
            DiscordUser creator = null;
            int usages = invites.Count;
            Dictionary<ulong, DiscordUser> invitees = new Dictionary<ulong, DiscordUser>();

            if (invites.Count > 0)
            {
                createdAt = invites[0].InviteCreatedAt;
                if (invites[0].InviteIssuerId != 0)
                {
                    creator = await _discordAPI.FetchUserInfo(invites[0].InviteIssuerId, CacheBehavior.Default);
                }

                int count = 0;
                foreach (UserInvite invite in invites)
                {
                    if (count > 20)
                    {
                        break;
                    }
                    if (! invitees.ContainsKey(invite.JoinedUserId))
                    {
                        invitees.Add(invite.JoinedUserId, await _discordAPI.FetchUserInfo(invite.JoinedUserId, CacheBehavior.Default));
                    }
                }
            } else
            {
                string code = inviteCode.Split("/").Last();
                try
                {
                    DiscordInvite fetchedInvite = await ctx.Client.GetInviteByCodeAsync(code, true, false);
                    if (fetchedInvite.Guild.Id != ctx.Guild.Id)
                    {
                        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(_translator.T().CmdTrackInviteNotFromThisGuild()));
                        return;
                    }
                    try
                    {
                        usages = fetchedInvite.Uses;
                        creator = await _discordAPI.FetchUserInfo(fetchedInvite.Inviter.Id, CacheBehavior.Default);
                    } catch (NullReferenceException) { }  // vanity url
                } catch (DSharpPlus.Exceptions.NotFoundException)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(_translator.T().CmdTrackCannotFindInvite()));
                    return;
                } catch (Exception)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(_translator.T().CmdTrackFailedToFetchInvite()));
                    return;
                }
            }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithDescription(inviteCode);
            if (creator != null)
            {
                embed.WithAuthor($"{creator.Username}#{creator.Discriminator}", creator.AvatarUrl, creator.AvatarUrl);
                if (createdAt.HasValue && createdAt.Value != null)
                {
                    embed.WithDescription(_translator.T().CmdTrackCreatedByAt(inviteCode, creator, createdAt.Value));
                } else
                {
                    embed.WithDescription(_translator.T().CmdTrackCreatedBy(inviteCode, creator));
                }
            } else if (createdAt.HasValue && createdAt.Value != null)
            {
                embed.WithDescription(_translator.T().CmdTrackCreatedAt(inviteCode, createdAt.Value));
            }

            StringBuilder usedBy = new StringBuilder();
            foreach (UserInvite invite in invites)
            {
                if (usedBy.Length > 900)
                {
                    break;
                }

                usedBy.Append("- ");
                if (invitees.ContainsKey(invite.JoinedUserId))
                {
                    DiscordUser user = invitees[invite.JoinedUserId];
                    usedBy.Append($"`{user.Username}#{user.Discriminator}` ");
                }
                usedBy.AppendLine($"`{invite.JoinedUserId}` - {invite.JoinedAt.ToDiscordTS()}");
            }
            if (invites.Count == 0)
            {
                usedBy.Clear();
                usedBy.Append(_translator.T().CmdTrackNotTrackedYet());
            }

            embed.AddField(_translator.T().CmdTrackUsedBy(usages), usedBy.ToString(), false);
            embed.WithFooter($"Invite: {inviteCode}");
            embed.WithTimestamp(DateTime.UtcNow);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
    }
}
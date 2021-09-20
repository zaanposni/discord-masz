using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using masz.Enums;
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
                        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invite is not from this guild."));
                        return;
                    }
                    try
                    {
                        usages = fetchedInvite.Uses;
                        createdAt = fetchedInvite.CreatedAt.DateTime;
                        creator = await _discordAPI.FetchUserInfo(fetchedInvite.Inviter.Id, CacheBehavior.Default);
                    } catch (NullReferenceException) { }  // vanity url
                } catch (DSharpPlus.Exceptions.NotFoundException)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Could not find invite in database or in this guild."));
                    return;
                } catch (Exception)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to fetch invite."));
                    return;
                }
            }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.WithDescription(inviteCode);
            if (creator != null)
            {
                embed.WithAuthor($"{creator.Username}#{creator.Discriminator}", creator.AvatarUrl, creator.AvatarUrl);
                if (createdAt == null)
                {
                    embed.WithDescription($"{inviteCode} was created by {creator.Mention}.");
                } else
                {
                    string inviteCreatedAt = createdAt.Value.ToString("dd.MM.yyyy HH:mm:ss");
                    embed.WithDescription($"{inviteCode} was created by {creator.Mention} at `{inviteCreatedAt} (UTC)`.");
                }
            } else if (createdAt != null)
            {
                string inviteCreatedAt = createdAt.Value.ToString("dd.MM.yyyy HH:mm:ss");
                embed.WithDescription($"{inviteCode} was created at `{inviteCreatedAt} (UTC)`.");
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
                usedBy.Append($"`{invite.JoinedUserId}` ");
                string joinedAt = invite.JoinedAt.ToString("dd MMM yyyy HH:mm:ss");
                usedBy.Append($"- `{joinedAt} (UTC)`");
                usedBy.AppendLine();
            }
            if (invites.Count == 0)
            {
                usedBy.Clear();
                usedBy.Append("This invite has not been tracked by MASZ yet.");
            }

            embed.AddField($"Used by [{usages}]", usedBy.ToString(), false);
            embed.WithFooter($"Invite: {inviteCode}");
            embed.WithTimestamp(DateTime.UtcNow);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()));
        }
    }
}
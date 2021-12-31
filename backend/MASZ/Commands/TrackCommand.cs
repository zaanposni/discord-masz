using Discord;
using Discord.Interactions;
using Discord.Net;
using MASZ.Attributes;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using System.Net;
using System.Text;

namespace MASZ.Commands
{

    public class TrackCommand : BaseCommand<TrackCommand>
    {
        [Require(RequireCheckEnum.GuildModerator)]
        [SlashCommand("track", "Track an invite, its creator and its users.")]
        public async Task Track([Summary("invite", "Either enter the invite code or the url")] string inviteCode)
        {
            await Context.Interaction.RespondAsync("Tracking invite code...");

            if (!inviteCode.ToLower().Contains("https://discord.gg/"))
            {
                inviteCode = $"https://discord.gg/{inviteCode}";
            }

            List<UserInvite> invites = await InviteRepository.CreateDefault(ServiceProvider).GetInvitesByCode(inviteCode);
            invites = invites.Where(x => x.GuildId == Context.Guild.Id).OrderByDescending(x => x.JoinedAt).ToList();

            DateTime? createdAt = null;
            IUser creator = null;
            int? usages = invites.Count;
            Dictionary<ulong, IUser> invitees = new();

            if (invites.Count > 0)
            {
                createdAt = invites[0].InviteCreatedAt;
                if (invites[0].InviteIssuerId != 0)
                {
                    creator = await DiscordAPI.FetchUserInfo(invites[0].InviteIssuerId, CacheBehavior.Default);
                }

                int count = 0;
                foreach (UserInvite invite in invites)
                {
                    if (count > 20)
                    {
                        break;
                    }
                    if (!invitees.ContainsKey(invite.JoinedUserId))
                    {
                        invitees.Add(invite.JoinedUserId, await DiscordAPI.FetchUserInfo(invite.JoinedUserId, CacheBehavior.Default));
                    }
                }
            }
            else
            {
                string code = inviteCode.Split("/").Last();
                try
                {
                    var fetchedInvite = await Context.Client.GetInviteAsync(code);
                    if (fetchedInvite.GuildId != Context.Guild.Id)
                    {
                        await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdTrackInviteNotFromThisGuild());
                        return;
                    }
                    try
                    {
                        usages = fetchedInvite.Uses;
                        creator = await DiscordAPI.FetchUserInfo(fetchedInvite.Inviter.Id, CacheBehavior.Default);
                    }
                    catch (NullReferenceException) { }
                }
                catch (HttpException e)
                {
                    if (e.HttpCode == HttpStatusCode.NotFound)
                        await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdTrackCannotFindInvite());
                    else
                        await Context.Interaction.ModifyOriginalResponseAsync(message => message.Content = Translator.T().CmdTrackFailedToFetchInvite());
                    return;
                }
            }

            EmbedBuilder embed = new();
            embed.WithDescription(inviteCode);
            if (creator != null)
            {
                embed.WithAuthor(creator);
                if (createdAt.HasValue && createdAt.Value != default)
                {
                    embed.WithDescription(Translator.T().CmdTrackCreatedByAt(inviteCode, creator, createdAt.Value));
                }
                else
                {
                    embed.WithDescription(Translator.T().CmdTrackCreatedBy(inviteCode, creator));
                }
            }
            else if (createdAt.HasValue && createdAt.Value != default)
            {
                embed.WithDescription(Translator.T().CmdTrackCreatedAt(inviteCode, createdAt.Value));
            }

            StringBuilder usedBy = new();
            foreach (UserInvite invite in invites)
            {
                if (usedBy.Length > 900)
                {
                    break;
                }

                usedBy.Append("- ");
                if (invitees.ContainsKey(invite.JoinedUserId))
                {
                    IUser user = invitees[invite.JoinedUserId];
                    usedBy.Append($"`{user.Username}#{user.Discriminator}` ");
                }
                usedBy.AppendLine($"`{invite.JoinedUserId}` - {invite.JoinedAt.ToDiscordTS()}");
            }
            if (invites.Count == 0)
            {
                usedBy.Clear();
                usedBy.Append(Translator.T().CmdTrackNotTrackedYet());
            }

            embed.AddField(Translator.T().CmdTrackUsedBy(usages.GetValueOrDefault()), usedBy.ToString(), false);
            embed.WithFooter($"Invite: {inviteCode}");
            embed.WithTimestamp(DateTime.UtcNow);
            embed.WithColor(Color.Gold);

            await Context.Interaction.ModifyOriginalResponseAsync(message => { message.Content = ""; message.Embed = embed.Build(); });
        }
    }
}
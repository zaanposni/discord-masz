using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/network")]
    [Authorize]
    public class UserNetworkController : SimpleController
    {
        public UserNetworkController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserNetwork([FromQuery][Required] ulong userId)
        {
            Identity currentIdentity = await GetIdentity();
            List<string> modGuilds = new();
            List<DiscordGuildView> guildViews = new();

            List<GuildConfig> guildConfigs = await GuildConfigRepository.CreateDefault(_serviceProvider).GetAllGuildConfigs();
            if (guildConfigs.Count == 0)
            {
                throw new BaseAPIException("No guilds registered");
            }
            foreach (GuildConfig guildConfig in guildConfigs)
            {
                if (await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildConfig.GuildId))
                {
                    IGuild guild = _discordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.Default);
                    if (guild != null)
                    {
                        modGuilds.Add(guildConfig.GuildId.ToString());
                        guildViews.Add(new DiscordGuildView(guild));
                    }
                }
            }
            if (modGuilds.Count == 0)
            {
                return Unauthorized();
            }

            DiscordUserView searchedUser = DiscordUserView.CreateOrDefault(await _discordAPI.FetchUserInfo(userId, CacheBehavior.IgnoreButCacheOnError));

            // invites
            // ===============================================================================================
            InviteRepository inviteRepository = InviteRepository.CreateDefault(_serviceProvider);

            List<UserInviteExpandedView> invited = new();
            foreach (UserInvite invite in await inviteRepository.GetInvitedForUser(userId))
            {
                if (!modGuilds.Contains(invite.GuildId.ToString()))
                {
                    continue;
                }
                invited.Add(new UserInviteExpandedView(
                    invite,
                    await _discordAPI.FetchUserInfo(invite.JoinedUserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(invite.InviteIssuerId, CacheBehavior.OnlyCache)
                ));
            }

            List<UserInviteExpandedView> invitedBy = new();
            foreach (UserInvite invite in await inviteRepository.GetusedInvitesForUser(userId))
            {
                if (!modGuilds.Contains(invite.GuildId.ToString()))
                {
                    continue;
                }
                invitedBy.Add(new UserInviteExpandedView(
                    invite,
                    await _discordAPI.FetchUserInfo(invite.JoinedUserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(invite.InviteIssuerId, CacheBehavior.OnlyCache)
                ));
            }

            // mappings
            // ===============================================================================================
            UserMapRepository userMapRepository = UserMapRepository.CreateDefault(_serviceProvider, currentIdentity);
            List<UserMappingExpandedView> userMappings = new();
            foreach (UserMapping userMapping in await userMapRepository.GetUserMapsByUser(userId))
            {
                if (!modGuilds.Contains(userMapping.GuildId.ToString()))
                {
                    continue;
                }
                userMappings.Add(new UserMappingExpandedView(
                    userMapping,
                    await _discordAPI.FetchUserInfo(userMapping.UserA, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userMapping.UserB, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.OnlyCache)
                ));
            }

            ModCaseRepository modCaseRepository = ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity);
            AutoModerationEventRepository autoModerationEventRepository = AutoModerationEventRepository.CreateDefault(_serviceProvider);
            UserNoteRepository userNoteRepository = UserNoteRepository.CreateDefault(_serviceProvider, currentIdentity);

            List<CaseView> modCases = (await modCaseRepository.GetCasesForUser(userId)).Where(x => modGuilds.Contains(x.GuildId.ToString())).Select(x => new CaseView(x)).ToList();
            List<AutoModerationEventView> modEvents = (await autoModerationEventRepository.GetAllEventsForUser(userId)).Where(x => modGuilds.Contains(x.GuildId.ToString())).Select(x => new AutoModerationEventView(x)).ToList();
            List<UserNoteView> userNotes = (await userNoteRepository.GetUserNotesByUser(userId)).Where(x => modGuilds.Contains(x.GuildId.ToString())).Select(x => new UserNoteView(x)).ToList();

            return Ok(new
            {
                guilds = guildViews,
                user = searchedUser,
                invited,
                invitedBy,
                modCases,
                modEvents,
                userMappings,
                userNotes
            });
        }

        [HttpGet("invite")]
        public async Task<IActionResult> GetInviteNetwork([FromQuery][Required] string inviteUrl)
        {
            InviteRepository inviteRepository = InviteRepository.CreateDefault(_serviceProvider);

            List<UserInvite> invites = await inviteRepository.GetInvitesByCode(inviteUrl);
            if (invites == null || invites.Count == 0)
            {
                return NotFound();
            }

            await RequirePermission(invites[0].GuildId, DiscordPermission.Moderator);

            DiscordGuildView guild = new(_discordAPI.FetchGuildInfo(invites[0].GuildId, CacheBehavior.Default));

            List<UserInviteExpandedView> inviteViews = new();
            foreach (UserInvite invite in invites)
            {
                inviteViews.Add(new UserInviteExpandedView(
                    invite,
                    await _discordAPI.FetchUserInfo(invite.JoinedUserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(invite.InviteIssuerId, CacheBehavior.OnlyCache)
                ));
            }

            return Ok(new
            {
                invites = inviteViews,
                guild
            });
        }
    }
}
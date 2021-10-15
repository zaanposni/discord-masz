using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using masz.Models.Views;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/network")]
    [Authorize]
    public class UserNetworkController : SimpleController
    {
        private readonly ILogger<UserNetworkController> _logger;
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public UserNetworkController(ILogger<UserNetworkController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserNetwork([FromQuery][Required] ulong userId)
        {
            Identity currentIdentity = await GetIdentity();
            List<string> modGuilds = new List<string>();
            List<DiscordGuildView> guildViews = new List<DiscordGuildView>();

            List<GuildConfig> guildConfigs = await GuildConfigRepository.CreateDefault(_serviceProvider).GetAllGuildConfigs();
            if (guildConfigs.Count == 0)
            {
                throw new BaseAPIException("No guilds registered");
            }
            foreach (GuildConfig guildConfig in guildConfigs)
            {
                if (await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildConfig.GuildId))
                {
                    modGuilds.Add(guildConfig.GuildId.ToString());
                    guildViews.Add(new DiscordGuildView(await _discordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.Default)));
                }
            }
            if (modGuilds.Count == 0) {
                return Unauthorized();
            }

            DiscordUserView searchedUser = DiscordUserView.CreateOrDefault(await _discordAPI.FetchUserInfo(userId, CacheBehavior.IgnoreButCacheOnError));

            // invites
            // ===============================================================================================
            InviteRepository inviteRepository = InviteRepository.CreateDefault(_serviceProvider);

            List<UserInviteExpandedView> invited = new List<UserInviteExpandedView>();
            foreach(UserInvite invite in await inviteRepository.GetInvitedForUser(userId))
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

            List<UserInviteExpandedView> invitedBy = new List<UserInviteExpandedView>();
            foreach(UserInvite invite in await inviteRepository.GetusedInvitesForUser(userId))
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
            List<UserMappingExpandedView> userMappings = new List<UserMappingExpandedView>();
            foreach(UserMapping userMapping in await userMapRepository.GetUserMapsByUser(userId))
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

            return Ok(new {
                guilds = guildViews,
                user = searchedUser,
                invited = invited,
                invitedBy = invitedBy,
                modCases = modCases,
                modEvents = modEvents,
                userMappings =  userMappings,
                userNotes = userNotes
            });
        }

        [HttpGet("invite")]
        public async Task<IActionResult> GetInviteNetwork([FromQuery][Required] string inviteUrl)
        {
            Identity currentIdentity = await GetIdentity();
            InviteRepository inviteRepository = InviteRepository.CreateDefault(_serviceProvider);

            List<UserInvite> invites = await inviteRepository.GetInvitesByCode(inviteUrl);
            if (invites == null || invites.Count == 0)
            {
                return NotFound();
            }

            await RequirePermission(invites[0].GuildId, DiscordPermission.Moderator);

            DiscordGuildView guild = new DiscordGuildView(await _discordAPI.FetchGuildInfo(invites[0].GuildId, CacheBehavior.Default));

            List<UserInviteExpandedView> inviteViews = new List<UserInviteExpandedView>();
            foreach(UserInvite invite in invites)
            {
                inviteViews.Add(new UserInviteExpandedView(
                    invite,
                    await _discordAPI.FetchUserInfo(invite.JoinedUserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(invite.InviteIssuerId, CacheBehavior.OnlyCache)
                ));
            }

            return Ok(new {
                invites = inviteViews,
                guild = guild
            });
        }
    }
}
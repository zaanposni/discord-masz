using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
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
        private readonly ILogger<UserNetworkController> logger;
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public UserNetworkController(ILogger<UserNetworkController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetNetwork([FromRoute] string userid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            List<string> modGuilds = new List<string>();
            List<Guild> guildViews = new List<Guild>();

            List<GuildConfig> guildConfigs = await database.SelectAllGuildConfigs();
            if (guildConfigs.Count == 0) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 No guilds registered.");
                return BadRequest("No guilds registered.");
            }
            foreach (GuildConfig guildConfig in guildConfigs) {
                if (await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildConfig.GuildId)) {
                    modGuilds.Add(guildConfig.GuildId);
                    guildViews.Add(await discord.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.Default));
                }
            }
            if (modGuilds.Count == 0) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            User searchedUser = await discord.FetchUserInfo(userid, CacheBehavior.IgnoreButCacheOnError);
            List<UserInviteView> invited = new List<UserInviteView>();
            foreach(UserInvite invite in await database.GetInvitedUsersByUserId(userid)) 
            {
                if (!modGuilds.Contains(invite.GuildId)) {
                    continue;
                }
                invited.Add(new UserInviteView(
                    invite,
                    await discord.FetchUserInfo(invite.JoinedUserId, CacheBehavior.OnlyCache),
                    await discord.FetchUserInfo(invite.InviteIssuerId, CacheBehavior.OnlyCache)
                ));
            }
            List<UserInviteView> invitedBy = new List<UserInviteView>();
            foreach(UserInvite invite in await database.GetUsedInvitesByUserId(userid)) 
            {
                if (!modGuilds.Contains(invite.GuildId)) {
                    continue;
                }
                invitedBy.Add(new UserInviteView(
                    invite,
                    await discord.FetchUserInfo(invite.JoinedUserId, CacheBehavior.OnlyCache),
                    await discord.FetchUserInfo(invite.InviteIssuerId, CacheBehavior.OnlyCache)
                ));
            }
            List<UserMappingView> userMappings = new List<UserMappingView>();
            foreach(UserMapping userMapping in await database.GetUserMappingsByUserId(userid))
            {
                if (!modGuilds.Contains(userMapping.GuildId)) {
                    continue;
                }
                userMappings.Add(new UserMappingView() {
                    UserMapping = userMapping,
                    Moderator = await discord.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.OnlyCache),
                    UserA = await discord.FetchUserInfo(userMapping.UserA, CacheBehavior.OnlyCache),
                    UserB = await discord.FetchUserInfo(userMapping.UserB, CacheBehavior.OnlyCache)
                });
            }

            List<ModCase> modCases = (await database.SelectAllModCasesForSpecificUser(userid)).Where(x => modGuilds.Contains(x.GuildId)).ToList();
            List<AutoModerationEvent> modEvents = (await database.SelectAllModerationEventsForSpecificUser(userid)).Where(x => modGuilds.Contains(x.GuildId)).ToList();
            List<UserNote> userNotes = (await database.GetUserNotesByUserId(userid)).Where(x => modGuilds.Contains(x.GuildId)).ToList();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning network.");
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
    }
}
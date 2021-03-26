using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.UserAPIResponses;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers.api.v1
{
    [ApiController]
    [Authorize]
    [Route("api/v1/discord")]
    public class DiscordAPIController : ControllerBase
    {
        private readonly ILogger<DiscordAPIController> logger;
        private readonly IDiscordAPIInterface discord;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;

        public DiscordAPIController(ILogger<DiscordAPIController> logger, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discord, IDatabase database)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.identityManager = identityManager;
            this.database = database;
        }

        [HttpGet("users/@me")]
        public async Task<IActionResult> GetUser()
        {
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);

            User currentUser = await currentIdentity.GetCurrentDiscordUser();
             if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            List<Guild> memberGuilds = new List<Guild>();
            List<Guild> modGuilds = new List<Guild>();
            List<Guild> adminGuilds = new List<Guild>();
            List<Guild> bannedGuilds = new List<Guild>();
            bool siteAdmin = config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id);

            List<Guild> userGuilds = await currentIdentity.GetCurrentGuilds();

            List<GuildConfig> registeredGuilds = await database.SelectAllGuildConfigs();

            foreach (GuildConfig guild in registeredGuilds)
            {
                if (userGuilds == null) {
                    break;
                }
                var userGuild = userGuilds.FirstOrDefault(x => x.Id == guild.GuildId);
                if (userGuild != null)
                {
                    if (await currentIdentity.HasModRoleOrHigherOnGuild(guild.GuildId, this.database)) {
                        if (await currentIdentity.HasAdminRoleOnGuild(guild.GuildId, this.database)) {
                            adminGuilds.Add(userGuild);
                        } else {
                            modGuilds.Add(userGuild);
                        }
                    } else {
                        memberGuilds.Add(userGuild);
                    }
                } else {
                    if (await discord.GetGuildUserBan(guild.GuildId, currentUser.Id, CacheBehavior.Default) != null) {
                        bannedGuilds.Add(await discord.FetchGuildInfo(guild.GuildId, CacheBehavior.Default));
                    }
                }
            }

            return Ok(new APIUser(memberGuilds, bannedGuilds,  modGuilds, adminGuilds, currentUser, siteAdmin));
        }

        [HttpGet("users/{userid}")]
        public async Task<IActionResult> GetSpecificUser([FromRoute] string userid)
        {
            var user = await discord.FetchUserInfo(userid, CacheBehavior.Default);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}")]
        public async Task<IActionResult> GetSpecificGuild([FromRoute] string guildid)
        {
            var guild = await discord.FetchGuildInfo(guildid, CacheBehavior.Default);
            if (guild != null)
            {
                return Ok(guild);
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}/channels")]
        public async Task<IActionResult> GetAllGuildChannels([FromRoute] string guildid)
        {
            var channels = await discord.FetchGuildChannels(guildid, CacheBehavior.Default);
            if (channels != null)
            {
                return Ok(channels);
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}/members")]
        public async Task<IActionResult> GetGuildMembers([FromRoute] string guildid, [FromQuery] bool partial=false)
        {
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Endpoint only available for registered guilds.");
            }

            var members = await discord.FetchGuildMembers(guildid, CacheBehavior.Default);
            if (members != null)
            {
                if (partial) {
                    return Ok(members.Select(x => x.User).ToList());
                } else {
                    return Ok(members);
                }
                
            }
            return NotFound();
        }

        [HttpGet("guilds")]
        public async Task<IActionResult> GetAllGuilds()
        {
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);

            User currentUser = await currentIdentity.GetCurrentDiscordUser();
             if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            var guilds = await currentIdentity.GetCurrentGuilds();
            if (guilds != null)
            {
                return Ok(guilds);
            }
            return NotFound();
        }
    }
}
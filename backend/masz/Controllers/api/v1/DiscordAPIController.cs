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

            List<string> memberGuilds = new List<string>();
            List<string> modGuilds = new List<string>();
            List<string> bannedGuilds = new List<string>();
            bool siteAdmin = config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id);

            List<GuildConfig> registeredGuilds = await database.SelectAllGuildConfigs();
            foreach (GuildConfig guild in registeredGuilds)
            {
                if (await currentIdentity.IsOnGuild(guild.GuildId))
                {
                    if (await currentIdentity.HasModRoleOrHigherOnGuild(guild.GuildId, this.database)) {
                        modGuilds.Add(guild.GuildId);
                    } else {
                        memberGuilds.Add(guild.GuildId);
                    }
                } else {
                    if (await discord.GetGuildUserBan(guild.GuildId, currentUser.Id) != null) {
                        bannedGuilds.Add(guild.GuildId);
                    }
                }
            }

            return Ok(new APIUser(memberGuilds, bannedGuilds,  modGuilds, currentUser, siteAdmin));
        }

        [HttpGet("users/{userid}")]
        public async Task<IActionResult> GetSpecificUser([FromRoute] string userid)
        {
            var user = await discord.FetchUserInfo(userid);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}")]
        public async Task<IActionResult> GetSpecificGuild([FromRoute] string guildid)
        {
            var guild = await discord.FetchGuildInfo(guildid);
            if (guild != null)
            {
                return Ok(guild);
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
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

namespace masz.Controllers.api.v1
{
    [ApiController]
    [Authorize]
    [Route("api/v1/discord")]
    public class DiscordAPIController : ControllerBase
    {
        private readonly ILogger<DiscordAPIController> logger;
        private readonly IDiscordInterface discord;
        private readonly IDatabase database;
        private readonly IIdentityManager identityManager;

        public DiscordAPIController(ILogger<DiscordAPIController> logger, IIdentityManager identityManager, IDiscordInterface discord, IDatabase database)
        {
            this.logger = logger;
            this.discord = discord;
            this.identityManager = identityManager;
            this.database = database;
        }

        [HttpGet("users/@me")]
        public async Task<IActionResult> GetUser()
        {
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);

            User currentUser = await currentIdentity.GetCurrentDiscordUser();

            List<GuildConfig> registeredGuilds = await database.SelectAllGuildConfigs();
            List<string> intersectionGuilds = new List<string>();

            foreach (GuildConfig guild in registeredGuilds)
            {
                if (await currentIdentity.IsOnGuild(guild.GuildId))
                {
                    intersectionGuilds.Add(guild.GuildId);
                }
            }

            // TODO: fetch list of guilds the user is banned on

            return Ok(new APIUser(intersectionGuilds, currentUser));
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
    }
}
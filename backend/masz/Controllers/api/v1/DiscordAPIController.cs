using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.UserAPIResponses;
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
        private readonly IAuthRepository authRepo;
        private readonly DataContext dbContext;
        private readonly IDiscordRepository discordRepo;

        public DiscordAPIController(ILogger<DiscordAPIController> logger, IAuthRepository authRepo, DataContext context, IDiscordRepository discordRepo)
        {
            this.logger = logger;
            this.authRepo = authRepo;
            this.dbContext = context;
            this.discordRepo = discordRepo;
        }

        [HttpGet("users/@me")]
        public async Task<IActionResult> GetUser()
        {
            List<string> registeredGuilds = dbContext.GuildConfigs.AsQueryable().Select(x => x.GuildId).ToList();

            foreach (string guildId in registeredGuilds)
            {
                if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildId))
                {
                    registeredGuilds.Remove(guildId);
                }
            }

            User discordUser = await discordRepo.ValidateDiscordUserToken(await authRepo.GetDiscordUserToken(HttpContext));

            // fetch list of guilds the user is banned on

            return Ok(new APIUser(registeredGuilds, discordUser));
        }

        [HttpGet("users/{userid}")]
        public async Task<IActionResult> GetSpecificUser([FromRoute] string userid)
        {
            var user = await discordRepo.FetchDiscordUserInfo(userid);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [HttpGet("guilds/{guildid}")]
        public async Task<IActionResult> GetSpecificGuild([FromRoute] string guildid)
        {
            var guild = await discordRepo.FetchDiscordGuildInfo(guildid);
            if (guild != null)
            {
                return Ok(guild);
            }
            return NotFound();
        }
    }
}
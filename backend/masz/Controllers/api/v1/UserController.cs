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
    [Route("api/v1")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IAuthRepository authRepo;
        private readonly DataContext dbContext;
        private readonly IDiscordRepository discordRepo;

        public UserController(ILogger<UserController> logger, IAuthRepository authRepo, DataContext context, IDiscordRepository discordRepo)
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
    }
}
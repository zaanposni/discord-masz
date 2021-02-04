using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/meta/")]
    [Authorize]
    public class AdminStatsController : ControllerBase
    {
        private readonly ILogger<AdminStatsController> logger;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;

        public AdminStatsController(ILogger<AdminStatsController> logger, IOptions<InternalConfig> config, IIdentityManager identityManager)
        {
            this.logger = logger;
            this.config = config;
            this.identityManager = identityManager;
        }

        [HttpGet("adminstats")]
        public async Task<IActionResult> Status() {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            String userOnly = String.Empty;
            if (!config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                return Unauthorized();
            }

            List<string> currentLogins = new List<string>();
            foreach (var login in this.identityManager.GetCurrentIdentities())
            {
                var user = await login.GetCurrentDiscordUser();
                currentLogins.Add($"{user.Username}#{user.Discriminator}");
            }

            return Ok(new { loginsInLast10Minutes = currentLogins });
        }
    }
}
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
    public class AdminStatsController : SimpleController
    {
        private readonly ILogger<AdminStatsController> logger;
        private readonly IIdentityManager identityManager;
        private readonly IScheduler scheduler;

        public AdminStatsController(IServiceProvider serviceProvider, ILogger<AdminStatsController> logger, IIdentityManager identityManager, IScheduler scheduler) : base(serviceProvider) {
            this.logger = logger;
            this.identityManager = identityManager;
            this.scheduler = scheduler;
        }

        [HttpGet("adminstats")]
        public async Task<IActionResult> Status() {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            List<string> currentLogins = new List<string>();
            foreach (var login in this.identityManager.GetCurrentIdentities())
            {
                if (login is DiscordIdentity) 
                {
                    var user = await login.GetCurrentDiscordUser();
                    if (user == null) {
                        currentLogins.Add($"Invalid user.");
                    } else {
                        currentLogins.Add($"{user.Username}#{user.Discriminator}");
                    }
                }
            }

            var cache = this.discord.GetCache();

            return Ok(new {
                loginsInLast15Minutes = currentLogins,
                trackedInvites = await database.CountTrackedInvites(),
                modCases = await database.CountAllModCases(),
                guilds = await database.CountAllGuildConfigs(),
                automodEvents = await database.CountAllModerationEvents(),
                userNotes = await database.CountUserNotes(),
                userMappings = await database.CountUserMappings(),
                apiTokens = await database.CountAllAPITokens(),
                nextCache = this.scheduler.GetNextCacheSchedule(),
                cachedDataFromDiscord = cache.Keys
            });
        }

        [HttpPost("cache")]
        public async Task<IActionResult> TriggerCache() {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            Task task = new Task(() => {
                this.cacher.CacheAll();
                this.identityManager.ClearAllIdentities();
            });
            task.Start();

            return Ok();
        }
    }
}
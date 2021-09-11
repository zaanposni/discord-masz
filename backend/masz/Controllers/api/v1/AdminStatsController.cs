using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using masz.Models;
using masz.Repositories;
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
        private readonly ILogger<AdminStatsController> _logger;

        public AdminStatsController(IServiceProvider serviceProvider, ILogger<AdminStatsController> logger) : base(serviceProvider) {
            _logger = logger;
        }

        [HttpGet("adminstats")]
        public async Task<IActionResult> Status()
        {
            Identity identity = await GetIdentity();
            if (! identity.IsSiteAdmin()) return Unauthorized();

            List<string> currentLogins = new List<string>();
            foreach (var login in _identityManager.GetCurrentIdentities())
            {
                if (login is DiscordOAuthIdentity)
                {
                    try
                    {
                        var user = login.GetCurrentUser();
                        if (user == null)
                        {
                            currentLogins.Add($"Invalid user.");
                        } else
                        {
                            currentLogins.Add($"{user.Username}#{user.Discriminator}");
                        }
                    } catch (Exception e)
                    {
                        _logger.LogError(e, "Error getting logged in user.");
                        currentLogins.Add($"Invalid user.");
                    }
                }
            }

            StatusRepository repo = StatusRepository.CreateDefault(_serviceProvider);

            StatusDetail botDetails = repo.GetBotStatus();
            StatusDetail dbDetails = await repo.GetDbStatus();
            StatusDetail cacheDetails = repo.GetCacheStatus();
            StatusDetail discordAPIDetails = await repo.GetDiscordAPIStatus();

            return Ok(new {
                botStatus = botDetails,
                dbStatus = dbDetails,
                cacheStatus = cacheDetails,
                discordStatus = discordAPIDetails,
                loginsInLast15Minutes = currentLogins,
                defaultLanguage = _config.GetDefaultLanguage(),
                trackedInvites = await _database.CountTrackedInvites(),
                modCases = await _database.CountAllModCases(),
                guilds = await _database.CountAllGuildConfigs(),
                automodEvents = await _database.CountAllModerationEvents(),
                userNotes = await _database.CountUserNotes(),
                userMappings = await _database.CountUserMappings(),
                apiTokens = await _database.CountAllAPITokens(),
                nextCache = _scheduler.GetNextCacheSchedule(),
                cachedDataFromDiscord = _discordAPI.GetCache().Keys
            });
        }

        [HttpPost("cache")]
        public async Task<IActionResult> TriggerCache() {
            Identity identity = await GetIdentity();
            if (! identity.IsSiteAdmin()) return Unauthorized();

            Task task = new Task(() => {
                _identityManager.ClearAllIdentities();
                _scheduler.CacheAll();
            });
            task.Start();

            return Ok();
        }
    }
}
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/meta/")]
    [Authorize]
    public class AdminStatsController : SimpleController
    {
        private readonly ILogger<AdminStatsController> _logger;

        public AdminStatsController(IServiceProvider serviceProvider, ILogger<AdminStatsController> logger) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("adminstats")]
        public async Task<IActionResult> Status()
        {
            Identity currentIdentity = await GetIdentity();
            if (!currentIdentity.IsSiteAdmin()) return Unauthorized();

            List<string> currentLogins = new();
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
                        }
                        else
                        {
                            currentLogins.Add($"{user.Username}#{user.Discriminator}");
                        }
                    }
                    catch (Exception e)
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

            return Ok(new
            {
                botStatus = botDetails,
                dbStatus = dbDetails,
                cacheStatus = cacheDetails,
                loginsInLast15Minutes = currentLogins,
                defaultLanguage = _config.GetDefaultLanguage(),
                trackedInvites = await InviteRepository.CreateDefault(_serviceProvider).CountInvites(),
                modCases = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).CountAllCases(),
                guilds = await GuildConfigRepository.CreateDefault(_serviceProvider).CountGuildConfigs(),
                automodEvents = await AutoModerationEventRepository.CreateDefault(_serviceProvider).CountEvents(),
                userNotes = await UserNoteRepository.CreateWithBotIdentity(_serviceProvider).CountUserNotes(),
                userMappings = await UserMapRepository.CreateWithBotIdentity(_serviceProvider).CountAllUserMaps(),
                apiTokens = await TokenRepository.CreateDefault(_serviceProvider).CountTokens(),
                scheduledMessages = await ScheduledMessageRepository.CreateWithBotIdentity(_serviceProvider).CountMessages(),
                appeals = await AppealRepository.CreateDefault(_serviceProvider).CountAppeals(),
                nextCache = _scheduler.GetNextCacheSchedule(),
                cachedDataFromDiscord = _discordAPI.GetCache().Keys
            });
        }

        [HttpPost("cache")]
        public async Task<IActionResult> TriggerCache()
        {
            Identity identity = await GetIdentity();
            if (!identity.IsSiteAdmin()) return Unauthorized();

            Task task = new(() =>
            {
                _identityManager.ClearAllIdentities();
                _scheduler.CacheAll();
            });
            task.Start();

            return Ok();
        }
    }
}
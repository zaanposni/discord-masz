using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/automoderations")]
    [Authorize]
    public class AutoModerationEventController : ControllerBase
    {
        private readonly ILogger<ModCaseController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAnnouncer discordAnnouncer;
        private readonly IDiscordAPIInterface discord;
        private readonly IFilesHandler filesHandler;
        private readonly IPunishmentHandler punishmentHandler;

        public AutoModerationEventController(ILogger<ModCaseController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IDiscordAnnouncer modCaseAnnouncer, IFilesHandler filesHandler, IPunishmentHandler punishmentHandler)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.discordAnnouncer = modCaseAnnouncer;
            this.discord = discordInterface;
            this.filesHandler = filesHandler;
            this.punishmentHandler = punishmentHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid, [FromQuery] int startPage = 0) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            String userOnly = String.Empty;
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                userOnly = currentUser.Id;
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            startPage = Math.Max(0, startPage);
            int pageSize = 50;
            List<AutoModerationEvent> events = new List<AutoModerationEvent>();
            int eventsCount = 0;
            if (String.IsNullOrEmpty(userOnly)) {
                events = await database.SelectAllModerationEventsForGuild(guildid, startPage, pageSize);
                eventsCount = await database.CountAllModerationEventsForGuild(guildid);
            }
            else {
                events = await database.SelectAllModerationEventsForSpecificUserOnGuild(guildid, currentUser.Id, startPage, pageSize);  
                eventsCount = await database.CountAllModerationEventsForSpecificUserOnGuild(guildid, currentUser.Id);
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning Events.");
            return Ok(new {
                events = events,
                count = eventsCount
            });
        }
    }
}
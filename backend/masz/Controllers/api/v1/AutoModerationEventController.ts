using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class AutoModerationEventController : SimpleController
    {
        private readonly ILogger<AutoModerationEventController> logger;

        public AutoModerationEventController(ILogger<AutoModerationEventController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage = 0) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            User currentUser = await this.IsValidUser();
            if (currentUser == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            string userOnly = String.Empty;
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                userOnly = currentUser.Id;
            }

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
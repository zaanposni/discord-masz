using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.AutoModerationConfig;
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
    [Route("api/v1/guilds/{guildid}/automoderationconfig/")]
    [Authorize]
    public class AutoModerationConfigController : SimpleController
    {
        private readonly ILogger<AutoModerationConfigController> logger;

        public AutoModerationConfigController(IServiceProvider serviceProvider, ILogger<AutoModerationConfigController> logger) : base(serviceProvider) {
            this.logger = logger;
        }

        [HttpPut]
        public async Task<IActionResult> SetItem([FromRoute] string guildid, [FromBody] AutoModerationConfigForPutDto dto) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            AutoModerationConfig currentConfig = await database.SelectModerationConfigForGuildAndType(guildid, dto.AutoModerationType);
            if (currentConfig == null) {
                currentConfig = new AutoModerationConfig();
            }
            if (! Enum.IsDefined(typeof(AutoModerationType), dto.AutoModerationType)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid moderation type.");
                return BadRequest("Invalid moderation type");
            }
            if (! Enum.IsDefined(typeof(AutoModerationAction), dto.AutoModerationAction)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid action type.");
                return BadRequest("Invalid action type");
            }

            currentConfig.AutoModerationType = dto.AutoModerationType;
            currentConfig.AutoModerationAction = dto.AutoModerationAction;
            currentConfig.GuildId = guildid;
            currentConfig.IgnoreChannels = dto.IgnoreChannels.Distinct().ToArray();
            currentConfig.IgnoreRoles = dto.IgnoreRoles.Distinct().ToArray();
            currentConfig.Limit = dto.Limit;
            currentConfig.TimeLimitMinutes = dto.TimeLimitMinutes;
            currentConfig.CustomWordFilter = dto.CustomWordFilter;
            currentConfig.PunishmentType = dto.PunishmentType;
            currentConfig.PunishmentDurationMinutes = dto.PunishmentDurationMinutes;
            currentConfig.SendPublicNotification = dto.SendPublicNotification;
            currentConfig.SendDmNotification = dto.SendDmNotification;

            database.PutModerationConfig(currentConfig);
            await database.SaveChangesAsync();

            return Ok(new { Id = currentConfig.Id, GuildId = currentConfig.GuildId, Type = currentConfig.AutoModerationType });
        }

        [HttpDelete("{type}")]
        public async Task<IActionResult> DeleteItem([FromRoute] string guildid, [FromRoute] string type) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            int x = 0;
            try {
                x = Int32.Parse(type);
            } catch(Exception e) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid type.", e);
                return BadRequest("Invalid type");
            }

            if (! Enum.IsDefined(typeof(AutoModerationType), x)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid type.");
                return BadRequest("Invalid type");
            }

            AutoModerationType eType = (AutoModerationType)x;
            AutoModerationConfig currentConfig = await database.SelectModerationConfigForGuildAndType(guildid, eType);

            if (currentConfig != null) {
                database.DeleteSpecificModerationConfig(currentConfig);
                await database.SaveChangesAsync();
                return Ok(new { id = currentConfig.Id });
            } else {
                return NotFound();
            }
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetItem([FromRoute] string guildid, [FromRoute] string type) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            int x = 0;
            try {
                x = Int32.Parse(type);
            } catch(Exception e) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid type.", e);
                return BadRequest("Invalid type");
            }

            if (! Enum.IsDefined(typeof(AutoModerationType), x)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid type.");
                return BadRequest("Invalid type");
            }

            AutoModerationType eType = (AutoModerationType)x;
            AutoModerationConfig currentConfig = await database.SelectModerationConfigForGuildAndType(guildid, eType);

            if (currentConfig != null) {
                return Ok(currentConfig);
            } else {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            List<AutoModerationConfig> currentConfigs = await database.SelectAllModerationConfigsForGuild(guildid);

            return Ok(currentConfigs);
        }
    }
}
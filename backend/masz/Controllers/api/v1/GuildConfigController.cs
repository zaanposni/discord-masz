using System;
using System.IO;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.GuildConfig;
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
    [Route("api/v1/guilds/")]
    [Authorize]
    public class GuildConfigController : SimpleController
    {
        private readonly ILogger<GuildConfigController> logger;

        public GuildConfigController(ILogger<GuildConfigController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet("{guildid}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning GuildConfig.");
            return Ok(guildConfig);
        }

        [HttpDelete("{guildid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromQuery] bool deleteData = false) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
            }
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (deleteData) {
                await database.DeleteAllModCasesForGuild(guildid);
                try {
                    filesHandler.DeleteDirectory(Path.Combine(config.Value.AbsolutePathToFileUpload, guildid));
                } catch (Exception e) {
                    logger.LogError(e, "Failed to delete files directory for guilds.");
                }
                await database.DeleteAllModerationConfigsForGuild(guildid);
                await database.DeleteAllModerationEventsForGuild(guildid);
                await database.DeleteAllTemplatesForGuild(guildid);
                await database.DeleteMotdForGuild(guildid);
                await database.DeleteInviteHistoryByGuild(guildid);
                await database.DeleteUserNoteByGuild(guildid);
                await database.DeleteUserMappingByGuild(guildid);
            }

            database.DeleteSpecificGuildConfig(guildConfig);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource deleted.");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] GuildConfigForCreateDto guildConfigForCreateDto) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig alreadyExists = await database.SelectSpecificGuildConfig(guildConfigForCreateDto.GuildId);
            if (alreadyExists != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild is already registered.");
                return BadRequest("Guild is already registered.");
            }
            if (! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            Guild guild = await discord.FetchGuildInfo(guildConfigForCreateDto.GuildId, CacheBehavior.IgnoreCache);
            if (guild == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not found.");
                return BadRequest("Guild not found.");
            }
            foreach (string role in guildConfigForCreateDto.modRoles)
            {
                if (guild.Roles.FindIndex(x => x.Id == role) < 0)
                {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Roles not found.");
                    return BadRequest($"Role {role} not found.");
                }
            }
            foreach (string role in guildConfigForCreateDto.adminRoles)
            {
                if (guild.Roles.FindIndex(x => x.Id == role) < 0)
                {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Roles not found.");
                    return BadRequest($"Role {role} not found.");
                }
            }
            foreach (string role in guildConfigForCreateDto.mutedRoles)
            {
                if (guild.Roles.FindIndex(x => x.Id == role) < 0)
                {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Roles not found.");
                    return BadRequest($"Role {role} not found.");
                }
            }
            
            GuildConfig guildConfig = new GuildConfig();
            guildConfig.GuildId = guildConfigForCreateDto.GuildId;
            guildConfig.ModRoles = guildConfigForCreateDto.modRoles;
            guildConfig.AdminRoles = guildConfigForCreateDto.adminRoles;
            guildConfig.ModNotificationDM = guildConfigForCreateDto.ModNotificationDM;
            guildConfig.MutedRoles = guildConfigForCreateDto.mutedRoles;
            guildConfig.ModPublicNotificationWebhook = guildConfigForCreateDto.ModPublicNotificationWebhook;
            guildConfig.ModInternalNotificationWebhook = guildConfigForCreateDto.ModInternalNotificationWebhook;

            await database.SaveGuildConfig(guildConfig);
            await database.SaveChangesAsync();

            Task task = new Task(() => {
                this.cacher.CacheAll();
            });
            task.Start();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 201 Resource created.");
            return StatusCode(201);
        }

        [HttpPut("{guildid}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] string guildid, [FromBody] GuildConfigForPutDto newValue) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_DEMO_MODE"))) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Not allowed in demo mode.");
                return BadRequest("This is not allowed in demo mode.");
            }
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            guildConfig.ModRoles = newValue.modRoles;
            guildConfig.AdminRoles = newValue.AdminRoles;
            guildConfig.MutedRoles = newValue.mutedRoles;
            guildConfig.ModNotificationDM = newValue.ModNotificationDM;
            guildConfig.ModInternalNotificationWebhook = newValue.ModInternalNotificationWebhook;
            guildConfig.ModPublicNotificationWebhook = newValue.ModPublicNotificationWebhook;

            database.UpdateGuildConfig(guildConfig);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok();
        }
    }
}
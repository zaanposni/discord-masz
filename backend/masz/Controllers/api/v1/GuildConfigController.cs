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
    public class GuildConfigController : ControllerBase
    {
        private readonly ILogger<GuildConfigController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAPIInterface discord;
        private readonly IFilesHandler filesHandler;

        public GuildConfigController(ILogger<GuildConfigController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IFilesHandler filesHandler)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.discord = discordInterface;
            this.filesHandler = filesHandler;
        }

        [HttpGet("{guildid}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning GuildConfig.");
            return Ok(guildConfig);
        }

        [HttpDelete("{guildid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromQuery] bool deleteData = false) 
        {
            // check if request is made by a site admin
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id)) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 User unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
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
            }

            database.DeleteSpecificGuildConfig(guildConfig);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource deleted.");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] GuildConfigForCreateDto guildConfigForCreateDto) 
        {
            // check if request is made by a site admin
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 User unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            GuildConfig alreadyExists = await database.SelectSpecificGuildConfig(guildConfigForCreateDto.GuildId);
            if (alreadyExists != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild is already registered.");
                return BadRequest("Guild is already registered.");
            }

            Guild guild = await discord.FetchGuildInfo(guildConfigForCreateDto.GuildId);
            if (guild == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not found.");
                return BadRequest("Guild not found.");
            }
            if (guild.Roles.FindIndex(x => x.Id == guildConfigForCreateDto.ModRoleId) < 0 || guild.Roles.FindIndex(x => x.Id == guildConfigForCreateDto.AdminRoleId) < 0)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Roles not found.");
                return BadRequest("Roles not found.");
            }          

            GuildConfig guildConfig = new GuildConfig();
            guildConfig.GuildId = guildConfigForCreateDto.GuildId;
            guildConfig.ModRoleId = guildConfigForCreateDto.ModRoleId;
            guildConfig.AdminRoleId = guildConfigForCreateDto.AdminRoleId;
            guildConfig.ModNotificationDM = guildConfigForCreateDto.ModNotificationDM;
            guildConfig.MutedRoleId = guildConfigForCreateDto.MutedRoleId;
            guildConfig.ModPublicNotificationWebhook = guildConfigForCreateDto.ModPublicNotificationWebhook;
            guildConfig.ModInternalNotificationWebhook = guildConfigForCreateDto.ModInternalNotificationWebhook;

            await database.SaveGuildConfig(guildConfig);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 201 Resource created.");
            return StatusCode(201);
        }

        [HttpPut("{guildid}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] string guildid, [FromBody] GuildConfigForPutDto newValue) 
        {
            // check if request is made by a site admin
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");

            // do not allow in demo mode
            if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_DEMO_MODE"))) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Not allowed in demo mode.");
                return BadRequest("This is not allowed in demo mode.");
            }

            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasAdminRoleOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 User unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            guildConfig.ModRoleId = newValue.ModRoleId;
            guildConfig.AdminRoleId = newValue.AdminRoleId;
            guildConfig.MutedRoleId = newValue.MutedRoleId;
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
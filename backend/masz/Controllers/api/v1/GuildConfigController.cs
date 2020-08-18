using System.Threading.Tasks;
using masz.data;
using masz.Dtos.GuildConfig;
using masz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/{guildid}/config")]
    [Authorize]
    public class GuildConfigController : ControllerBase
    {
        private readonly ILogger<GuildConfigController> logger;
        private readonly IAuthRepository repo;
        private readonly DataContext dbContext;
        private readonly IOptions<InternalConfig> config;

        public GuildConfigController(ILogger<GuildConfigController> logger, IAuthRepository repo, DataContext context, IOptions<InternalConfig> config)
        {
            this.logger = logger;
            this.repo = repo;
            this.dbContext = context;
            this.config = config;
        }

        private bool ValidatePathParameter(string variable) 
        {
            if (variable == null)
                return false;
            if (variable.Trim().Length == 0)
                return false;
            return true;
        }

        private async Task<bool> ValidateGuildId(string guildId) 
        {
            if (!ValidatePathParameter(guildId))
                return false;
            
            GuildConfig
             modGuild = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildId);
            return modGuild != null;
        }

        /// <summary>
        /// This method checks the incoming request for bad input and authorization
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <param name="guildId">guildId the user wants to access</param>
        /// <param name="modCaseId">the modcase the user wants to access</param>
        /// <returns>Returning IActionResult if validation failed, null if Validation was successful</returns>
        private async Task<IActionResult> ValidateRequestForGuild(HttpContext context, string guildId, string modGuildId)
        {
            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | Incoming request");
            if (!ValidatePathParameter(guildId) || !ValidatePathParameter(modGuildId))
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 400 Bad Request.");
                return BadRequest();
            }
            if (! await ValidateGuildId(guildId))
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 400 Guild is not registered.");
                return BadRequest("Guild is not registered.");
            }            

            if (! await repo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildId))
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 401 Unauthorized.");
                return Unauthorized("test");
            }
            return null;
        }

        /// <summary>
        /// This method checks the incoming request for bad input and authorization
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <param name="guildId">guildId the user wants to access</param>
        /// <returns>Returning IActionResult if validation failed, null if Validation was successful</returns>
        private async Task<IActionResult> ValidateRequestForGuild(HttpContext context, string guildId)
        {
            return await ValidateRequestForGuild(context, guildId, "dummy");
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid);
            if (validation != null)
                return validation;

            GuildConfig guildConfig = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 404 Resource not found.");
                return NotFound();
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Returning GuildConfig.");
            return Ok(guildConfig);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid);
            if (validation != null)
                return validation;

            // check if request is made by a site admin
            if (!config.Value.SiteAdminDiscordUserIds.Contains(await repo.GetDiscordUserId(HttpContext))) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 401 User unauthorized.");
                return Unauthorized();
            }

            GuildConfig guildConfig = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 404 Resource not found.");
                return NotFound();
            }

            dbContext.GuildConfigs.Remove(guildConfig);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Resource deleted.");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGuildConfig([FromRoute] string guildid, [FromBody] GuildConfigForCreateDto guildConfigForCreateDto) 
        {
            if (!ValidatePathParameter(guildid))
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 400 Input invalid.");
                return BadRequest();
            }
            
            // check if request is made by a site admin
            if (!config.Value.SiteAdminDiscordUserIds.Contains(await repo.GetDiscordUserId(HttpContext))) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 401 User unauthorized.");
                return Unauthorized();
            }

            GuildConfig alreadyExists = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (alreadyExists != null)
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 400 Guild is already registered.");
                return BadRequest("Guild is already registered.");
            }

            GuildConfig guildConfig = new GuildConfig();
            guildConfig.GuildId = guildid;
            guildConfig.ModRoleId = guildConfigForCreateDto.ModRoleId;
            guildConfig.AdminRoleId = guildConfigForCreateDto.AdminRoleId;
            guildConfig.ModNotificationWebhook = guildConfigForCreateDto.ModNotificationWebhook;
            
            await dbContext.GuildConfigs.AddAsync(guildConfig);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] string guildid, [FromBody] GuildConfigForPatchDto guildConfigForPatchDto) 
        {            
            var validation = await ValidateRequestForGuild(HttpContext, guildid);
            if (validation != null)
                return validation;
            
            // check if request is made by a site admin
            if (!config.Value.SiteAdminDiscordUserIds.Contains(await repo.GetDiscordUserId(HttpContext))) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 401 User unauthorized.");
                return Unauthorized();
            }

            GuildConfig oldGuildConfig = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (oldGuildConfig == null) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 404 ModCase not found.");
                return NotFound();
            }

            foreach (var oldProperty in oldGuildConfig.GetType().GetProperties())
            {
                foreach (var property in guildConfigForPatchDto.GetType().GetProperties())
                {
                    if (property.Name == oldProperty.Name && property.PropertyType == oldProperty.PropertyType)
                    {
                        if (property.GetValue(guildConfigForPatchDto) != null)
                            oldProperty.SetValue(oldGuildConfig, property.GetValue(guildConfigForPatchDto));
                    }
                }                
            }

            dbContext.Update(oldGuildConfig);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Resource updated.");
            return Ok();
        }
    }
}
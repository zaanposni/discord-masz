using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
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
    [Route("api/v1/configs/{guildid}")]
    [Authorize]
    public class GuildConfigController : ControllerBase
    {
        private readonly ILogger<GuildConfigController> logger;
        private readonly IAuthRepository authRepo;
        private readonly DataContext dbContext;
        private readonly IOptions<InternalConfig> config;
        private readonly IDiscordRepository discordRepo;

        public GuildConfigController(ILogger<GuildConfigController> logger, IAuthRepository authRepo, DataContext context, IOptions<InternalConfig> config, IDiscordRepository discordRepo)
        {
            this.logger = logger;
            this.authRepo = authRepo;
            this.dbContext = context;
            this.config = config;
            this.discordRepo = discordRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            GuildConfig guildConfig = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning GuildConfig.");
            return Ok(guildConfig);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid) 
        {
            // check if request is made by a site admin
            if (!config.Value.SiteAdminDiscordUserIds.Contains(await authRepo.GetDiscordUserId(HttpContext))) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 User unauthorized.");
                return Unauthorized();
            }

            GuildConfig guildConfig = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (guildConfig == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Resource not found.");
                return NotFound();
            }

            dbContext.GuildConfigs.Remove(guildConfig);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource deleted.");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] GuildConfigForCreateDto guildConfigForCreateDto) 
        {            
            // check if request is made by a site admin
            if (!config.Value.SiteAdminDiscordUserIds.Contains(await authRepo.GetDiscordUserId(HttpContext))) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 User unauthorized.");
                return Unauthorized();
            }

            GuildConfig alreadyExists = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (alreadyExists != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild is already registered.");
                return BadRequest("Guild is already registered.");
            }

            Guild guild = await discordRepo.FetchDiscordGuildInfo(guildid);
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
            guildConfig.GuildId = guildid;
            guildConfig.ModRoleId = guildConfigForCreateDto.ModRoleId;
            guildConfig.AdminRoleId = guildConfigForCreateDto.AdminRoleId;
            guildConfig.ModNotificationWebhook = guildConfigForCreateDto.ModNotificationWebhook;
            
            await dbContext.GuildConfigs.AddAsync(guildConfig);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 201 Resource created.");
            return StatusCode(201);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] string guildid, [FromBody] GuildConfigForPatchDto guildConfigForPatchDto) 
        {            
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            
            // check if request is made by a site admin
            if (!config.Value.SiteAdminDiscordUserIds.Contains(await authRepo.GetDiscordUserId(HttpContext))) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 User unauthorized.");
                return Unauthorized();
            }

            GuildConfig oldGuildConfig = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildid);
            if (oldGuildConfig == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
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

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok();
        }
    }
}
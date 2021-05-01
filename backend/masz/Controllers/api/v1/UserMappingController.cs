using System;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.UserMapping;
using masz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/usermap")]
    [Authorize]
    public class UserMappingController : SimpleController
    {
        private readonly ILogger<UserMappingController> logger;

        public UserMappingController(ILogger<UserMappingController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetUserMap([FromRoute] string guildid, [FromRoute] string userid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning list.");
            return Ok(await this.database.GetUserMappingsByUserIdAndGuildId(userid, guildid));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserMap([FromRoute] string guildid, [FromBody] UserMappingForCreateDto userMapDto)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            UserMapping userMapping = new UserMapping();
            userMapping.GuildId = guildid;
            userMapping.CreatedAt = DateTime.UtcNow;
            userMapping.CreatorUserId = (await this.IsValidUser()).Id;
            userMapping.UserA = userMapDto.UserA;
            userMapping.UserB = userMapDto.UserB;
            userMapping.Reason = userMapDto.Reason;
            
            await this.database.SaveUserMapping(userMapping);
            await this.database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 201 Ressource created.");
            return StatusCode(201, new { id = userMapping.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CreateUserMap([FromRoute] string guildid, [FromRoute] string id)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            UserMapping existing = await this.database.GetUserMappingById(id);
            if (existing == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }

            this.database.DeleteUserMapping(existing);
            await this.database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Ressource deleted.");
            return Ok(new { id = existing.Id });
        }
    }
}
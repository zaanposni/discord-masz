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

        [HttpGet]
        public async Task<IActionResult> GetUserMap([FromRoute] string guildid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning list.");
            return Ok(await this.database.GetUserMappingsByGuildId(guildid));
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

            if (await this.database.GetUserMappingByUserIdsAndGuildId(userMapDto.UserA, userMapDto.UserB, guildid) != null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Mapping already exists.");
                return BadRequest("Mapping already exists.");
            }

            User validUserA = await discord.FetchUserInfo(userMapDto.UserA, CacheBehavior.Default);
            User validUserB = await discord.FetchUserInfo(userMapDto.UserB, CacheBehavior.Default);
            if (validUserA == null || validUserB == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 User invalid.");
                return BadRequest("Invalid user.");
            }
            if (userMapDto.UserA == userMapDto.UserB) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Same users.");
                return BadRequest("Cannot be same user.");
            }

            UserMapping userMapping = new UserMapping();
            userMapping.GuildId = guildid;
            userMapping.CreatedAt = DateTime.UtcNow;
            userMapping.CreatorUserId = (await this.IsValidUser()).Id;
            userMapping.UserA = userMapDto.UserA;
            userMapping.UserB = userMapDto.UserB;
            userMapping.Reason = userMapDto.Reason.Trim();
            
            this.database.SaveUserMapping(userMapping);
            await this.database.SaveChangesAsync();

            await this.discordAnnouncer.AnnounceUserMapping(userMapping, await this.IsValidUser(), RestAction.Created);

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 201 Ressource created.");
            return StatusCode(201, userMapping);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateUserMap([FromRoute] string guildid, [FromRoute] string id, [FromBody] UserMappingForUpdateDto userMapDto)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            UserMapping existing = await this.database.GetUserMappingById(id);
            if (existing == null || existing.GuildId != guildid) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not found.");
                return NotFound();
            }

            existing.Reason = userMapDto.Reason.Trim();
            
            this.database.SaveUserMapping(existing);
            await this.database.SaveChangesAsync();
            
            await this.discordAnnouncer.AnnounceUserMapping(existing, await this.IsValidUser(), RestAction.Edited);

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Ressource updated.");
            return Ok(existing);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserMap([FromRoute] string guildid, [FromRoute] string id)
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
            
            await this.discordAnnouncer.AnnounceUserMapping(existing, await this.IsValidUser(), RestAction.Deleted);

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Ressource deleted.");
            return Ok(existing);
        }
    }
}
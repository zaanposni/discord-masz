using System;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.UserNote;
using masz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/usernote")]
    [Authorize]
    public class UserNoteController : SimpleController
    {
        private readonly ILogger<UserNoteController> logger;

        public UserNoteController(ILogger<UserNoteController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNote([FromRoute] string guildid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning list.");
            return Ok(await this.database.GetUserNotesByGuildId(guildid));
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetUserNote([FromRoute] string guildid, [FromRoute] string userid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            UserNote userNote = await this.database.GetUserNoteByUserIdAndGuildId(userid, guildid);
            if (userNote == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not found.");
                return NotFound();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning usernote.");
            return Ok(userNote);
        }

        [HttpPut]
        public async Task<IActionResult> CreateUserNote([FromRoute] string guildid, [FromBody] UserNoteForUpdateDto userNote)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            User validUser = await discord.FetchUserInfo(userNote.UserId, CacheBehavior.Default);
            if (validUser == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 User invalid.");
                return BadRequest("Invalid user.");
            }

            UserNote existing = await this.database.GetUserNoteByUserIdAndGuildId(userNote.UserId, guildid);
            if (existing == null) {
                existing = new UserNote();
            }
            existing.UpdatedAt = DateTime.UtcNow;
            existing.CreatorId = (await this.IsValidUser()).Id;
            existing.UserId = userNote.UserId;
            existing.GuildId = guildid;
            existing.Description = userNote.Description.Trim();

            this.database.SaveUserNote(existing);
            await this.database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 201 Ressource updated.");
            return StatusCode(201, new { id = existing.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserNote([FromRoute] string guildid, [FromRoute] string id)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            UserNote existing = await this.database.GetUserNoteById(id);
            if (existing == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Not Found.");
                return NotFound();
            }

            this.database.DeleteUserNote(existing);
            await this.database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Ressource deleted.");
            return Ok(new { id = existing.Id });
        }
    }
}
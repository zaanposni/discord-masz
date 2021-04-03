using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/motd")]
    [Authorize]
    public class GuildMotdController : SimpleController
    {
        private readonly ILogger<GuildMotdController> logger;

        public GuildMotdController(ILogger<GuildMotdController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMotd([FromRoute] string guildid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            GuildMotd motd = await this.database.GetMotdForGuild(guildid);
            if (motd == null) {
                motd = new GuildMotd() {
                    Id = 0,
                    UserId = this.config.Value.DiscordClientId,
                    Message = "Default message. An administrator of this guild can set a custom message.",
                    ShowMotd = false,
                    CreatedAt = DateTime.UtcNow,
                    GuildId = guildid
                };
            }

            User creator = await this.discord.FetchUserInfo(motd.UserId, CacheBehavior.Default);

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning motd.");
            return Ok(new { creator=creator, motd=motd });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMotd([FromRoute] string guildid, [FromBody] GuildMotd motd)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Admin, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(motd.Message)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Empty message.");
                return BadRequest("Please provide a message.");
            }

            User currentUser = await this.IsValidUser();
            GuildMotd current = await this.database.GetMotdForGuild(guildid);
            if (current == null) {
                current = new GuildMotd();
            }

            current.GuildId = guildid;
            current.UserId = currentUser.Id;
            current.Message = motd.Message;
            current.ShowMotd = motd.ShowMotd;
            current.CreatedAt = DateTime.UtcNow;

            this.database.SaveMotd(current);
            await this.database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Motd saved.");
            return Ok(new { id = current.Id });
        }
    }
}
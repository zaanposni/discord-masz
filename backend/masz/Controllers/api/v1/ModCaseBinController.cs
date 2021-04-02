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
    [Route("api/v1/guilds/{guildid}/bin")]
    [Authorize]
    public class ModCaseBinController : SimpleController
    {
        private readonly ILogger<ModCaseBinController> logger;

        public ModCaseBinController(ILogger<ModCaseBinController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpDelete("{caseid}")]
        public async Task<IActionResult> RestoreModCase([FromRoute] string guildid, [FromRoute] string caseid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            if (modCase.MarkedToDeleteAt == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case not marked to delete.");
                return BadRequest("Case not marked to delete.");
            }

            modCase.DeletedByUserId = null;
            modCase.MarkedToDeleteAt = null;

            this.database.UpdateModCase(modCase);
            await this.database.SaveChangesAsync();

            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }
    }
}
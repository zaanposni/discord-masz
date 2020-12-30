using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
using masz.Dtos.ModCaseComments;
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
    [Route("api/v1/modcases/{guildid}/{caseid}/comments")]
    [Authorize]
    public class ModCaseCommentsController : ControllerBase
    {
        private readonly ILogger<ModCaseCommentsController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAnnouncer discordAnnouncer;
        private readonly IDiscordAPIInterface discord;

        public ModCaseCommentsController(ILogger<ModCaseCommentsController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IDiscordAnnouncer modCaseAnnouncer)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.discordAnnouncer = modCaseAnnouncer;
            this.discord = discordInterface;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromRoute] string caseid, [FromBody] ModCaseCommentForCreateDto comment) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                if (modCase == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                    return Unauthorized();                    
                } else {
                    if (modCase.UserId != currentUser.Id) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                        return Unauthorized();
                    }
                }
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            // normal user can only comment if no comments are there yet or last comment was not by him.
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                if (modCase.Comments.Any())
                {
                    if (modCase.Comments.Last().UserId == currentUser.Id) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Already commented.");
                        return BadRequest("Already commented. Please wait for a response.");
                    }
                }
            }

            ModCaseComment commentToCreate = new ModCaseComment();
            commentToCreate.ModCase = modCase;
            commentToCreate.UserId = currentUser.Id;
            commentToCreate.Message = comment.Message.Trim();
            commentToCreate.CreatedAt = DateTime.UtcNow;

            await database.SaveModCaseComment(commentToCreate);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceComment(commentToCreate, currentUser, RestAction.Created);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce comment.");
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = commentToCreate.Id });
        }

        [HttpPut("{commentid}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] int commentid, [FromBody] ModCaseCommentForPutDto newValue)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                if (modCase == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                    return Unauthorized();                    
                } else {
                    if (modCase.UserId != currentUser.Id) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                        return Unauthorized();
                    }
                }
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            ModCaseComment comment = modCase.Comments.FirstOrDefault(x => x.Id == commentid);
            if (comment == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Comment not found.");
                return NotFound();
            }

            // only commentor or site admin should be able to edit comment
            if (comment.UserId != currentUser.Id && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            comment.Message = newValue.Message.Trim();

            database.UpdateModCaseComment(comment);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceComment(comment, currentUser, RestAction.Edited);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce comment.");
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok(new { id = comment.Id });
        }

        [HttpDelete("{commentid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] int commentid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, caseid);
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                if (modCase == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                    return Unauthorized();                    
                } else {
                    if (modCase.UserId != currentUser.Id) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                        return Unauthorized();
                    }
                }
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            ModCaseComment comment = modCase.Comments.FirstOrDefault(x => x.Id == commentid);
            if (comment == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Comment not found.");
                return NotFound();
            }

            if (comment.UserId != currentUser.Id && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id) && !await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            database.DeleteSpecificModCaseComment(comment);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceComment(comment, currentUser, RestAction.Deleted);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce comment.");
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource deleted.");
            return Ok(new { id = comment.Id });
        }
    }
}
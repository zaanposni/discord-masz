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
    public class ModCaseCommentsController : SimpleCaseController
    {
        private readonly ILogger<ModCaseCommentsController> logger;
        public ModCaseCommentsController(ILogger<ModCaseCommentsController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromRoute] string caseid, [FromBody] ModCaseCommentForCreateDto comment) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            User currentUser = await this.IsValidUser();
            ModCase modCase = await this.database.SelectSpecificModCase(guildid, caseid);
            if(! await this.IsAllowedTo(APIActionPermission.View, modCase)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!modCase.AllowComments) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Comments are locked.");
                return BadRequest("Comments are locked.");
            }
            if (modCase.MarkedToDeleteAt != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case is marked to be deleted.");
                return BadRequest("Case is marked to be deleted.");
            }

            // normal user can only comment if no comments are there yet or last comment was not by him.
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid))
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
            return StatusCode(201, commentToCreate);
        }

        [HttpPut("{commentid}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] int commentid, [FromBody] ModCaseCommentForPutDto newValue)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            User currentUser = await this.IsValidUser();
            ModCase modCase = await this.database.SelectSpecificModCase(guildid, caseid);
            if(! await this.IsAllowedTo(APIActionPermission.View, modCase)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!modCase.AllowComments) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Comments are locked.");
                return BadRequest("Comments are locked.");
            }
            if (modCase.MarkedToDeleteAt != null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Case is marked to be deleted.");
                return BadRequest("Case is marked to be deleted.");
            }

            ModCaseComment comment = modCase.Comments.FirstOrDefault(x => x.Id == commentid);
            if (comment == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Comment not found.");
                return NotFound();
            }
            // only commentor or site admin should be able to edit comment
            if (comment.UserId != currentUser.Id && ! await this.IsSiteAdmin())
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
            return Ok(comment);
        }

        [HttpDelete("{commentid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string caseid, [FromRoute] int commentid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }
            User currentUser = await this.IsValidUser();
            ModCase modCase = await this.database.SelectSpecificModCase(guildid, caseid);
            if(! await this.IsAllowedTo(APIActionPermission.View, modCase)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            ModCaseComment comment = modCase.Comments.FirstOrDefault(x => x.Id == commentid);
            if (comment == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 Comment not found.");
                return NotFound();
            }
            if (comment.UserId != currentUser.Id && ! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid))
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
            return Ok(comment);
        }
    }
}
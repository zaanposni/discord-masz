using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.ModCase;
using masz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/{guildid}/modcase")]
    [Authorize]
    public class ModCaseController : ControllerBase
    {
        private readonly ILogger<ModCaseController> logger;
        private readonly IAuthRepository repo;
        private readonly DataContext dbContext;

        public ModCaseController(ILogger<ModCaseController> logger, IAuthRepository repo, DataContext context)
        {
            this.logger = logger;
            this.repo = repo;
            this.dbContext = context;
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
            
            GuildConfig modGuild = await dbContext.GuildConfigs.FirstOrDefaultAsync(x => x.GuildId == guildId);
            return modGuild != null;
        }

        /// <summary>
        /// This method checks the incoming request for bad input and authorization
        /// </summary>
        /// <param name="context">current http context to check</param>
        /// <param name="guildId">guildId the user wants to access</param>
        /// <param name="modCaseId">the modcase the user wants to access</param>
        /// <returns>Returning IActionResult if validation failed, null if Validation was successful</returns>
        private async Task<IActionResult> ValidateRequestForGuild(HttpContext context, string guildId, string modCaseId)
        {
            logger.LogInformation(context.Request.Method + " " + context.Request.Path + " | Incoming request");
            if (!ValidatePathParameter(guildId) || !ValidatePathParameter(modCaseId))
            {
                logger.LogInformation(context.Request.Method + " " + context.Request.Path + " | 400 Bad Request.");
                return BadRequest();
            }
            if (! await ValidateGuildId(guildId))
            {
                logger.LogInformation(context.Request.Method + " " + context.Request.Path + " | 400 Guild is not registered.");
                return BadRequest("Guild is not registered.");
            }            

            if (! await repo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildId))
            {
                logger.LogInformation(context.Request.Method + " " + context.Request.Path + " | 401 Unauthorized.");
                return Unauthorized();
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

        [HttpGet("{modcaseid}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid, modcaseid);
            if (validation != null)
                return validation;

            ModCase modCase = await dbContext.ModCases.FirstOrDefaultAsync(x => x.Id.ToString() == modcaseid);
            if (modCase == null) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 404 ModCase not found.");
                return NotFound();
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Returning ModCase.");
            return Ok(modCase);
        }

        [HttpDelete("{modcaseid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid, modcaseid);
            if (validation != null)
                return validation;
            
            ModCase modCase = await dbContext.ModCases.FirstOrDefaultAsync(x => x.Id.ToString() == modcaseid);
            if (modCase == null) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 404 ModCase not found.");
                return NotFound();
            }

            modCase.Valid = false;

            dbContext.ModCases.Update(modCase);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{modcaseid}")]
        public async Task<IActionResult> PatchSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromBody] ModCaseForPatchDto modCase) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid, modcaseid);
            if (validation != null)
                return validation;
            
            ModCase oldModCase = await dbContext.ModCases.FirstOrDefaultAsync(x => x.Id.ToString() == modcaseid);
            if (oldModCase == null) 
            {
                logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 404 ModCase not found.");
                return NotFound();
            }

            foreach (var oldProperty in oldModCase.GetType().GetProperties())
            {
                foreach (var property in modCase.GetType().GetProperties())
                {
                    if (property.Name == oldProperty.Name && property.PropertyType == oldProperty.PropertyType)
                    {
                        if (property.GetValue(modCase) != null)
                            oldProperty.SetValue(oldModCase, property.GetValue(modCase));
                    }
                }                
            }

            oldModCase.LastEditedAt = DateTime.Now;
            oldModCase.LastEditedByModId = await repo.GetDiscordUserId(HttpContext);

            dbContext.Update(oldModCase);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Resource updated.");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewModCase([FromRoute] string guildid, [FromBody] ModCaseForCreateDto modCase) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid);
            if (validation != null)
                return validation;

            ModCase newModCase = new ModCase();
            newModCase.Title = modCase.Title;
            newModCase.Description = modCase.Description;
            newModCase.GuildId = guildid;
            newModCase.UserId = modCase.UserId;
            newModCase.CurrentUsername = modCase.CurrentUsername;
            newModCase.Severity = modCase.Severity;
            newModCase.CreatedAt = DateTime.Now;
            if (modCase.OccuredAt != null)
                newModCase.OccuredAt = modCase.OccuredAt;
            else
                newModCase.OccuredAt = DateTime.Now;
            newModCase.LastEditedAt = DateTime.Now;
            newModCase.LastEditedByModId = await repo.GetDiscordUserId(HttpContext);
            newModCase.Punishment = modCase.Punishment;
            newModCase.Labels = modCase.Labels;
            newModCase.Others = modCase.Others;
            newModCase.Valid = true;
            
            await dbContext.ModCases.AddAsync(newModCase);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllModCases([FromRoute] string guildid) 
        {
            var validation = await ValidateRequestForGuild(HttpContext, guildid);
            if (validation != null)
                return validation;
            
            List<ModCase> modCases = await dbContext.ModCases.ToListAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 200 Returning ModCases.");
            return Ok(modCases);
        }
    }
}
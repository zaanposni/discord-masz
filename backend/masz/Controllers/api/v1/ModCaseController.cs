using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAuthRepository authRepo;
        private readonly DataContext dbContext;
        private readonly IDiscordRepository discordRepo;

        public ModCaseController(ILogger<ModCaseController> logger, IAuthRepository authRepo, DataContext context, IDiscordRepository discordRepo)
        {
            this.logger = logger;
            this.authRepo = authRepo;
            this.dbContext = context;
            this.discordRepo = discordRepo;
        }

        [HttpGet("{modcaseid}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            ModCase modCase = await dbContext.ModCases.FirstOrDefaultAsync(x => x.GuildId == guildid && x.Id.ToString() == modcaseid);
            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCase.");
            return Ok(modCase);
        }

        [HttpDelete("{modcaseid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            
            ModCase modCase = await dbContext.ModCases.FirstOrDefaultAsync(x => x.GuildId == guildid && x.Id.ToString() == modcaseid);
            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
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
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            
            ModCase oldModCase = await dbContext.ModCases.FirstOrDefaultAsync(x => x.GuildId == guildid && x.Id.ToString() == modcaseid);
            if (oldModCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
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

            oldModCase.Valid = true;
            oldModCase.LastEditedAt = DateTime.Now;
            oldModCase.LastEditedByModId = await authRepo.GetDiscordUserId(HttpContext);

            dbContext.Update(oldModCase);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok(oldModCase.Id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] ModCaseForCreateDto modCase) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            var currentModUserId = await authRepo.GetDiscordUserId(HttpContext);
            if (currentModUserId == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Failed to fetch mod user info.");
                return BadRequest("Could not fetch own user info.");
            }

            ModCase newModCase = new ModCase();

            var currentReportedUser = await discordRepo.FetchDiscordMemberInfo(guildid, modCase.UserId);
            if (currentReportedUser != null)
            {
                newModCase.CurrentUsername = currentReportedUser.User.Username;
                newModCase.CurrentNickname = currentReportedUser.Nick;
            }

            newModCase.Title = modCase.Title;
            newModCase.Description = modCase.Description;
            newModCase.GuildId = guildid;
            newModCase.ModId = currentModUserId;
            newModCase.UserId = modCase.UserId;
            newModCase.Severity = modCase.Severity;
            newModCase.CreatedAt = DateTime.Now;
            if (modCase.OccuredAt.HasValue)
                newModCase.OccuredAt = modCase.OccuredAt.Value;
            else
                newModCase.OccuredAt = DateTime.Now;
            newModCase.LastEditedAt = DateTime.Now;
            newModCase.LastEditedByModId = currentModUserId;
            newModCase.Punishment = modCase.Punishment;
            newModCase.Labels = modCase.Labels;
            newModCase.Others = modCase.Others;
            newModCase.Valid = true;
            
            await dbContext.ModCases.AddAsync(newModCase);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = newModCase.Id });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid, [FromQuery] string limit = "100") 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await authRepo.DiscordUserHasModRoleOrHigherOnGuild(HttpContext, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            
            int iLimit = 0;

            if (!Int32.TryParse(limit, out iLimit))
                iLimit = 100;

            List<ModCase> modCases = await dbContext.ModCases.Where(x => x.GuildId == guildid).Take(iLimit).ToListAsync();            

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(modCases);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
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
    [Route("api/v1/modcases/{guildid}/")]
    [Authorize]
    public class ModCaseController : ControllerBase
    {
        private readonly ILogger<ModCaseController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IModCaseAnnouncer modCaseAnnouncer;
        private readonly IDiscordAPIInterface discord;

        public ModCaseController(ILogger<ModCaseController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IModCaseAnnouncer modCaseAnnouncer)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.modCaseAnnouncer = modCaseAnnouncer;
            this.discord = discordInterface;
        }

        [HttpGet("{modcaseid}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
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

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCase.");
            return Ok(modCase);
        }

        [HttpDelete("{modcaseid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromQuery] bool sendNotification = true) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);
            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            var uploadDir = Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, modcaseid);
            if (System.IO.Directory.Exists(uploadDir))
            {
                Directory.Delete(uploadDir, true);
            }

            database.DeleteSpecificModCase(modCase);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await modCaseAnnouncer.AnnounceModCase(modCase, ModCaseAction.Deleted, sendNotification);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            } 

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Deleted ModCase.");
            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }

        [HttpPatch("{modcaseid}")]
        public async Task<IActionResult> PatchSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromBody] JsonPatchDocument<ModCase> newValue, [FromQuery] bool sendNotification = true)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);
            if (modCase == null) 
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 ModCase not found.");
                return NotFound();
            }

            string oldUserId = modCase.UserId;
            // unchangeable values
            int id = modCase.Id;
            int caseid = modCase.CaseId;
            string guildId = modCase.GuildId;
            string username = modCase.Username;
            string nickname = modCase.Nickname;
            DateTime createdAt = modCase.CreatedAt;

            // json patch
            var serialized = JsonConvert.SerializeObject(newValue);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(modCase);

            // apply automated and unchangeable values
            modCase.Title = modCase.Title.Substring(0, Math.Min(modCase.Title.Length, 100)); // max length 100
            modCase.Punishment = modCase.Punishment.Substring(0, Math.Min(modCase.Punishment.Length, 100)); // max length 100
            if (modCase.Labels == null) {
                modCase.Labels = new string[0];
            }
            if (!Enumerable.Range(0, 4).Contains(modCase.Severity)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Severity is invalid.");
                return BadRequest("Severity between 0 and 3 is required.");
            }
            modCase.Labels = modCase.Labels.Distinct().ToArray();
            modCase.Id = id;
            modCase.CaseId = caseid;
            modCase.GuildId = guildId;
            modCase.Username = username;
            modCase.Nickname = nickname;
            modCase.CreatedAt = createdAt;
            modCase.LastEditedAt = DateTime.UtcNow;
            modCase.LastEditedByModId = currentUser.Id;

            if (oldUserId != modCase.UserId)  // if user id got updated, update nickname and username
            {
                var regex = @"^[0-9]{18}$";
                var match = Regex.Match(modCase.UserId, regex);
                if (!match.Success) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 UserId is invalid.");
                    return BadRequest("UserId is invalid.");
                }
                var currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId);
                if (currentReportedMember != null)
                {
                    modCase.Username = currentReportedMember.User.Username;
                    modCase.Nickname = currentReportedMember.Nick;
                }
                var currentReportedUser = await discord.FetchUserInfo(modCase.UserId);
                if (currentReportedUser == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid Discord UserId.");
                    return BadRequest("Invalid Discord UserId.");
                }
                if (currentReportedMember.User.Bot) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for bots.");
                    return BadRequest("Cannot create cases for bots.");
                }
            }

            database.UpdateModCase(modCase);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await modCaseAnnouncer.AnnounceModCase(modCase, ModCaseAction.Edited, sendNotification);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }  

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] ModCaseForCreateDto modCase, [FromQuery] bool sendNotification = true) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            var currentModerator = currentUser.Id;
            if (currentModerator == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Failed to fetch mod user info.");
                return BadRequest("Could not fetch own user info.");
            }

            ModCase newModCase = new ModCase();
            
            User currentReportedUser = await discord.FetchUserInfo(modCase.UserId);
            if (currentReportedUser == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid Discord UserId.");
                return BadRequest("Invalid Discord UserId.");
            }
            if (currentReportedUser.Bot) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for bots.");
                return BadRequest("Cannot create cases for bots.");
            }

            newModCase.Username = currentReportedUser.Username;

            GuildMember currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId);

            if (currentReportedMember != null)
            {
                newModCase.Nickname = currentReportedMember.Nick;
            }

            newModCase.CaseId = await database.GetHighestCaseIdForGuild(guildid) + 1;
            newModCase.Title = modCase.Title;
            newModCase.Description = modCase.Description;
            newModCase.GuildId = guildid;
            newModCase.ModId = currentModerator;
            newModCase.UserId = modCase.UserId;
            newModCase.Severity = modCase.Severity;
            newModCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt.HasValue)
                newModCase.OccuredAt = modCase.OccuredAt.Value;
            else
                newModCase.OccuredAt = DateTime.UtcNow;
            newModCase.LastEditedAt = DateTime.UtcNow;
            newModCase.LastEditedByModId = currentModerator;
            newModCase.Punishment = modCase.Punishment;
            newModCase.Labels = modCase.Labels.Distinct().ToArray();
            newModCase.Others = modCase.Others;
            newModCase.Valid = true;
            
            await database.SaveModCase(newModCase);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await modCaseAnnouncer.AnnounceModCase(newModCase, ModCaseAction.Created, sendNotification);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }        

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = newModCase.Id, caseid = newModCase.CaseId });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            String userOnly = String.Empty;
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                userOnly = currentUser.Id;
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            List<ModCase> modCases = new List<ModCase>();
            if (String.IsNullOrEmpty(userOnly)) {
                modCases = await database.SelectAllModCasesForGuild(guildid);       
            }
            else {
                modCases = await database.SelectAllModcasesForSpecificUserOnGuild(guildid, currentUser.Id);  
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(modCases);
        }


        [HttpGet("@me")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            List<ModCase> modCases = await database.SelectAllModcasesForSpecificUserOnGuild(guildid, currentUser.Id);       

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(modCases);
        }
    }
}
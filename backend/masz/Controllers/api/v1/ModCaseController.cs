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
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
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

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Deleted ModCase.");
            return Ok();
        }

        [HttpPatch("{modcaseid}")]
        public async Task<IActionResult> PatchSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromBody] JsonPatchDocument<ModCase> newValue)
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
            modCase.Id = id;
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
                var currentReportedUser = await discord.FetchMemberInfo(guildid, modCase.UserId);
                if (currentReportedUser != null)
                {
                    modCase.Username = currentReportedUser.User.Username;
                    modCase.Nickname = currentReportedUser.Nick;
                }
            }

            database.UpdateModCase(modCase);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok(modCase.Id);
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

            var currentModUserId = currentUser.Id;
            if (currentModUserId == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Failed to fetch mod user info.");
                return BadRequest("Could not fetch own user info.");
            }

            ModCase newModCase = new ModCase();
            
            var currentReportedUser = await discord.FetchMemberInfo(guildid, modCase.UserId);
            if (currentReportedUser != null)
            {
                newModCase.Username = currentReportedUser.User.Username;
                newModCase.Nickname = currentReportedUser.Nick;
            }

            newModCase.Title = modCase.Title;
            newModCase.Description = modCase.Description;
            newModCase.GuildId = guildid;
            newModCase.ModId = currentModUserId;
            newModCase.UserId = modCase.UserId;
            newModCase.Severity = modCase.Severity;
            newModCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt.HasValue)
                newModCase.OccuredAt = modCase.OccuredAt.Value;
            else
                newModCase.OccuredAt = DateTime.UtcNow;
            newModCase.LastEditedAt = DateTime.UtcNow;
            newModCase.LastEditedByModId = currentModUserId;
            newModCase.Punishment = modCase.Punishment;
            newModCase.Labels = modCase.Labels;
            newModCase.Others = modCase.Others;
            newModCase.Valid = true;
            
            await database.SaveModCase(newModCase);
            await database.SaveChangesAsync();

            if (sendNotification) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
                try {
                    await modCaseAnnouncer.AnnounceModCase(newModCase, "created");
                }
                catch(Exception e){
                    logger.LogError(e, "Failed to announce modcase.");
                }
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = newModCase.Id });
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
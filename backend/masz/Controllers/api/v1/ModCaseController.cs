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
    [Route("api/v1/modcases/{guildid}")]
    [Authorize]
    public class ModCaseController : ControllerBase
    {
        private readonly ILogger<ModCaseController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAnnouncer discordAnnouncer;
        private readonly IDiscordAPIInterface discord;
        private readonly IFilesHandler filesHandler;
        private readonly IPunishmentHandler punishmentHandler;

        public ModCaseController(ILogger<ModCaseController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IDiscordAnnouncer modCaseAnnouncer, IFilesHandler filesHandler, IPunishmentHandler punishmentHandler)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.discordAnnouncer = modCaseAnnouncer;
            this.discord = discordInterface;
            this.filesHandler = filesHandler;
            this.punishmentHandler = punishmentHandler;
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

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCase.");
            return Ok(modCase);
        }

        [HttpDelete("{modcaseid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
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

            try {
                filesHandler.DeleteDirectory(Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, modcaseid));
            } catch (Exception e) {
                logger.LogError(e, "Failed to delete files directory for modcase.");
            }

            database.DeleteSpecificModCase(modCase);
            await database.SaveChangesAsync();

            if (handlePunishment)
            {
                try {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                    await punishmentHandler.UndoPunishment(modCase, database);
                }
                catch(Exception e){
                    logger.LogError(e, "Failed to handle punishment for modcase.");
                }
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceModCase(modCase, RestAction.Deleted, currentUser, sendNotification);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Deleted ModCase.");
            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }

        [HttpPut("{modcaseid}")]
        public async Task<IActionResult> PutSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromBody] ModCaseForPutDto newValue, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null)
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
            ModCase oldModCase = (ModCase) modCase.Clone();

            modCase.Title = newValue.Title;
            modCase.Description = newValue.Description;
            modCase.UserId = newValue.UserId;
            if (newValue.OccuredAt.HasValue)
            {
                modCase.OccuredAt = newValue.OccuredAt.Value;
            }
            modCase.Punishment = newValue.Punishment;
            modCase.Labels = newValue.Labels.Distinct().ToArray();
            modCase.Others = newValue.Others;
            modCase.PunishmentType = newValue.PunishmentType;
            modCase.PunishedUntil = newValue.PunishedUntil;
            if (modCase.PunishmentType == PunishmentType.None) {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            }
            if (modCase.PunishedUntil == null) {
                modCase.PunishmentActive = modCase.PunishmentType != PunishmentType.None && modCase.PunishmentType != PunishmentType.Kick;
            } else {
                modCase.PunishmentActive = modCase.PunishedUntil > DateTime.UtcNow && modCase.PunishmentType != PunishmentType.None && modCase.PunishmentType != PunishmentType.Kick;
            }

            modCase.Id = oldModCase.Id;
            modCase.CaseId = oldModCase.CaseId;
            modCase.GuildId = oldModCase.GuildId;
            modCase.Username = oldModCase.Username;
            modCase.Discriminator = oldModCase.Discriminator;
            modCase.Nickname = oldModCase.Nickname;
            modCase.CreatedAt = oldModCase.CreatedAt;
            modCase.LastEditedAt = DateTime.UtcNow;
            modCase.LastEditedByModId = currentUser.Id;

            if (oldModCase.UserId != modCase.UserId)  // if user id got updated, update nickname and username
            {
                var currentReportedUser = await discord.FetchUserInfo(modCase.UserId, true);
                if (currentReportedUser == null) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid Discord UserId.");
                    return BadRequest("Invalid Discord UserId.");
                }
                if (currentReportedUser.Bot) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for bots.");
                    return BadRequest("Cannot create cases for bots.");
                }
                if (config.Value.SiteAdminDiscordUserIds.Contains(currentReportedUser.Id)) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for site admins.");
                    return BadRequest("Cannot create cases for site admins.");
                }
                modCase.Username = currentReportedUser.Username;  // update to new username
                modCase.Discriminator = currentReportedUser.Discriminator;

                var currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId);
                if (currentReportedMember != null)
                {
                    if (currentReportedMember.Roles.Contains(guildConfig.ModRoleId) || currentReportedMember.Roles.Contains(guildConfig.AdminRoleId)) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for team members.");
                        return BadRequest("Cannot create cases for team members.");
                    }
                    modCase.Nickname = currentReportedMember.Nick;  // update to new nickname if no member anymore leave old fetched nickname
                }
            }

            database.UpdateModCase(modCase);
            await database.SaveChangesAsync();

            if (handlePunishment)
            {
                if  ( oldModCase.UserId != modCase.UserId || oldModCase.PunishmentType != modCase.PunishmentType || oldModCase.PunishedUntil != modCase.PunishedUntil)
                {
                    try {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                        await punishmentHandler.UndoPunishment(oldModCase, database);
                        if (modCase.PunishmentActive || (modCase.PunishmentType == PunishmentType.Kick && oldModCase.PunishmentType != PunishmentType.Kick))
                        {
                            if (modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow)
                            {
                                await punishmentHandler.ExecutePunishment(modCase, database);
                            }
                        }
                    }
                    catch(Exception e){
                        logger.LogError(e, "Failed to handle punishment for modcase.");
                    }
                }
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceModCase(modCase, RestAction.Edited, currentUser, sendNotification);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }  

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] ModCaseForCreateDto modCase, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            Identity currentIdentity = await identityManager.GetIdentity(HttpContext);
            User currentUser = await currentIdentity.GetCurrentDiscordUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null)
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
            
            User currentReportedUser = await discord.FetchUserInfo(modCase.UserId, true);
            if (currentReportedUser == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Invalid Discord UserId.");
                return BadRequest("Invalid Discord UserId.");
            }
            if (currentReportedUser.Bot) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for bots.");
                return BadRequest("Cannot create cases for bots.");
            }
            if (config.Value.SiteAdminDiscordUserIds.Contains(currentReportedUser.Id)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for site admins.");
                return BadRequest("Cannot create cases for site admins.");
            }

            newModCase.Username = currentReportedUser.Username;
            newModCase.Discriminator = currentReportedUser.Discriminator;

            GuildMember currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId);

            if (currentReportedMember != null)
            {
                if (currentReportedMember.Roles.Contains(guildConfig.ModRoleId) || currentReportedMember.Roles.Contains(guildConfig.AdminRoleId)) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for team members.");
                    return BadRequest("Cannot create cases for team members.");
                }
                newModCase.Nickname = currentReportedMember.Nick;
            }

            newModCase.CaseId = await database.GetHighestCaseIdForGuild(guildid) + 1;
            newModCase.Title = modCase.Title;
            newModCase.Description = modCase.Description;
            newModCase.GuildId = guildid;
            newModCase.ModId = currentModerator;
            newModCase.UserId = modCase.UserId;
            newModCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt.HasValue)
                newModCase.OccuredAt = modCase.OccuredAt.Value;
            else
                newModCase.OccuredAt = newModCase.CreatedAt;
            newModCase.LastEditedAt = newModCase.CreatedAt;
            newModCase.LastEditedByModId = currentModerator;
            newModCase.Punishment = modCase.Punishment;
            newModCase.Labels = modCase.Labels.Distinct().ToArray();
            newModCase.Others = modCase.Others;
            newModCase.Valid = true;
            newModCase.PunishmentType = modCase.PunishmentType;
            newModCase.PunishedUntil = modCase.PunishedUntil;
            if (modCase.PunishmentType == PunishmentType.None) {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            }
            if (modCase.PunishedUntil == null) {
                newModCase.PunishmentActive = modCase.PunishmentType != PunishmentType.None && modCase.PunishmentType != PunishmentType.Kick;
            } else {
                newModCase.PunishmentActive = modCase.PunishedUntil > DateTime.UtcNow && modCase.PunishmentType != PunishmentType.None && modCase.PunishmentType != PunishmentType.Kick;
            }
            
            await database.SaveModCase(newModCase);
            await database.SaveChangesAsync();

            if (handlePunishment && (newModCase.PunishmentActive || newModCase.PunishmentType == PunishmentType.Kick))
            {
                if (newModCase.PunishedUntil == null || newModCase.PunishedUntil > DateTime.UtcNow)
                {
                    try {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                        await punishmentHandler.ExecutePunishment(newModCase, database);
                    }
                    catch(Exception e){
                        logger.LogError(e, "Failed to handle punishment for modcase.");
                    }
                }
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceModCase(newModCase, RestAction.Created, currentUser, sendNotification);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = newModCase.Id, caseid = newModCase.CaseId });
        }

        [HttpGet]
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
            if (!await currentIdentity.HasModRoleOrHigherOnGuild(guildid, this.database) && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id))
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
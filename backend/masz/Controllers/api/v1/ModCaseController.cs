using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/modcases/{guildid}")]
    [Authorize]
    public class ModCaseController : SimpleCaseController
    {
        private readonly ILogger<ModCaseController> logger;

        public ModCaseController(IServiceProvider serviceProvider, ILogger<ModCaseController> logger) : base(serviceProvider, logger) 
        {
            this.logger = logger;
        }

        [HttpGet("{modcaseid}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult auth = await this.HandleRequest(guildid, modcaseid, APIActionPermission.View);
            if (auth != null) {
                return auth;
            }
            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCase.");
            return Ok(modCase);
        }

        [HttpDelete("{modcaseid}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true, [FromQuery] bool announceDm = true, [FromQuery] bool forceDelete = false) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult auth = await this.HandleRequest(guildid, modcaseid, forceDelete ? APIActionPermission.ForceDelete : APIActionPermission.Delete);
            if (auth != null) {
                return auth;
            }
            User currentUser = await this.IsValidUser();
            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);

            if (forceDelete)
            {
                try {
                    filesHandler.DeleteDirectory(Path.Combine(config.Value.AbsolutePathToFileUpload, guildid, modcaseid));
                } catch (Exception e) {
                    logger.LogError(e, "Failed to delete files directory for modcase.");
                }

                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Force deleting ModCase.");
                database.DeleteSpecificModCase(modCase);
                await database.SaveChangesAsync();
            } else {
                modCase.MarkedToDeleteAt = DateTime.UtcNow.AddDays(7);
                modCase.DeletedByUserId = currentUser.Id;

                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Marking modcase as deleted.");
                database.UpdateModCase(modCase);
                await database.SaveChangesAsync();
            }

            if (handlePunishment)
            {
                try {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                    await punishmentHandler.UndoPunishment(modCase);
                }
                catch(Exception e){
                    logger.LogError(e, "Failed to handle punishment for modcase.");
                }
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceModCase(modCase, RestAction.Deleted, currentUser, sendNotification, announceDm);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Deleted ModCase.");
            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }

        [HttpPut("{modcaseid}")]
        public async Task<IActionResult> PutSpecificItem([FromRoute] string guildid, [FromRoute] string modcaseid, [FromBody] ModCaseForPutDto newValue, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true, [FromQuery] bool announceDm = true)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult auth = await this.HandleRequest(guildid, modcaseid, APIActionPermission.Edit);
            if (auth != null) {
                return auth;
            }
            User currentUser = await this.IsValidUser();
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);
            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);
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
                var currentReportedUser = await discord.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
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

                var currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
                if (currentReportedMember != null)
                {
                    if (currentReportedMember.Roles.Intersect(guildConfig.ModRoles).Any() || currentReportedMember.Roles.Intersect(guildConfig.AdminRoles).Any()) {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for team members.");
                        return BadRequest("Cannot create cases for team members.");
                    }
                    modCase.Nickname = currentReportedMember.Nick;  // update to new nickname if no member anymore leave old fetched nickname
                }
            }

            database.UpdateModCase(modCase);
            await database.SaveChangesAsync();

            Task announcementTask = new Task(() => {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
                discordAnnouncer.AnnounceModCase(modCase, RestAction.Edited, currentUser, sendNotification, announceDm);
            });
            announcementTask.Start();

            if (handlePunishment)
            {
                if  ( oldModCase.UserId != modCase.UserId || oldModCase.PunishmentType != modCase.PunishmentType || oldModCase.PunishedUntil != modCase.PunishedUntil)
                {
                    Task punishmentTask = new Task(() => {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                        punishmentHandler.UndoPunishment(oldModCase);
                        if (modCase.PunishmentActive || (modCase.PunishmentType == PunishmentType.Kick && oldModCase.PunishmentType != PunishmentType.Kick))
                        {
                            if (modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow)
                            {
                                punishmentHandler.ExecutePunishment(modCase);
                            }
                        }
                    });
                    punishmentTask.Start();
                }
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Resource updated.");
            return Ok(new { id = modCase.Id, caseid = modCase.CaseId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] ModCaseForCreateDto modCase, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true, [FromQuery] bool announceDm = true) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult auth = await this.HandleRequest(guildid, DiscordPermission.Moderator);
            if (auth != null) {
                return auth;
            }
            User currentUser = await this.IsValidUser();
            GuildConfig guildConfig = await database.SelectSpecificGuildConfig(guildid);

            ModCase newModCase = new ModCase();
            
            User currentReportedUser = await discord.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
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

            GuildMember currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);

            if (currentReportedMember != null)
            {
                if (currentReportedMember.Roles.Intersect(guildConfig.ModRoles).Any() || currentReportedMember.Roles.Intersect(guildConfig.AdminRoles).Any()) {
                    logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Cannot create cases for team members.");
                    return BadRequest("Cannot create cases for team members.");
                }
                newModCase.Nickname = currentReportedMember.Nick;
            }

            newModCase.CaseId = await database.GetHighestCaseIdForGuild(guildid) + 1;
            newModCase.Title = modCase.Title;
            newModCase.Description = modCase.Description;
            newModCase.GuildId = guildid;
            newModCase.ModId = currentUser.Id;
            newModCase.UserId = modCase.UserId;
            newModCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt.HasValue)
                newModCase.OccuredAt = modCase.OccuredAt.Value;
            else
                newModCase.OccuredAt = newModCase.CreatedAt;
            newModCase.LastEditedAt = newModCase.CreatedAt;
            newModCase.LastEditedByModId = currentUser.Id;
            newModCase.Punishment = modCase.Punishment;
            newModCase.Labels = modCase.Labels.Distinct().ToArray();
            newModCase.Others = modCase.Others;
            newModCase.Valid = true;
            newModCase.CreationType = CaseCreationType.Default;
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

            Task announcementTask = new Task(() => {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
                discordAnnouncer.AnnounceModCase(newModCase, RestAction.Created, currentUser, sendNotification, announceDm);
            });
            announcementTask.Start();

            if (handlePunishment && (newModCase.PunishmentActive || newModCase.PunishmentType == PunishmentType.Kick))
            {
                if (newModCase.PunishedUntil == null || newModCase.PunishedUntil > DateTime.UtcNow)
                {
                    Task punishmentTask = new Task(() => {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                        punishmentHandler.ExecutePunishment(newModCase);
                    });
                    punishmentTask.Start();
                }
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = newModCase.Id, caseid = newModCase.CaseId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            GuildConfig guildConfig = await this.database.SelectSpecificGuildConfig(guildid);
            if (guildConfig == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered");
            }
            User currentUser = await this.IsValidUser();
            String userOnly = String.Empty;
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid))
            {
                userOnly = currentUser.Id;
            }
            // ========================================================
            List<ModCase> modCases = new List<ModCase>();
            if (String.IsNullOrEmpty(userOnly)) {
                modCases = await database.SelectAllModCasesForGuild(guildid, startPage, 20);       
            }
            else {
                modCases = await database.SelectAllModcasesForSpecificUserOnGuild(guildid, currentUser.Id, startPage, 20);  
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(modCases);
        }

        [HttpPost("{modcaseid}/lock")]
        public async Task<IActionResult> LockComments([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult auth = await this.HandleRequest(guildid, modcaseid, APIActionPermission.Edit);
            if (auth != null) {
                return auth;
            }
            User currentUser = await this.IsValidUser();
            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);
            if (!modCase.AllowComments) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Comments are already locked.");
                return BadRequest("Comments are already locked.");
            }

            modCase.AllowComments = false;
            modCase.LockedAt = DateTime.Now;
            modCase.LockedByUserId = currentUser.Id;

            database.UpdateModCase(modCase);
            await database.SaveChangesAsync();

            return Ok(modcaseid);
        }

        [HttpDelete("{modcaseid}/lock")]
        public async Task<IActionResult> UnlockComments([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult auth = await this.HandleRequest(guildid, modcaseid, APIActionPermission.Edit);
            if (auth != null) {
                return auth;
            }
            User currentUser = await this.IsValidUser();
            ModCase modCase = await database.SelectSpecificModCase(guildid, modcaseid);

            modCase.AllowComments = true;
            modCase.LockedAt = null;
            modCase.LockedByUserId = null;

            database.UpdateModCase(modCase);
            await database.SaveChangesAsync();

            return Ok(modcaseid);
        }
    }
}
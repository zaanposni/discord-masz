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
    [Route("internalapi/v1/guilds/{guildid}/modcases")]
    public class ModCaseInternalController : ControllerBase
    {
        private readonly ILogger<ModCaseInternalController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAnnouncer discordAnnouncer;
        private readonly IDiscordAPIInterface discord;
        private readonly IFilesHandler filesHandler;
        private readonly IPunishmentHandler punishmentHandler;

        public ModCaseInternalController(ILogger<ModCaseInternalController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IDiscordAnnouncer modCaseAnnouncer, IFilesHandler filesHandler, IPunishmentHandler punishmentHandler)
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

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] string guildid, [FromBody] ModCaseForInternalCreateDto modCase, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true, [FromQuery] bool announceDm = true) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (string.IsNullOrEmpty(this.config.Value.DiscordBotToken)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Authorization header not defined.");
                return Unauthorized();
            }

            string auth = String.Empty;
            try {
                auth = Request.Headers["Authorization"];
            } catch(Exception e) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Authorization header not defined.", e);
                return Unauthorized();
            }

            if (this.config.Value.DiscordBotToken != auth) {
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

            var actor = await discord.FetchMemberInfo(guildid, modCase.ModId, true);
            if (actor == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            if (!actor.Roles.Contains(guildConfig.ModRoleId) && !actor.Roles.Contains(guildConfig.AdminRoleId) && !config.Value.SiteAdminDiscordUserIds.Contains(actor.User.Id)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            // ========================================================

            User currentUser = await discord.FetchUserInfo(modCase.ModId);
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

            GuildMember currentReportedMember = await discord.FetchMemberInfo(guildid, modCase.UserId, true);

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
            newModCase.CreationType = CaseCreationType.ByCommand;
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

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Sending notification.");
            try {
                await discordAnnouncer.AnnounceModCase(newModCase, RestAction.Created, currentUser, sendNotification, announceDm);
            }
            catch(Exception e){
                logger.LogError(e, "Failed to announce modcase.");
            }

            if (handlePunishment && (newModCase.PunishmentActive || newModCase.PunishmentType == PunishmentType.Kick))
            {
                if (newModCase.PunishedUntil == null || newModCase.PunishedUntil > DateTime.UtcNow)
                {
                    try {
                        logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Handling punishment.");
                        await punishmentHandler.ExecutePunishment(newModCase);
                    }
                    catch(Exception e){
                        logger.LogError(e, "Failed to handle punishment for modcase.");
                    }
                }
            }

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, new { id = newModCase.Id, caseid = newModCase.CaseId });
        }
    }
}
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
    [Route("api/v1/guilds/{guildid}")]
    [Authorize]
    public class ModCaseTableController : ControllerBase
    {
        private readonly ILogger<ModCaseTableController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAnnouncer discordAnnouncer;
        private readonly IDiscordAPIInterface discord;
        private readonly IFilesHandler filesHandler;
        private readonly IPunishmentHandler punishmentHandler;

        public ModCaseTableController(ILogger<ModCaseTableController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface, IDiscordAnnouncer modCaseAnnouncer, IFilesHandler filesHandler, IPunishmentHandler punishmentHandler)
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

        [HttpGet("modcasetable")]
        public async Task<IActionResult> GetAllModCases([FromRoute] string guildid) 
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

            List<ModCaseTableEntry> table = new List<ModCaseTableEntry>();
            foreach (var c in modCases)
            {
                table.Add( new ModCaseTableEntry() {
                    ModCase = c,
                    Suspect = await discord.FetchUserInfo(c.UserId),
                    Moderator = await discord.FetchUserInfo(c.ModId)
                });
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(table);
        }

        [HttpGet("punishmenttable")]
        public async Task<IActionResult> GetAllPunishments([FromRoute] string guildid) 
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
                modCases = (await database.SelectAllModCasesForGuild(guildid)).Where(x => x.PunishmentActive == true).ToList();
            }
            else {
                modCases = (await database.SelectAllModcasesForSpecificUserOnGuild(guildid, currentUser.Id)).Where(x => x.PunishmentActive == true).ToList();
            }

            List<ModCaseTableEntry> table = new List<ModCaseTableEntry>();
            foreach (var c in modCases)
            {
                table.Add( new ModCaseTableEntry() {
                    ModCase = c,
                    Suspect = await discord.FetchUserInfo(c.UserId),
                    Moderator = await discord.FetchUserInfo(c.ModId)
                });
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(table);
        }
    }
}
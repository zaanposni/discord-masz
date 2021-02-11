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
        public async Task<IActionResult> GetAllModCases([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null) 
        {
            return await generateTable(guildid, false, startPage, search);
        }

        [HttpGet("punishmenttable")]
        public async Task<IActionResult> GetAllPunishments([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null) 
        {
            return await generateTable(guildid, true, startPage, search);
        }

        private async Task<IActionResult> generateTable(string guildid, bool onlyPunishments, int startPage=0, string search=null) {
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
                if (String.IsNullOrWhiteSpace(search)) {
                    modCases = await database.SelectAllModCasesForGuild(guildid, startPage, 20);       
                } else {
                    modCases = await database.SelectAllModCasesForGuild(guildid);       
                }
            }
            else {                
                if (String.IsNullOrWhiteSpace(search)) {
                    modCases = await database.SelectAllModcasesForSpecificUserOnGuild(guildid, currentUser.Id, startPage, 20);       
                } else {
                    modCases = await database.SelectAllModcasesForSpecificUserOnGuild(guildid, currentUser.Id);       
                }
            }

            if (onlyPunishments) {
                modCases = modCases.Where(x => x.PunishmentActive == true).ToList();
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

            if (!String.IsNullOrWhiteSpace(search)) {
                table = table.Where(t =>
                    contains(t.ModCase.Title, search) ||
                    contains(t.ModCase.Description, search) ||
                    contains(t.ModCase.Punishment, search) ||
                    contains(t.ModCase.Username, search) ||
                    contains(t.ModCase.Discriminator, search) ||
                    contains(t.ModCase.Nickname, search) ||
                    contains(t.ModCase.UserId, search) ||
                    contains(t.ModCase.ModId, search) ||
                    contains(t.ModCase.LastEditedByModId, search) ||
                    contains(t.ModCase.CreatedAt, search) ||
                    contains(t.ModCase.OccuredAt, search) ||
                    contains(t.ModCase.LastEditedAt, search) ||
                    contains(t.ModCase.Labels, search) ||
                    contains(t.ModCase.CaseId.ToString(), search) ||
                    contains("#" + t.ModCase.CaseId.ToString(), search) ||

                    contains(t.Moderator, search) ||
                    contains(t.Suspect, search)
                ).ToList();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCases.");
            return Ok(table);
        }

        private bool contains(string obj, string search) {
            if (String.IsNullOrWhiteSpace(obj)) {
                return false;
            }
            return obj.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool contains(DateTime obj, string search) {
            if (obj == null) {
                return false;
            }
            return obj.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool contains(string[] obj, string search) {
            if (obj == null) {
                return false;
            }
            return obj.Contains(search);
        }

        private bool contains(User obj, string search) {
            if (obj == null) {
                return false;
            }
            return contains(obj.Username, search) || contains(obj.Discriminator, search);
        }
    }
}
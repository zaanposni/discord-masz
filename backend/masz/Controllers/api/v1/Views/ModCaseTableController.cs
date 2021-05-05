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
    public class ModCaseTableController : SimpleCaseController
    {
        private readonly ILogger<ModCaseTableController> logger;

        public ModCaseTableController(ILogger<ModCaseTableController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            this.logger = logger;
        }

        [HttpGet("modcasetable")]
        public async Task<IActionResult> GetAllModCases([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null) 
        {
            return await generateTable(guildid, ModcaseTableType.Default, startPage, search, ModcaseTableSortType.Default);
        }

        [HttpGet("punishmenttable")]
        public async Task<IActionResult> GetAllPunishments([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null) 
        {
            return await generateTable(guildid, ModcaseTableType.OnlyPunishments, startPage, search, ModcaseTableSortType.Default);
        }

        [HttpGet("expiringpunishment")]
        public async Task<IActionResult> GetExpiringPunishments([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null) 
        {
            return await generateTable(guildid, ModcaseTableType.OnlyPunishments, startPage, search, ModcaseTableSortType.SortByExpiring);
        }

        [HttpGet("casebin")]
        public async Task<IActionResult> GetDeletedModCases([FromRoute] string guildid, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null) 
        {
            return await generateTable(guildid, ModcaseTableType.OnlyBin, startPage, search, ModcaseTableSortType.SortByDeleting);
        }

        private async Task<IActionResult> generateTable(string guildid, ModcaseTableType tableType, int startPage=0, string search=null, ModcaseTableSortType sortBy = ModcaseTableSortType.Default) {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            User currentUser = await this.IsValidUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            String userOnly = String.Empty;
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid))
            {
                userOnly = currentUser.Id;
            }
            // ========================================================

            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            // SELECT
            List<ModCase> modCases = await database.SelectAllModCasesForGuild(guildid);

            // ORDER BY
            switch(sortBy) {
                case ModcaseTableSortType.SortByExpiring:
                    modCases = modCases.Where(x => x.PunishedUntil != null).OrderBy(x => x.PunishedUntil).ToList();
                    break;
                case ModcaseTableSortType.SortByDeleting:
                    modCases = modCases.OrderBy(x => x.MarkedToDeleteAt).ToList();
                    break;
            }
            
            // WHERE
            if (! String.IsNullOrEmpty(userOnly)) {
                modCases = modCases.Where(x => x.UserId == userOnly).ToList();
            }
            switch(tableType) {
                case ModcaseTableType.OnlyPunishments:
                    modCases = modCases.Where(x => x.PunishmentActive).ToList();
                    break;
                case ModcaseTableType.OnlyBin:
                    modCases = modCases.Where(x => x.MarkedToDeleteAt != null).ToList();
                    break;
            }

            // LIMIT
            if (String.IsNullOrEmpty(search)) {
                modCases = modCases.Skip(startPage * 20).Take(20).ToList();
            }

            List<ModCaseTableEntry> table = new List<ModCaseTableEntry>();
            foreach (var c in modCases)
            {
                table.Add( new ModCaseTableEntry() {
                    ModCase = c,
                    Suspect = await discord.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache),
                    Moderator = await discord.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache)
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
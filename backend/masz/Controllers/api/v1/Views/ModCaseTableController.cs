using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Models;
using masz.Models.Views;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using masz.Enums;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}")]
    [Authorize]
    public class ModCaseTableController : SimpleCaseController
    {
        private readonly ILogger<ModCaseTableController> _logger;

        public ModCaseTableController(ILogger<ModCaseTableController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            _logger = logger;
        }

        [HttpGet("modcasetable")]
        public async Task<IActionResult> GetAllModCases([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null)
        {
            return await generateTable(guildId, ModcaseTableType.Default, startPage, search, ModcaseTableSortType.Default);
        }

        [HttpGet("punishmenttable")]
        public async Task<IActionResult> GetAllPunishments([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null)
        {
            return await generateTable(guildId, ModcaseTableType.OnlyPunishments, startPage, search, ModcaseTableSortType.Default);
        }

        [HttpGet("expiringpunishment")]
        public async Task<IActionResult> GetExpiringPunishments([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null)
        {
            return await generateTable(guildId, ModcaseTableType.OnlyPunishments, startPage, search, ModcaseTableSortType.SortByExpiring);
        }

        [HttpGet("casebin")]
        public async Task<IActionResult> GetDeletedModCases([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage=0, [FromQuery] string search=null)
        {
            return await generateTable(guildId, ModcaseTableType.OnlyBin, startPage, search, ModcaseTableSortType.SortByDeleting);
        }

        private async Task<IActionResult> generateTable(ulong guildId, ModcaseTableType tableType, int startPage=0, string search=null, ModcaseTableSortType sortBy = ModcaseTableSortType.Default) {
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);

            ulong userOnly = 0;
            if (! await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = identity.GetCurrentUser().Id;
            }
            // ========================================================

            // SELECT
            List<ModCase> modCases = await ModCaseRepository.CreateDefault(_serviceProvider, identity).GetCasesForGuild(guildId);

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
            if (userOnly!= 0) {
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

            bool publishMod = guildConfig.PublishModeratorInfo || await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);
            List<ModCaseTableEntry> table = new List<ModCaseTableEntry>();
            foreach (var c in modCases)
            {
                var entry = new ModCaseTableEntry(
                    c,
                    await _discordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache)
                );
                if (!publishMod) {
                    entry.RemoveModeratorInfo();
                }
                table.Add(entry);
            }

            if (!String.IsNullOrWhiteSpace(search)) {
                table = table.Where(t =>
                    contains(t.ModCase.Title, search) ||
                    contains(t.ModCase.Description, search) ||
                    contains(t.ModCase.GetPunishment(_translator), search) ||
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

        private bool contains(DiscordUserView obj, string search) {
            if (obj == null) {
                return false;
            }
            return contains(obj.Username, search) || contains(obj.Discriminator, search);
        }
    }
}
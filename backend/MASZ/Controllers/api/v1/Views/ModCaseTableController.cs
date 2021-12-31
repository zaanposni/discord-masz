using MASZ.Dtos.ModCase;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}")]
    [Authorize]
    public class ModCaseTableController : SimpleCaseController
    {
        public ModCaseTableController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("modcasetable")]
        public async Task<IActionResult> GetAllModCases([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0, [FromBody] ModCaseTableFilterDto search = null)
        {
            return Ok(await GenerateTable(guildId, ModcaseTableType.Default, startPage, search, ModcaseTableSortType.Default));
        }

        [HttpPost("expiringpunishment")]
        public async Task<IActionResult> GetExpiringPunishments([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0, [FromBody] ModCaseTableFilterDto search = null)
        {
            return Ok(await GenerateTable(guildId, ModcaseTableType.OnlyPunishments, startPage, search, ModcaseTableSortType.SortByExpiring));
        }

        [HttpPost("casebin")]
        public async Task<IActionResult> GetDeletedModCases([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0, [FromBody] ModCaseTableFilterDto search = null)
        {
            return Ok(await GenerateTable(guildId, ModcaseTableType.OnlyBin, startPage, search, ModcaseTableSortType.SortByDeleting));
        }

        private async Task<CaseTable> GenerateTable(ulong guildId, ModcaseTableType tableType, int startPage = 0, ModCaseTableFilterDto search = null, ModcaseTableSortType sortBy = ModcaseTableSortType.Default)
        {
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);

            ulong userOnly = 0;
            if (!await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = identity.GetCurrentUser().Id;
            }
            // ========================================================

            // SELECT
            List<ModCase> modCases = await ModCaseRepository.CreateDefault(_serviceProvider, identity).GetCasesForGuild(guildId);

            // ORDER BY
            switch (sortBy)
            {
                case ModcaseTableSortType.SortByExpiring:
                    modCases = modCases.Where(x => x.PunishedUntil != null).OrderBy(x => x.PunishedUntil).ToList();
                    break;
                case ModcaseTableSortType.SortByDeleting:
                    modCases = modCases.OrderBy(x => x.MarkedToDeleteAt).ToList();
                    break;
            }

            // WHERE
            if (userOnly != 0)
            {
                modCases = modCases.Where(x => x.UserId == userOnly).ToList();
            }

            switch (tableType)
            {
                case ModcaseTableType.OnlyPunishments:
                    modCases = modCases.Where(x => x.PunishmentActive).ToList();
                    break;
                case ModcaseTableType.OnlyBin:
                    modCases = modCases.Where(x => x.MarkedToDeleteAt != null).ToList();
                    break;
            }

            bool publishMod = guildConfig.PublishModeratorInfo || await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);
            List<ModCaseTableEntry> tmp = new();
            foreach (var c in modCases)
            {
                var entry = new ModCaseTableEntry(
                    c,
                    await _discordAPI.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (!publishMod)
                {
                    entry.RemoveModeratorInfo();
                }
                tmp.Add(entry);
            }

            IEnumerable<ModCaseTableEntry> table = tmp.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search?.CustomTextFilter))
            {
                table = table.Where(t =>
                    Contains(t.ModCase.Title, search.CustomTextFilter) ||
                    Contains(t.ModCase.Description, search.CustomTextFilter) ||
                    Contains(t.ModCase.GetPunishment(_translator), search.CustomTextFilter) ||
                    Contains(t.ModCase.Username, search.CustomTextFilter) ||
                    Contains(t.ModCase.Discriminator, search.CustomTextFilter) ||
                    Contains(t.ModCase.Nickname, search.CustomTextFilter) ||
                    Contains(t.ModCase.UserId, search.CustomTextFilter) ||
                    Contains(t.ModCase.ModId, search.CustomTextFilter) ||
                    Contains(t.ModCase.LastEditedByModId, search.CustomTextFilter) ||
                    Contains(t.ModCase.CreatedAt, search.CustomTextFilter) ||
                    Contains(t.ModCase.OccuredAt, search.CustomTextFilter) ||
                    Contains(t.ModCase.LastEditedAt, search.CustomTextFilter) ||
                    Contains(t.ModCase.Labels, search.CustomTextFilter) ||
                    Contains(t.ModCase.CaseId.ToString(), search.CustomTextFilter) ||
                    Contains("#" + t.ModCase.CaseId.ToString(), search.CustomTextFilter) ||

                    Contains(t.Moderator, search.CustomTextFilter) ||
                    Contains(t.Suspect, search.CustomTextFilter)
                );
            }

            if (search?.UserIds != null && search.UserIds.Count > 0)
            {
                table = table.Where(x => search.UserIds.Contains(x.ModCase.UserId));
            }
            if (search?.ModeratorIds != null && search.ModeratorIds.Count > 0)
            {
                table = table.Where(x =>
                    search.ModeratorIds.Contains(x.ModCase.ModId) ||
                    search.ModeratorIds.Contains(x.ModCase.LastEditedByModId)
                );
            }
            if (search?.Since != null && search.Since != DateTime.MinValue)
            {
                table = table.Where(x => x.ModCase.CreatedAt >= search.Since);
            }
            if (search?.Before != null && search.Before != DateTime.MinValue)
            {
                table = table.Where(x => x.ModCase.CreatedAt <= search.Before);
            }
            if (search?.PunishedUntilMin != null && search.PunishedUntilMin != DateTime.MinValue)
            {
                table = table.Where(x => x.ModCase.PunishedUntil >= search.PunishedUntilMin);
            }
            if (search?.PunishedUntilMax != null && search.PunishedUntilMax != DateTime.MinValue)
            {
                table = table.Where(x => x.ModCase.PunishedUntil <= search.PunishedUntilMax);
            }
            if (search?.Edited != null)
            {
                table = table.Where(x => x.ModCase.LastEditedAt == x.ModCase.CreatedAt != search.Edited.Value);
            }
            if (search?.CreationTypes != null && search.CreationTypes.Count > 0)
            {
                table = table.Where(x => search.CreationTypes.Contains(x.ModCase.CreationType));
            }
            if (search?.PunishmentTypes != null && search.PunishmentTypes.Count > 0)
            {
                table = table.Where(x => search.PunishmentTypes.Contains(x.ModCase.PunishmentType));
            }
            if (search?.PunishmentActive != null)
            {
                table = table.Where(x => x.ModCase.PunishmentActive == search.PunishmentActive.Value);
            }
            if (search?.LockedComments != null)
            {
                table = table.Where(x => x.ModCase.AllowComments != search.LockedComments.Value);
            }
            if (search?.MarkedToDelete != null)
            {
                table = table.Where(x => x.ModCase.MarkedToDeleteAt.HasValue == search.MarkedToDelete.Value);
            }

            return new CaseTable(table.Skip(startPage * 20).Take(20).ToList(), table.Count());
        }

        private static bool Contains(string obj, string search)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return false;
            }
            return obj.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool Contains(DateTime obj, string search)
        {
            if (obj == default)
            {
                return false;
            }
            return obj.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool Contains(string[] obj, string search)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.Contains(search);
        }

        private static bool Contains(DiscordUserView obj, string search)
        {
            if (obj == null)
            {
                return false;
            }
            return Contains(obj.Username, search) || Contains(obj.Discriminator, search);
        }
    }
}
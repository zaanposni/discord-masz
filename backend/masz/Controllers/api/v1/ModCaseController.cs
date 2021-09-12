using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/")]
    [Authorize]
    public class ModCaseController : SimpleCaseController
    {
        private readonly ILogger<ModCaseController> _logger;

        public ModCaseController(IServiceProvider serviceProvider, ILogger<ModCaseController> logger) : base(serviceProvider, logger)
        {
            _logger = logger;
        }

        [HttpGet("{caseId}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            CaseView modCase = new CaseView(await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetModCase(guildId, caseId));

            if (! (await GetRegisteredGuild(guildId)).PublishModeratorInfo)
            {
                if (! await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
                {
                    modCase.RemoveModeratorInfo();
                }
            }

            return Ok(modCase);
        }

        [HttpDelete("{caseId}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true, [FromQuery] bool forceDelete = false)
        {
            await RequirePermission(guildId, caseId, forceDelete ? APIActionPermission.ForceDelete : APIActionPermission.Delete);

            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).DeleteModCase(guildId, caseId, forceDelete, handlePunishment, sendNotification);

            return Ok(modCase);
        }

        [HttpPut("{caseId}")]
        public async Task<IActionResult> PutSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromBody] ModCaseForPutDto newValue, [FromQuery] bool sendNotification = true, [FromQuery] bool handlePunishment = true)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);

            Identity currentIdentity = await GetIdentity();
            var repo = ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity);

            ModCase modCase = await repo.GetModCase(guildId, caseId);
            ModCase oldModCase = (ModCase) modCase.Clone();

            modCase.Title = newValue.Title;
            modCase.Description = newValue.Description;
            modCase.UserId = newValue.UserId;
            if (newValue.OccuredAt.HasValue)
            {
                modCase.OccuredAt = newValue.OccuredAt.Value;
            }
            modCase.Labels = newValue.Labels.Distinct().ToArray();
            modCase.Others = newValue.Others;
            modCase.PunishmentType = newValue.PunishmentType;
            modCase.PunishedUntil = newValue.PunishedUntil;
            modCase.LastEditedByModId = currentIdentity.GetCurrentUser().Id;

            modCase = await repo.UpdateModCase(modCase, handlePunishment, sendNotification);

            return Ok(modCase);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] ulong guildId, [FromBody] ModCaseForCreateDto modCaseDto, [FromQuery] bool sendPublicNotification = true, [FromQuery] bool handlePunishment = true, [FromQuery] bool sendDmNotification = true)
        {
            Identity currentIdentity = await GetIdentity();
            if (! await currentIdentity.HasPermissionToExecutePunishment(guildId, modCaseDto.PunishmentType)) return Unauthorized();

            ModCase newModCase = new ModCase();

            newModCase.Title = modCaseDto.Title;
            newModCase.Description = modCaseDto.Description;
            newModCase.GuildId = guildId;
            newModCase.ModId = currentIdentity.GetCurrentUser().Id;
            newModCase.UserId = modCaseDto.UserId;
            newModCase.Labels = modCaseDto.Labels.Distinct().ToArray();
            newModCase.Others = modCaseDto.Others;
            newModCase.CreationType = CaseCreationType.Default;
            newModCase.PunishmentType = modCaseDto.PunishmentType;
            newModCase.PunishedUntil = modCaseDto.PunishedUntil;

            newModCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).CreateModCase(newModCase, handlePunishment, sendPublicNotification, sendDmNotification);

            return StatusCode(201, newModCase);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage=0)
        {
            Identity currentIdentity = await GetIdentity();
            ulong userOnly = 0;
            if (! await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = currentIdentity.GetCurrentUser().Id;
            }
            // ========================================================
            List<CaseView> modCases = new List<CaseView>();
            if (userOnly == 0) {
                modCases = (await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetCasePagination(guildId, startPage)).Select(x => new CaseView(x)).ToList();
            }
            else {
                modCases = (await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetCasePaginationFilteredForUser(guildId, userOnly, startPage)).Select(x => new CaseView(x)).ToList();
            }

            if (! (await GetRegisteredGuild(guildId)).PublishModeratorInfo)
            {
                if (! await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
                {
                    foreach (var modCase in modCases)
                    {
                        modCase.RemoveModeratorInfo();
                    }
                }
            }

            return Ok(modCases);
        }

        [HttpPost("{caseId}/lock")]
        public async Task<IActionResult> LockComments([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);

            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).LockCaseComments(guildId, caseId, currentIdentity.GetCurrentUser());

            return Ok(modCase);
        }

        [HttpDelete("{caseId}/lock")]
        public async Task<IActionResult> UnlockComments([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);

            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).UnlockCaseComments(guildId, caseId);

            return Ok(modCase);
        }
    }
}
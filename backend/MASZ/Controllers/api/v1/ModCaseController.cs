using MASZ.Dtos.ModCase;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/")]
    [Authorize]
    public class ModCaseController : SimpleCaseController
    {
        public ModCaseController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("labels")]
        public async Task<IActionResult> Get([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            return Ok(await ModCaseRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetLabelUsages(guildId));
        }

        [HttpGet("{caseId}")]
        public async Task<IActionResult> GetSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            CaseView modCase = new(await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetModCase(guildId, caseId));

            if (!(await GetRegisteredGuild(guildId)).PublishModeratorInfo)
            {
                if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
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
            if (!await currentIdentity.HasPermissionToExecutePunishment(guildId, newValue.PunishmentType))
            {
                throw new UnauthorizedException();
            }

            var repo = ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity);

            ModCase modCase = await repo.GetModCase(guildId, caseId);

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
            if (!await currentIdentity.HasPermissionToExecutePunishment(guildId, modCaseDto.PunishmentType))
            {
                throw new UnauthorizedException();
            }

            ModCase newModCase = new()
            {
                Title = modCaseDto.Title,
                Description = modCaseDto.Description,
                GuildId = guildId,
                ModId = currentIdentity.GetCurrentUser().Id,
                UserId = modCaseDto.UserId,
                Labels = modCaseDto.Labels.Distinct().ToArray(),
                Others = modCaseDto.Others
            };
            if (modCaseDto.OccuredAt.HasValue)
            {
                newModCase.OccuredAt = modCaseDto.OccuredAt.Value;
            }
            newModCase.CreationType = CaseCreationType.Default;
            newModCase.PunishmentType = modCaseDto.PunishmentType;
            newModCase.PunishedUntil = modCaseDto.PunishedUntil;

            newModCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).CreateModCase(newModCase, handlePunishment, sendPublicNotification, sendDmNotification);

            return StatusCode(201, newModCase);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0)
        {
            Identity currentIdentity = await GetIdentity();
            ulong userOnly = 0;
            if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = currentIdentity.GetCurrentUser().Id;
            }
            // ========================================================
            List<CaseView> modCases = new();
            if (userOnly == 0)
            {
                modCases = (await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetCasePagination(guildId, startPage)).Select(x => new CaseView(x)).ToList();
            }
            else
            {
                modCases = (await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetCasePaginationFilteredForUser(guildId, userOnly, startPage)).Select(x => new CaseView(x)).ToList();
            }

            if (!(await GetRegisteredGuild(guildId)).PublishModeratorInfo)
            {
                if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
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
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).LockCaseComments(guildId, caseId);

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

        [HttpPost("{caseId}/active")]
        public async Task<IActionResult> ActivateCase([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);

            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).ActivateModCase(guildId, caseId);

            return Ok(modCase);
        }

        [HttpDelete("{caseId}/active")]
        public async Task<IActionResult> DeactivateCase([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.Edit);

            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).DeactivateModCase(guildId, caseId);

            return Ok(modCase);
        }
    }
}
using MASZ.Models;
using MASZ.Enums;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MASZ.Dtos.Appeal;
using MASZ.Exceptions;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/appealstructures")]
    public class AppealStructureController : SimpleController
    {

        public AppealStructureController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAppealStructures([FromRoute] ulong guildId)
        {
            List<AppealStructure> appealStructures = await AppealStructureRepository.CreateDefault(_serviceProvider).GetForGuild(guildId);

            return Ok(appealStructures.Where(x => !x.Deleted).Select(x => new AppealStructureView(x)));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAppealStructure([FromRoute] ulong guildId, [FromBody] AppealStructureDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            await GetRegisteredGuild(guildId);

            AppealStructure appealStructure = new();

            appealStructure.GuildId = guildId;
            appealStructure.SortOrder = dto.SortOrder;
            appealStructure.Question = dto.Question.Trim();
            appealStructure.Deleted = false;

            appealStructure = await AppealStructureRepository.CreateDefault(_serviceProvider).Create(appealStructure);

            return Ok(new AppealStructureView(appealStructure));
        }

        [HttpPut("reorder")]
        [Authorize]
        public async Task<IActionResult> ReorderAppealStructures([FromRoute] ulong guildId, [FromBody] List<AppealStructureOrderDto> dtos)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            await GetRegisteredGuild(guildId);

            foreach (AppealStructureOrderDto dto in dtos)
            {
                AppealStructure appealStructure = await AppealStructureRepository.CreateDefault(_serviceProvider).GetById(guildId, dto.Id);

                appealStructure.SortOrder = dto.SortOrder;

                await AppealStructureRepository.CreateDefault(_serviceProvider).Update(appealStructure);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAppealStructure([FromRoute] ulong guildId, [FromRoute] int id, [FromBody] AppealStructureDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            await GetRegisteredGuild(guildId);

            AppealStructureRepository repo = AppealStructureRepository.CreateDefault(_serviceProvider);

            AppealStructure appealStructure = await repo.GetById(guildId, id);

            appealStructure.SortOrder = dto.SortOrder;
            appealStructure.Question = dto.Question.Trim();
            appealStructure.Deleted = false;

            appealStructure = await AppealStructureRepository.CreateDefault(_serviceProvider).Update(appealStructure);

            return Ok(new AppealStructureView(appealStructure));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAppealStructure([FromRoute] ulong guildId, [FromRoute] int id)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            await GetRegisteredGuild(guildId);

            await AppealStructureRepository.CreateDefault(_serviceProvider).DeleteById(guildId, id);

            return Ok();
        }
    }
}

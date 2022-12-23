using MASZ.Enums;
using MASZ.Repositories;
using MASZ.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MASZ.Models.Views;

namespace MASZ.Controllers.api.v1
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/evidence")]
    [Authorize]
    public class VerifiedEvidenceController : SimpleController
    {
        public VerifiedEvidenceController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvidence([FromRoute] ulong guildId, [FromQuery] int startPage = 0)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            List<VerifiedEvidenceView> evidence = (await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetEvidencePagination(guildId, startPage)).Select(x => new VerifiedEvidenceView(x)).ToList();
            if(evidence.Count == 0)
            {
                return NotFound();
            }
            return Ok(evidence);
        }

        [HttpGet("{evidenceId}")]
        public async Task<IActionResult> GetEvidence([FromRoute] ulong guildId, [FromRoute] int evidenceId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            VerifiedEvidenceView evidence = new(await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetEvidence(guildId, evidenceId));
            if (evidence == default)
            {
                return NotFound();
            }
            return Ok(evidence);
        }

        [HttpDelete("{evidenceId}")]
        public async Task<IActionResult> DeleteEvidence([FromRoute] ulong guildId, [FromRoute] int evidenceId)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            VerifiedEvidence deleted = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).DeleteEvidence(guildId, evidenceId);
            if(deleted == default) 
            {
                return NotFound();
            }
            return Ok(deleted);
        }
    }
}

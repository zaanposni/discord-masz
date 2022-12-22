using MASZ.Enums;
using MASZ.Repositories;
using MASZ.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet()]
        public async Task<IActionResult> GetEvidence([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            List<VerifiedEvidence> evidence = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider).GetAllEvidence(guildId);
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
            VerifiedEvidence evidence = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider).GetEvidence(guildId, evidenceId);
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
            VerifiedEvidence deleted = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider).DeleteEvidence(guildId, evidenceId);
            if(deleted == default) 
            {
                return NotFound();
            }
            return Ok(deleted);
        }
    }
}

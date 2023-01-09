using MASZ.Enums;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers.api.v1
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/evidencemapping")]
    [Authorize]
    public class ModCaseEvidenceMappingController : SimpleController
    {
        public ModCaseEvidenceMappingController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("{evidenceId}/{caseId}")]
        public async Task<IActionResult> LinkEvidence([FromRoute] ulong guildId, [FromRoute] int evidenceId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).Link(guildId, evidenceId, caseId);

            return Ok();
        }

        [HttpDelete("{evidenceId}/{caseId}")]
        public async Task<IActionResult> UnlinkEvidence([FromRoute] ulong guildId, [FromRoute] int evidenceId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).Unlink(guildId, evidenceId, caseId);

            return Ok();
        }
    }
}

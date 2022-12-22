using Discord;
using MASZ.Enums;
using MASZ.Models.Database;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers.api.v1.Views
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/evidence/{evidenceId}/view")]
    [Authorize]
    public class VerifiedEvidenceViewController : SimpleController
    {
        public VerifiedEvidenceViewController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IActionResult> GetEvidenceView([FromRoute] ulong guildId, [FromRoute] int evidenceId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            VerifiedEvidence evidence = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider).GetEvidence(guildId, evidenceId);

            IUser reporter = await _discordAPI.FetchUserInfo(evidence.ReporterUserId, CacheBehavior.OnlyCache);
            IUser reported = await _discordAPI.FetchUserInfo(evidence.ReportedUserId, CacheBehavior.OnlyCache);

            VerifiedEvidenceExpandedView view = new(evidence, reporter, reported);

            // TODO: mappings

            return Ok(view);
        }
    }
}

using Discord;
using MASZ.Enums;
using MASZ.Models;
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
    public class VerifiedEvidenceViewController : SimpleEvidenceController
    {
        public VerifiedEvidenceViewController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IActionResult> GetEvidenceView([FromRoute] ulong guildId, [FromRoute] int evidenceId)
        {
            await RequirePermission(guildId, evidenceId, APIActionPermission.View);
            Identity currentIdentity = await GetIdentity();
            bool isMod = await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);

            VerifiedEvidence evidence = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, currentIdentity).GetEvidence(guildId, evidenceId);

            IUser reported = await _discordAPI.FetchUserInfo(evidence.UserId, CacheBehavior.OnlyCache);
            IUser moderator = await _discordAPI.FetchUserInfo(evidence.ModId, CacheBehavior.OnlyCache);

            VerifiedEvidenceExpandedView view = new(evidence, reported, moderator);

            if(!(await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId)).PublishModeratorInfo)
            {
                if(!isMod)
                {
                    view.RemoveModeratorInfo();
                }
            }

            if(evidence.EvidenceMappings != null)
            {
                if (isMod)
                {
                    view.LinkedCases = evidence.EvidenceMappings.Select(mapping => new CaseView(mapping.ModCase)).ToList();
                }
                else
                {
                    view.LinkedCases = evidence.EvidenceMappings.Where(x => x.ModCase.UserId == reported.Id).Select(x => new CaseView(x.ModCase)).ToList();
                }
            }
            return Ok(view);
        }
    }
}

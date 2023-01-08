using MASZ.Enums;
using MASZ.Repositories;
using MASZ.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MASZ.Models.Views;
using MASZ.Dtos.VerifiedEvidence;
using MASZ.Models;

namespace MASZ.Controllers.api.v1
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/evidence")]
    [Authorize]
    public class VerifiedEvidenceController : SimpleEvidenceController
    {
        public VerifiedEvidenceController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvidence([FromRoute] ulong guildId, [FromQuery] int startPage = 0)
        {
            Identity currentIdentity = await GetIdentity();
            ulong currentUser = 0;
            if(!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                currentUser = currentIdentity.GetCurrentUser().Id;
            }

            List<VerifiedEvidenceView> evidence = new();

            if(currentUser == 0) 
            {
                evidence = (await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetEvidencePagination(guildId, startPage)).Select(x => new VerifiedEvidenceView(x)).ToList();
            } else
            {
                evidence = (await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, currentIdentity).GetEvidencePaginationForUser(guildId, currentUser, startPage)).Select(x => new VerifiedEvidenceView(x)).ToList();
            }

            if (!(await GetRegisteredGuild(guildId)).PublishModeratorInfo)
            {
                if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
                {
                    foreach (var e in evidence)
                    {
                        e.RemoveModeratorInfo();
                    }
                }
            }

            return Ok(evidence);
        }

        [HttpGet("{evidenceId}")]
        public async Task<IActionResult> GetEvidence([FromRoute] ulong guildId, [FromRoute] int evidenceId)
        {
            await RequirePermission(guildId, evidenceId, APIActionPermission.View);
            Identity currentIdentity = await GetIdentity();
            VerifiedEvidenceView evidence = new(await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, currentIdentity).GetEvidence(guildId, evidenceId));

            if (!(await GetRegisteredGuild(guildId)).PublishModeratorInfo)
            {
                if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
                {
                    evidence.RemoveModeratorInfo();
                }
            }
            return Ok(evidence);
        }

        [HttpDelete("{evidenceId}")]
        public async Task<IActionResult> DeleteEvidence([FromRoute] ulong guildId, [FromRoute] int evidenceId)
        {
            await RequirePermission(guildId, evidenceId, APIActionPermission.Delete);
            VerifiedEvidence deleted = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).DeleteEvidence(guildId, evidenceId);
            return Ok(deleted);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateEvidence([FromRoute] ulong guildId, [FromBody] CreateVerifiedEvidenceDto body)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            var currentUser = await GetCurrentUser();

            var channelId = body.ChannelId;
            var messageId = body.MessageId;

            var message = await _discordAPI.GetIMessage(channelId, messageId, CacheBehavior.IgnoreCache);

            if(message == null) 
            {
                return NotFound();
            }

            var nickname = (await _discordAPI.FetchMemberInfo(guildId, message.Author.Id, CacheBehavior.OnlyCache))?.Nickname;

            VerifiedEvidence evidence = new()
            {
                GuildId = guildId,
                ChannelId = channelId,
                MessageId = messageId,
                ModId = currentUser.Id,
                ReportedAt= DateTime.UtcNow,
                SentAt = message.Timestamp.DateTime,
                ReportedContent = message.Content,
                UserId = message.Author.Id,
                Username = message.Author.Username,
                Discriminator = message.Author.Discriminator,
                Nickname = nickname,
            };

            var created = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).CreateEvidence(evidence);

            return Ok(created);
        }
    }
}

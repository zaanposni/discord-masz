using MASZ.Enums;
using MASZ.Repositories;
using MASZ.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MASZ.Models.Views;
using MASZ.Dtos.VerifiedEvidence;
using MASZ.Models;
using Discord;

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
                evidence = (await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).GetEvidencePagination(guildId, startPage)).Select(x => new VerifiedEvidenceView(x)).ToList();
            } else
            {
                evidence = (await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).GetEvidencePaginationForUser(guildId, currentUser, startPage)).Select(x => new VerifiedEvidenceView(x)).ToList();
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
            VerifiedEvidenceView evidence = new(await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).GetEvidence(guildId, evidenceId));

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
            VerifiedEvidence deleted = await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).DeleteEvidence(guildId, evidenceId);
            return Ok(deleted);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateEvidence([FromRoute] ulong guildId, [FromBody] CreateVerifiedEvidenceDto body)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            IUser currentUser = await GetCurrentUser();

            ulong channelId = body.ChannelId;
            ulong messageId = body.MessageId;

            IMessage message = await _discordAPI.GetIMessage(guildId, channelId, messageId, CacheBehavior.IgnoreCache);

            if(message == null)
            {
                return NotFound();
            }

            string nickname = (await _discordAPI.FetchMemberInfo(guildId, message.Author.Id, CacheBehavior.OnlyCache))?.Nickname;

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

            VerifiedEvidence created = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, await GetIdentity()).CreateEvidence(evidence);

            return Ok(created);
        }
    }
}

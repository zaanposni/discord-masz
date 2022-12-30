using MASZ.Enums;
using MASZ.Repositories;
using MASZ.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MASZ.Models.Views;
using MASZ.Dtos.VerifiedEvidence;

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

        [HttpPost()]
        public async Task<IActionResult> CreateEvidence([FromRoute] ulong guildId, [FromBody] CreateVerifiedEvidenceDto body)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            var channelId = ulong.Parse(body.ChannelId);
            var messageId = ulong.Parse(body.MessageId);
            var modId = ulong.Parse(body.ModId);

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
                ModId = modId,
                ReportedAt= DateTime.UtcNow,
                SentAt = message.Timestamp.DateTime,
                ReportedContent = message.CleanContent,
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

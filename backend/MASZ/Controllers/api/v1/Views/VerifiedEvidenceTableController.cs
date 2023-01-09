using System.ComponentModel.DataAnnotations;
using MASZ.Dtos.VerifiedEvidence;
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
    [Route("api/v1/guilds/{guildId}/evidence")]
    [Authorize]
    public class VerifiedEvidenceTableController : SimpleEvidenceController
    {
        public VerifiedEvidenceTableController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("evidencetable")]
        public async Task<IActionResult> GetAllEvidence(
            [FromRoute] ulong guildId,
            [FromQuery][Range(0, int.MaxValue)] int startPage = 0,
            [FromBody] VerifiedEvidenceTableFilterDto search = null)
        {
            return Ok(await GenerateTable(guildId, startPage, search));
        }

        private async Task<VerifiedEvidenceTable> GenerateTable(ulong guildId, int startPage, VerifiedEvidenceTableFilterDto search)
        {
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);

            ulong userOnly = 0;
            if (!await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = identity.GetCurrentUser().Id;
            }

            List<VerifiedEvidence> evidence = new();

            if (userOnly != 0)
            {
                evidence = await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).GetEvidencePaginationForUser(guildId, userOnly, startPage);
            } else
            {
                evidence = await VerifiedEvidenceRepository.CreateWithBotIdentity(_serviceProvider).GetEvidencePagination(guildId, startPage);
            }

            List<VerifiedEvidenceTableEntry> tmp = new();
            foreach(var e in evidence)
            {
                var entry = new VerifiedEvidenceTableEntry(
                    e,
                    await _discordAPI.FetchUserInfo(e.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(e.ModId, CacheBehavior.OnlyCache)
                );
                tmp.Add(entry);
            };

            IEnumerable<VerifiedEvidenceTableEntry> table = tmp.AsEnumerable();

            if(!string.IsNullOrEmpty(search?.CustomTextFilter))
            {
                table = table.Where(t =>
                    Contains(t.VerifiedEvidence.ReportedContent, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.UserId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.Username + "#" + t.VerifiedEvidence.Discriminator, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.Nickname, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ModId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ChannelId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.MessageId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.SentAt, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReportedAt, search.CustomTextFilter) ||
                    Contains("#" + t.VerifiedEvidence.Id.ToString(), search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.Id.ToString(), search.CustomTextFilter) ||
                    Contains(t.Reported, search.CustomTextFilter) ||
                    Contains(t.Moderator, search.CustomTextFilter)
                );
            }

            if(search?.ModIds != null && search.ModIds.Count > 0)
            {
                table = table.Where(x => search.ModIds.Contains(x.VerifiedEvidence.ModId));
            }

            if (search?.ReportedIds != null && search.ReportedIds.Count > 0)
            {
                table = table.Where(x => search.ReportedIds.Contains(x.VerifiedEvidence.UserId));
            }

            if(!guildConfig.PublishModeratorInfo && userOnly != 0)
            {
                foreach(var e in table)
                {
                    e.RemoveModeratorInfo();
                }
            }

            return new VerifiedEvidenceTable(table.ToList(), table.Count());
        }

        private static bool Contains(string obj, string search)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return false;
            }
            return obj.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }
        private static bool Contains(DateTime obj, string search)
        {
            if (obj == default)
            {
                return false;
            }
            return obj.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }
        private static bool Contains(DiscordUserView obj, string search)
        {
            if (obj == null)
            {
                return false;
            }
            return Contains(obj.Username, search) || Contains(obj.Discriminator, search);
        }
    }
}

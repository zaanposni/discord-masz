using System.ComponentModel.DataAnnotations;
using MASZ.Dtos.VerifiedEvidence;
using MASZ.Enums;
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
    public class VerifiedEvidenceTableController : SimpleController
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
            await RequirePermission(guildId, Enums.DiscordPermission.Moderator);

            List<VerifiedEvidence> evidence = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider).GetAllEvidence(guildId);

            List<VerifiedEvidenceTableEntry> tmp = new();
            foreach(var e in evidence) 
            {
                var entry = new VerifiedEvidenceTableEntry(
                    e,
                    await _discordAPI.FetchUserInfo(e.ReporterUserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(e.ReportedUserId, CacheBehavior.OnlyCache)
                );
                tmp.Add(entry);
            };

            IEnumerable<VerifiedEvidenceTableEntry> table = tmp.AsEnumerable();

            if(!string.IsNullOrEmpty(search?.CustomTextFilter))
            {
                table = table.Where(t =>
                    Contains(t.VerifiedEvidence.ReportedContent, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReporterUserId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReporterUsername, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReporterNickname, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReportedUserId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReportedUsername, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReportedNickname, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.MessageId, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.SentAt, search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.ReportedAt, search.CustomTextFilter) ||
                    Contains("#" + t.VerifiedEvidence.Id.ToString(), search.CustomTextFilter) ||
                    Contains(t.VerifiedEvidence.Id.ToString(), search.CustomTextFilter) ||
                    Contains(t.Reporter, search.CustomTextFilter) ||
                    Contains(t.Reported, search.CustomTextFilter)
                );
            }

            if(search?.ReporterIds != null && search.ReporterIds.Count > 0)
            {
                table = table.Where(x => search.ReporterIds.Contains(x.VerifiedEvidence.ReporterUserId));
            }

            if (search?.ReportedIds != null && search.ReportedIds.Count > 0)
            {
                table = table.Where(x => search.ReportedIds.Contains(x.VerifiedEvidence.ReportedUserId));
            }

            return new VerifiedEvidenceTable(table.Skip(startPage * 20).Take(20).ToList(), table.Count());
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

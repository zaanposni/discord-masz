using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/casemapping")]
    [Authorize]
    public class ModCaseMappingController : SimpleCaseController
    {

        public ModCaseMappingController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("{caseA}/{caseB}")]
        public async Task<IActionResult> LinkCase([FromRoute] ulong guildId, [FromRoute] int caseA, [FromRoute] int caseB)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            await ModCaseRepository.CreateDefault(_serviceProvider, await GetIdentity()).LinkCases(guildId, caseA, caseB);

            return Ok();
        }

        [HttpDelete("{caseA}/{caseB}")]
        public async Task<IActionResult> UnlinkCase([FromRoute] ulong guildId, [FromRoute] int caseA, [FromRoute] int caseB)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            await ModCaseRepository.CreateDefault(_serviceProvider, await GetIdentity()).UnlinkCases(guildId, caseA, caseB);

            return Ok();
        }
    }
}
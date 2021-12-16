using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/bin")]
    [Authorize]
    public class ModCaseBinController : SimpleCaseController
    {

        public ModCaseBinController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpDelete("{caseId}/restore")]
        public async Task<IActionResult> RestoreModCase([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, await GetIdentity()).RestoreCase(guildId, caseId);

            return Ok(modCase);
        }

        [HttpDelete("{caseId}/delete")]
        public async Task<IActionResult> DeleteModCase([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            Identity currentIdentity = await GetIdentity();
            await RequireSiteAdmin();

            await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).DeleteModCase(guildId, caseId, true, true, false);

            return Ok();
        }
    }
}
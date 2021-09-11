using System;
using System.Threading.Tasks;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/bin")]
    [Authorize]
    public class ModCaseBinController : SimpleCaseController
    {
        private readonly ILogger<ModCaseBinController> _logger;

        public ModCaseBinController(ILogger<ModCaseBinController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            _logger = logger;
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
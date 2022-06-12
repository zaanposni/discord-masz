using MASZ.Dtos;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/zalgo")]
    [Authorize]
    public class ZalgoController : SimpleController
    {

        public ZalgoController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetZalgo([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            ZalgoConfig zalgo = await ZalgoRepository.CreateDefault(_serviceProvider, identity).GetZalgo(guildId);

            return Ok(new ZalgoConfigView(zalgo));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateZalgo([FromRoute] ulong guildId, [FromBody] ZalgoConfigDto zalgo)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            Identity currentIdentity = await GetIdentity();

            ZalgoConfig config = new ZalgoConfig
            {
                Enabled = zalgo.Enabled,
                Percentage = zalgo.Percentage,
                renameNormal = zalgo.renameNormal,
                renameFallback = zalgo.renameFallback,
                logToModChannel = zalgo.logToModChannel
            };

            return Ok(await ZalgoRepository.CreateDefault(_serviceProvider, currentIdentity).CreateOrUpdateZalgo(guildId, config));
        }

        [HttpPost]
        public async Task<IActionResult> SimulateZalgo([FromRoute] ulong guildId, [FromBody] ZalgoConfigDto zalgo)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            Identity currentIdentity = await GetIdentity();

            ZalgoConfig config = new ZalgoConfig
            {
                Enabled = zalgo.Enabled,
                Percentage = zalgo.Percentage,
                renameNormal = zalgo.renameNormal,
                renameFallback = zalgo.renameFallback,
                logToModChannel = zalgo.logToModChannel
            };

            ZalgoRepository repository = ZalgoRepository.CreateDefault(_serviceProvider, currentIdentity);

            return Ok(await repository.CheckZalgoForAllMembers(guildId, config));
        }
    }
}
using MASZ.Dtos.AutoModerationConfig;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/automoderationconfig/")]
    [Authorize]
    public class AutoModerationConfigController : SimpleController
    {
        public AutoModerationConfigController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPut]
        public async Task<IActionResult> SetItem([FromRoute] ulong guildId, [FromBody] AutoModerationConfigForPutDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new AutoModerationConfigView(await AutoModerationConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .UpdateConfig(new AutoModerationConfig(dto, guildId))));
        }

        [HttpDelete("{type}")]
        public async Task<IActionResult> DeleteItem([FromRoute] ulong guildId, [FromRoute] AutoModerationType type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new AutoModerationConfigView(await AutoModerationConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .DeleteConfigForGuild(guildId, type)));
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetItem([FromRoute] ulong guildId, [FromRoute] AutoModerationType type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            return Ok(new AutoModerationConfigView(await AutoModerationConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .GetConfigsByGuildAndType(guildId, type)));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok((await AutoModerationConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .GetConfigsByGuild(guildId)).Select(x => new AutoModerationConfigView(x)));
        }
    }
}
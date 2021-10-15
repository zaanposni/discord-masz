using System;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.AutoModerationConfig;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using masz.Enums;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/automoderationconfig/")]
    [Authorize]
    public class AutoModerationConfigController : SimpleController
    {
        private readonly ILogger<AutoModerationConfigController> _logger;

        public AutoModerationConfigController(IServiceProvider serviceProvider, ILogger<AutoModerationConfigController> logger) : base(serviceProvider) {
            _logger = logger;
        }

        [HttpPut]
        public async Task<IActionResult> SetItem([FromRoute] ulong guildId, [FromBody] AutoModerationConfigForPutDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new AutoModerationConfigView(await AutoModerationConfigRepository.CreateDefault(_serviceProvider).UpdateConfig(new AutoModerationConfig(dto, guildId))));
        }

        [HttpDelete("{type}")]
        public async Task<IActionResult> DeleteItem([FromRoute] ulong guildId, [FromRoute] AutoModerationType type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new AutoModerationConfigView(await AutoModerationConfigRepository.CreateDefault(_serviceProvider).DeleteConfigForGuild(guildId, type)));
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetItem([FromRoute] ulong guildId, [FromRoute] AutoModerationType type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            return Ok(new AutoModerationConfigView(await AutoModerationConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuildAndType(guildId, type)));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok((await AutoModerationConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuild(guildId)).Select(x => new AutoModerationConfigView(x)));
        }
    }
}
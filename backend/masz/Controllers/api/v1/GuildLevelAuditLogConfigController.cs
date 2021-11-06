using System;
using System.Linq;
using System.Threading.Tasks;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using masz.Enums;
using masz.Dtos.GuildLevelAuditLogConfig;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/auditlog/")]
    [Authorize]
    public class GuildLevelAuditLogConfigController : SimpleController
    {
        private readonly ILogger<GuildLevelAuditLogConfigController> _logger;

        public GuildLevelAuditLogConfigController(IServiceProvider serviceProvider, ILogger<GuildLevelAuditLogConfigController> logger) : base(serviceProvider) {
            _logger = logger;
        }

        [HttpPut]
        public async Task<IActionResult> SetItem([FromRoute] ulong guildId, [FromBody] GuildLevelAuditLogConfigForPutDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new GuildLevelAuditLogConfigView(await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider).UpdateConfig(new GuildLevelAuditLogConfig(dto, guildId))));
        }

        [HttpDelete("{type}")]
        public async Task<IActionResult> DeleteItem([FromRoute] ulong guildId, [FromRoute] GuildAuditLogEvent type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new GuildLevelAuditLogConfigView(await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider).DeleteConfigForGuild(guildId, type)));
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetItem([FromRoute] ulong guildId, [FromRoute] GuildAuditLogEvent type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            return Ok(new GuildLevelAuditLogConfigView(await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuildAndType(guildId, type)));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok((await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuild(guildId)).Select(x => new GuildLevelAuditLogConfigView(x)));
        }
    }
}
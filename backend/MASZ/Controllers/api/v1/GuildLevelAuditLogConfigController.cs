using MASZ.Dtos.GuildLevelAuditLogConfig;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/auditlog/")]
    [Authorize]
    public class GuildLevelAuditLogConfigController : SimpleController
    {
        public GuildLevelAuditLogConfigController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPut]
        public async Task<IActionResult> SetItem([FromRoute] ulong guildId, [FromBody] GuildLevelAuditLogConfigForPutDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new GuildLevelAuditLogConfigView(
                await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                    .UpdateConfig(new GuildLevelAuditLogConfig(dto, guildId))));
        }

        [HttpDelete("{type}")]
        public async Task<IActionResult> DeleteItem([FromRoute] ulong guildId, [FromRoute] GuildAuditLogEvent type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok(new GuildLevelAuditLogConfigView(await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .DeleteConfigForGuild(guildId, type)));
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetItem([FromRoute] ulong guildId, [FromRoute] GuildAuditLogEvent type)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);
            return Ok(new GuildLevelAuditLogConfigView(await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .GetConfigsByGuildAndType(guildId, type)));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            return Ok((await GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider, (await GetIdentity()).GetCurrentUser())
                .GetConfigsByGuild(guildId)).Select(x => new GuildLevelAuditLogConfigView(x)));
        }
    }
}
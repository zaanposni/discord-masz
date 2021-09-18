using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Dtos.GuildMotd;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/motd")]
    [Authorize]
    public class GuildMotdController : SimpleController
    {
        private readonly ILogger<GuildMotdController> _logger;

        public GuildMotdController(ILogger<GuildMotdController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMotd([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            GuildMotd motd = await GuildMotdRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetMotd(guildId);

            DiscordUser creator = await _discordAPI.FetchUserInfo(motd.UserId, CacheBehavior.Default);

            return Ok(new GuildMotdExpandedView(motd, creator));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMotd([FromRoute] ulong guildId, [FromBody] MotdForCreateDto motd)
        {
            await RequirePermission(guildId, DiscordPermission.Admin);

            Identity currentIdentity = await GetIdentity();

            return Ok(await GuildMotdRepository.CreateDefault(_serviceProvider, currentIdentity).CreateOrUpdateMotd(guildId, motd.Message, motd.ShowMotd));
        }
    }
}
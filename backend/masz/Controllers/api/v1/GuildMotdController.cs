using Discord;
using MASZ.Dtos.GuildMotd;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/motd")]
    [Authorize]
    public class GuildMotdController : SimpleController
    {

        public GuildMotdController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetMotd([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            GuildMotd motd = await GuildMotdRepository.CreateDefault(_serviceProvider, identity).GetMotd(guildId);

            IUser creator = await _discordAPI.FetchUserInfo(motd.UserId, CacheBehavior.Default);

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
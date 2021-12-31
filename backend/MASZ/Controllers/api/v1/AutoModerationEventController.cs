using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/automoderations")]
    [Authorize]
    public class AutoModerationEventController : SimpleController
    {

        public AutoModerationEventController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0)
        {
            Identity currentIdentity = await GetIdentity();
            await GetRegisteredGuild(guildId);

            ulong userOnly = 0;
            if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = currentIdentity.GetCurrentUser().Id;
            }

            AutoModerationEventRepository repo = AutoModerationEventRepository.CreateDefault(_serviceProvider);

            List<AutoModerationEvent> events = null;
            int eventsCount = 0;
            if (userOnly == 0)
            {
                events = await repo.GetPagination(guildId, startPage);
                eventsCount = await repo.CountEventsByGuild(guildId);
            }
            else
            {
                events = await repo.GetPaginationFilteredForUser(guildId, userOnly, startPage);
                eventsCount = await repo.CountEventsByGuildAndUser(guildId, userOnly);
            }

            return Ok(new
            {
                events = events.Select(x => new AutoModerationEventView(x)),
                count = eventsCount
            });
        }
    }
}
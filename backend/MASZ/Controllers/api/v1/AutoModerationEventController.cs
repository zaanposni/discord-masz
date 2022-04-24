using MASZ.Dtos.AutoModerationEvent;
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

        [HttpPost]
        public async Task<IActionResult> GetAllItems([FromRoute] ulong guildId, [FromBody] AutomoderationEventFilterDto filter = null, [FromQuery][Range(0, int.MaxValue)] int startPage = 0)
        {
            Identity currentIdentity = await GetIdentity();
            await GetRegisteredGuild(guildId);

            ulong userOnly = 0;
            if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = currentIdentity.GetCurrentUser().Id;
            }

            AutoModerationEventRepository repo = AutoModerationEventRepository.CreateDefault(_serviceProvider);

            IEnumerable<AutoModerationEvent> events = null;
            if (userOnly == 0)
            {
                events = await repo.GetAllEventsForGuild(guildId);
            }
            else
            {
                events = await repo.GetAllEventsForUserAndGuild(guildId, userOnly);
            }

            // WHERE
            if (filter?.UserIds != null && filter.UserIds.Count > 0)
            {
                events = events.Where(x => filter.UserIds.Contains(x.UserId.ToString()));
            }
            if (filter?.types != null && filter.types.Count > 0)
            {
                events = events.Where(x => filter.types.Contains(x.AutoModerationType));
            }
            if (filter?.actions != null && filter.actions.Count > 0)
            {
                events = events.Where(x => filter.actions.Contains(x.AutoModerationAction));
            }

            return Ok(new
            {
                events = events.Skip(startPage*20).Take(20).Select(x => new AutoModerationEventView(x)),
                count = events.Count()
            });
        }
    }
}
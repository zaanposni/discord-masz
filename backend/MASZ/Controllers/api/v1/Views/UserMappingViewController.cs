using System.ComponentModel.DataAnnotations;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usermapview")]
    [Authorize]
    public class UserMappingViewController : SimpleController
    {

        public UserMappingViewController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<UserMapping> userMappings = await UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetUserMapsByGuild(guildId);

            int fullSize = userMappings.Count;

            List<UserMappingExpandedView> userMappingViews = new();
            foreach (UserMapping userMapping in userMappings.Skip(startPage * 20).Take(20))
            {
                userMappingViews.Add(new UserMappingExpandedView(
                    userMapping,
                    await _discordAPI.FetchUserInfo(userMapping.UserA, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userMapping.UserB, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.OnlyCache)
                ));
            }

            return Ok(new
            {
                items = userMappingViews,
                fullSize
            });
        }
    }
}
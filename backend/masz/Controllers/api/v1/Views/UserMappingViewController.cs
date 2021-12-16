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
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            UserMapRepository repository = UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity());
            List<UserMapping> userMappings = await repository.GetUserMapsByGuild(guildId);
            List<UserMappingExpandedView> userMappingViews = new();
            foreach (UserMapping userMapping in userMappings)
            {
                userMappingViews.Add(new UserMappingExpandedView(
                    userMapping,
                    await _discordAPI.FetchUserInfo(userMapping.UserA, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userMapping.UserB, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.OnlyCache)
                ));
            }

            return Ok(userMappingViews);
        }
    }
}
using MASZ.Dtos.UserMapping;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usermap")]
    [Authorize]
    public class UserMappingController : SimpleController
    {

        public UserMappingController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserMap([FromRoute] ulong guildId, [FromBody] UserMappingForUpdateDto userMapDto)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            UserMapping result = await UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity())
                                                        .CreateOrUpdateUserMap(guildId, userMapDto.UserA, userMapDto.UserB, userMapDto.Reason);

            return Ok(new UserMappingExpandedView(
                result,
                await _discordAPI.FetchUserInfo(result.UserA, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(result.UserB, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(result.CreatorUserId, CacheBehavior.OnlyCache)));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserMap([FromRoute] ulong guildId, [FromRoute] int id)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            var repo = UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity());

            UserMapping userMap = await repo.GetUserMap(id);
            if (userMap.GuildId != guildId)
            {
                throw new ResourceNotFoundException();
            }

            await UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity()).DeleteUserMap(id);

            return Ok();
        }
    }
}
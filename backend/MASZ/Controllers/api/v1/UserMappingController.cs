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

        [HttpPost]
        public async Task<IActionResult> CreateUserMap([FromRoute] ulong guildId, [FromBody] UserMappingForCreateDto userMapDto)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            var repo = UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity());
            try
            {
                await repo.GetUserMap(guildId, userMapDto.UserA, userMapDto.UserB);
                throw new ResourceAlreadyExists();
            }
            catch (ResourceNotFoundException) { }

            UserMapping userMap = await repo.CreateOrUpdateUserMap(guildId, userMapDto.UserA, userMapDto.UserB, userMapDto.Reason);

            return StatusCode(201, new UserMappingView(userMap));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserMap([FromRoute] ulong guildId, [FromRoute] int id, [FromBody] UserMappingForUpdateDto userMapDto)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            var repo = UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity());

            UserMapping userMap = await repo.GetUserMap(id);
            if (userMap.GuildId != guildId)
            {
                throw new ResourceNotFoundException();
            }

            UserMapping result = await repo.CreateOrUpdateUserMap(guildId, userMap.UserA, userMap.UserB, userMapDto.Reason);

            return Ok(new UserMappingView(result));
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
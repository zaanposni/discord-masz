using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.UserMapping;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usermap")]
    [Authorize]
    public class UserMappingController : SimpleController
    {
        private readonly ILogger<UserMappingController> _logger;

        public UserMappingController(ILogger<UserMappingController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserMap([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<UserMapping> userMappings = await UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetUserMapsByGuild(guildId);
            return Ok(userMappings.Select(x => new UserMappingView(x)));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserMaps([FromRoute] ulong guildId, [FromRoute] ulong userId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<UserMapping> userMappings = await UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetUserMapsByGuildAndUser(guildId, userId);
            return Ok(userMappings.Select(x => new UserMappingView(x)));
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
            } catch (ResourceNotFoundException) { }

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
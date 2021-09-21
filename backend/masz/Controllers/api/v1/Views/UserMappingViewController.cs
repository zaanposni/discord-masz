using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using masz.Enums;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usermapview")]
    [Authorize]
    public class UserMappingViewController : SimpleController
    {
        private readonly ILogger<UserMappingViewController> _logger;

        public UserMappingViewController(ILogger<UserMappingViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            UserMapRepository repository = UserMapRepository.CreateDefault(_serviceProvider, await GetIdentity());
            List<UserMapping> userMappings = await repository.GetUserMapsByGuild(guildId);
            List<UserMappingExpandedView> userMappingViews = new List<UserMappingExpandedView>();
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
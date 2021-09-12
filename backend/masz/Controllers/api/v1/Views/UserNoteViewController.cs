using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usernoteview")]
    [Authorize]
    public class UserNoteViewController : SimpleController
    {
        private readonly ILogger<UserNoteViewController> _logger;

        public UserNoteViewController(ILogger<UserNoteViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<UserNote> userNotes = await UserNoteRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetUserNotesByGuild(guildId);
            List<UserNoteExpandedView> userNoteViews = new List<UserNoteExpandedView>();
            foreach (UserNote userNote in userNotes)
            {
                userNoteViews.Add(new UserNoteExpandedView(
                    userNote,
                    await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userNote.CreatorId, CacheBehavior.OnlyCache)));
            }

            return Ok(userNoteViews);
        }
    }
}
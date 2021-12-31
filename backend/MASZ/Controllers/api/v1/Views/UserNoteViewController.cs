using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usernoteview")]
    [Authorize]
    public class UserNoteViewController : SimpleController
    {
        public UserNoteViewController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<UserNote> userNotes = await UserNoteRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetUserNotesByGuild(guildId);
            List<UserNoteExpandedView> userNoteViews = new();
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
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int startPage = 0)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            List<UserNote> userNotes = await UserNoteRepository.CreateDefault(_serviceProvider, await GetIdentity()).GetUserNotesByGuild(guildId);

            int fullSize = userNotes.Count;

            List<UserNoteExpandedView> userNoteViews = new();
            foreach (UserNote userNote in userNotes.Skip(startPage * 20).Take(20))
            {
                userNoteViews.Add(new UserNoteExpandedView(
                    userNote,
                    await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(userNote.CreatorId, CacheBehavior.OnlyCache)));
            }

            return Ok(new
            {
                items = userNoteViews,
                fullSize
            });
        }
    }
}
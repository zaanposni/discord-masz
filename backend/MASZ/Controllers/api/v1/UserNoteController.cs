using MASZ.Dtos.UserNote;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/usernote")]
    [Authorize]
    public class UserNoteController : SimpleController
    {

        public UserNoteController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPut]
        public async Task<IActionResult> CreateUserNote([FromRoute] ulong guildId, [FromBody] UserNoteForUpdateDto userNote)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            UserNote createdUserNote = await UserNoteRepository.CreateDefault(_serviceProvider, await GetIdentity()).CreateOrUpdateUserNote(guildId, userNote.UserId, userNote.Description);

            return StatusCode(201, new UserNoteExpandedView(
                createdUserNote,
                await _discordAPI.FetchUserInfo(createdUserNote.UserId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(createdUserNote.CreatorId, CacheBehavior.OnlyCache)));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserNote([FromRoute] ulong guildId, [FromRoute] ulong userId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            await UserNoteRepository.CreateDefault(_serviceProvider, await GetIdentity()).DeleteUserNote(guildId, userId);

            return Ok();
        }
    }
}
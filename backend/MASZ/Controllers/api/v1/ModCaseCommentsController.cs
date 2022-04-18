using Discord;
using MASZ.Dtos.ModCaseComments;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/{caseId}/comments")]
    [Authorize]
    public class ModCaseCommentsController : SimpleCaseController
    {
        public ModCaseCommentsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromBody] ModCaseCommentForCreateDto comment)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            IUser currentUser = currentIdentity.GetCurrentUser();

            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetModCase(guildId, caseId);

            // suspects can only comment if last comment was not by him.
            if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                if (modCase.Comments.Any())
                {
                    if (modCase.Comments.Last().UserId == currentUser.Id)
                    {
                        throw new BaseAPIException("Already commented", APIError.LastCommentAlreadyFromSuspect);
                    }
                }
            }

            ModCaseComment createdComment = await ModCaseCommentRepository.CreateDefault(_serviceProvider, currentIdentity).CreateComment(guildId, caseId, comment.Message.Trim());

            return StatusCode(201, new CommentsView(createdComment));
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromRoute] int commentId, [FromBody] ModCaseCommentForPutDto newValue)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            IUser currentUser = currentIdentity.GetCurrentUser();

            var repo = ModCaseCommentRepository.CreateDefault(_serviceProvider, currentIdentity);

            ModCaseComment comment = await repo.GetSpecificComment(commentId);
            if (comment.UserId != currentUser.Id && !currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException();
            }

            ModCaseComment createdComment = await repo.UpdateComment(guildId, caseId, commentId, newValue.Message.Trim());

            return Ok(new CommentsView(createdComment));
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromRoute] int commentId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            IUser currentUser = currentIdentity.GetCurrentUser();

            var repo = ModCaseCommentRepository.CreateDefault(_serviceProvider, currentIdentity);

            ModCaseComment comment = await repo.GetSpecificComment(commentId);
            if (comment.UserId != currentUser.Id && !currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException();
            }

            await repo.DeleteComment(guildId, caseId, commentId);

            return Ok();
        }
    }
}
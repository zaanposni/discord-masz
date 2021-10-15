using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Dtos.ModCaseComments;
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
    [Route("api/v1/guilds/{guildId}/cases/{caseId}/comments")]
    [Authorize]
    public class ModCaseCommentsController : SimpleCaseController
    {
        private readonly ILogger<ModCaseCommentsController> _logger;
        public ModCaseCommentsController(ILogger<ModCaseCommentsController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromBody] ModCaseCommentForCreateDto comment)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            DiscordUser currentUser = currentIdentity.GetCurrentUser();

            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetModCase(guildId, caseId);

            // suspects can only comment if last comment was not by him.
            if (! await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                if (modCase.Comments.Any())
                {
                    if (modCase.Comments.Last().UserId == currentUser.Id)
                    {
                        throw new BaseAPIException("Already commented", APIError.LastCommentAlreadyFromSuspect);
                    }
                }
            }

            ModCaseComment createdComment = await ModCaseCommentRepository.CreateDefault(_serviceProvider, currentIdentity).CreateComment(guildId, caseId, comment.Message);

            try
            {
                await _discordAnnouncer.AnnounceComment(createdComment, currentUser, RestAction.Created);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to announce comment.");
            }

            return StatusCode(201, new CommentsView(createdComment));
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromRoute] int commentId, [FromBody] ModCaseCommentForPutDto newValue)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            DiscordUser currentUser = currentIdentity.GetCurrentUser();

            var repo = ModCaseCommentRepository.CreateDefault(_serviceProvider, currentIdentity);

            ModCaseComment comment = await repo.GetSpecificComment(commentId);
            if (comment.UserId != currentUser.Id && ! currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException();
            }

            ModCaseComment createdComment = await repo.UpdateComment(guildId, caseId, commentId, newValue.Message);

            try
            {
                await _discordAnnouncer.AnnounceComment(createdComment, currentUser, RestAction.Edited);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to announce comment.");
            }

            return Ok(new CommentsView(createdComment));
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteSpecificItem([FromRoute] ulong guildId, [FromRoute] int caseId, [FromRoute] int commentId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);

            Identity currentIdentity = await GetIdentity();
            DiscordUser currentUser = currentIdentity.GetCurrentUser();

            var repo = ModCaseCommentRepository.CreateDefault(_serviceProvider, currentIdentity);

            ModCaseComment comment = await repo.GetSpecificComment(commentId);
            if (comment.UserId != currentUser.Id && ! currentIdentity.IsSiteAdmin())
            {
                throw new UnauthorizedException();
            }

            ModCaseComment deletedComment = await repo.DeleteComment(guildId, caseId, commentId);

            try
            {
                await _discordAnnouncer.AnnounceComment(deletedComment, currentUser, RestAction.Deleted);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to announce comment.");
            }

            return Ok();
        }
    }
}
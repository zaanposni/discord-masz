using Discord;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class ModCaseCommentRepository : BaseRepository<ModCaseCommentRepository>
    {
        private readonly IUser _currentUser;
        private ModCaseCommentRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private ModCaseCommentRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static ModCaseCommentRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static ModCaseCommentRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<int> CountCommentsByGuild(ulong guildId)
        {
            return await Database.CountCommentsForGuild(guildId);
        }
        public async Task<List<ModCaseComment>> GetLastCommentsByGuild(ulong guildId)
        {
            return await Database.SelectLastModCaseCommentsByGuild(guildId);
        }
        public async Task<ModCaseComment> GetSpecificComment(int commentId)
        {
            ModCaseComment comment = await Database.SelectSpecificModCaseComment(commentId);
            if (comment == null)
            {
                throw new ResourceNotFoundException();
            }
            return comment;
        }
        public async Task<ModCaseComment> CreateComment(ulong guildId, int caseId, string comment)
        {
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            if (!modCase.AllowComments)
            {
                throw new CaseIsLockedException();
            }
            if (modCase.MarkedToDeleteAt.HasValue)
            {
                throw new CaseMarkedToBeDeletedException();
            }

            ModCaseComment newComment = new()
            {
                CreatedAt = DateTime.UtcNow,
                UserId = _currentUser.Id,
                Message = comment,
                ModCase = modCase
            };

            await Database.SaveModCaseComment(newComment);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseCommentCreatedEvent.InvokeAsync(newComment, _currentUser);

            return newComment;
        }

        public async Task<ModCaseComment> UpdateComment(ulong guildId, int caseId, int commentId, string newMessage)
        {
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            if (!modCase.AllowComments)
            {
                throw new CaseIsLockedException();
            }
            if (modCase.MarkedToDeleteAt.HasValue)
            {
                throw new CaseMarkedToBeDeletedException();
            }

            ModCaseComment newComment = modCase.Comments.FirstOrDefault(c => c.Id == commentId);
            if (newComment == null)
            {
                throw new ResourceNotFoundException();
            }

            newComment.Message = newMessage;

            Database.UpdateModCaseComment(newComment);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseCommentUpdatedEvent.InvokeAsync(newComment, _currentUser);

            return newComment;
        }

        public async Task<ModCaseComment> DeleteComment(ulong guildId, int caseId, int commentId)
        {
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            if (!modCase.AllowComments)
            {
                throw new CaseIsLockedException();
            }
            if (modCase.MarkedToDeleteAt.HasValue)
            {
                throw new CaseMarkedToBeDeletedException();
            }

            ModCaseComment deleteComment = modCase.Comments.FirstOrDefault(c => c.Id == commentId);
            if (deleteComment == null)
            {
                throw new ResourceNotFoundException();
            }

            Database.DeleteSpecificModCaseComment(deleteComment);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseCommentDeletedEvent.InvokeAsync(deleteComment, _currentUser);

            return deleteComment;
        }
    }
}
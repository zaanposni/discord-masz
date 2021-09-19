using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Events;
using masz.Exceptions;
using masz.Models;

namespace masz.Repositories
{

    public class ModCaseCommentRepository : BaseRepository<ModCaseCommentRepository>
    {
        private readonly DiscordUser _currentUser;
        private ModCaseCommentRepository(IServiceProvider serviceProvider, DiscordUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private ModCaseCommentRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
        }
        public static ModCaseCommentRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new ModCaseCommentRepository(serviceProvider, identity.GetCurrentUser());
        public static ModCaseCommentRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new ModCaseCommentRepository(serviceProvider);
        public async Task<int> CountCommentsByGuild(ulong guildId)
        {
            return await _database.CountCommentsForGuild(guildId);
        }
        public async Task<List<ModCaseComment>> GetLastCommentsByGuild(ulong guildId)
        {
            return await _database.SelectLastModCaseCommentsByGuild(guildId);
        }
        public async Task<ModCaseComment> GetSpecificComment(int commentId)
        {
            ModCaseComment comment = await _database.SelectSpecificModCaseComment(commentId);
            if (comment == null)
            {
                throw new ResourceNotFoundException();
            }
            return comment;
        }
        public async Task<ModCaseComment> CreateComment(ulong guildId, int caseId, string comment)
        {
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            if (! modCase.AllowComments)
            {
                throw new CaseIsLockedException();
            }
            if (modCase.MarkedToDeleteAt.HasValue)
            {
                throw new CaseMarkedToBeDeletedException();
            }

            ModCaseComment newComment = new ModCaseComment();
            newComment.CreatedAt = DateTime.UtcNow;
            newComment.UserId = _currentUser.Id;
            newComment.Message = comment;
            newComment.ModCase = modCase;

            await _database.SaveModCaseComment(newComment);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseCommentCreated(new ModCaseCommentCreatedEventArgs(newComment));

            return newComment;
        }

        public async Task<ModCaseComment> UpdateComment(ulong guildId, int caseId, int commentId, string newMessage)
        {
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            if (! modCase.AllowComments)
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

            _database.UpdateModCaseComment(newComment);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseCommentUpdated(new ModCaseCommentUpdatedEventArgs(newComment));

            return newComment;
        }

        public async Task<ModCaseComment> DeleteComment(ulong guildId, int caseId, int commentId)
        {
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            if (! modCase.AllowComments)
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

            _database.DeleteSpecificModCaseComment(deleteComment);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseCommentDeleted(new ModCaseCommentDeletedEventArgs(deleteComment));

            return deleteComment;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using masz.Enums;

namespace masz.Repositories
{

    public class UserNoteRepository : BaseRepository<UserNoteRepository>
    {
        private readonly DiscordUser _currentUser;
        private UserNoteRepository(IServiceProvider serviceProvider, DiscordUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private UserNoteRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
        }
        public static UserNoteRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new UserNoteRepository(serviceProvider, identity.GetCurrentUser());
        public static UserNoteRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new UserNoteRepository(serviceProvider);
        public async Task<UserNote> GetUserNote(ulong guildId, ulong userId)
        {
            UserNote userNote = await _database.GetUserNoteByUserIdAndGuildId(userId, guildId);
            if (userNote == null)
            {
                throw new ResourceNotFoundException($"UserNote for guild {guildId} and user {userId} not found.");
            }
            return userNote;
        }
        public async Task<List<UserNote>> GetUserNotesByGuild(ulong guildId)
        {
            return await _database.GetUserNotesByGuildId(guildId);
        }
        public async Task<List<UserNote>> GetUserNotesByUser(ulong userId)
        {
            return await _database.GetUserNotesByUserId(userId);
        }
        public async Task<UserNote> CreateOrUpdateUserNote(ulong guildId, ulong userId, string content)
        {
            DiscordUser validUser = await _discordAPI.FetchUserInfo(userId, CacheBehavior.Default);
            if (validUser == null) {
                throw new InvalidDiscordUserException("User not found", userId);
            }

            UserNote userNote;
            RestAction action = RestAction.Edited;
            try {
                userNote = await GetUserNote(guildId, userId);
            } catch (ResourceNotFoundException) {
                userNote = new UserNote();
                userNote.GuildId = guildId;
                userNote.UserId = userId;
                action = RestAction.Created;
            }
            userNote.UpdatedAt = DateTime.UtcNow;
            userNote.CreatorId = _currentUser.Id;

            userNote.Description = content;

            _database.SaveUserNote(userNote);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeUserNoteUpdated(new UserNoteUpdatedEventArgs(userNote));

            await _discordAnnouncer.AnnounceUserNote(userNote, _currentUser, action);

            return userNote;
        }
        public async Task DeleteUserNote(ulong guildId, ulong userId)
        {
            UserNote userNote = await GetUserNote(guildId, userId);

            _database.DeleteUserNote(userNote);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeUserNoteDeleted(new UserNoteDeletedEventArgs(userNote));

            await _discordAnnouncer.AnnounceUserNote(userNote, _currentUser, RestAction.Deleted);
        }
        public async Task DeleteForGuild(ulong guildId)
        {
            await _database.DeleteUserNoteByGuild(guildId);
            await _database.SaveChangesAsync();
        }
        public async Task<int> CountUserNotesForGuild(ulong guildId)
        {
            return await _database.CountUserNotesForGuild(guildId);
        }
        public async Task<int> CountUserNotes()
        {
            return await _database.CountUserNotes();
        }
    }
}
using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class UserNoteRepository : BaseRepository<UserNoteRepository>
    {
        private readonly IUser _currentUser;
        private UserNoteRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private UserNoteRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static UserNoteRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static UserNoteRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<UserNote> GetUserNote(ulong guildId, ulong userId)
        {
            UserNote userNote = await Database.GetUserNoteByUserIdAndGuildId(userId, guildId);
            if (userNote == null)
            {
                throw new ResourceNotFoundException($"UserNote for guild {guildId} and user {userId} not found.");
            }
            return userNote;
        }
        public async Task<List<UserNote>> GetUserNotesByGuild(ulong guildId)
        {
            return await Database.GetUserNotesByGuildId(guildId);
        }
        public async Task<List<UserNote>> GetUserNotesByUser(ulong userId)
        {
            return await Database.GetUserNotesByUserId(userId);
        }
        public async Task<UserNote> CreateOrUpdateUserNote(ulong guildId, ulong userId, string content)
        {
            IUser validUser = await DiscordAPI.FetchUserInfo(userId, CacheBehavior.Default);
            if (validUser == null)
            {
                throw new InvalidIUserException("User not found", userId);
            }

            UserNote userNote;
            RestAction action = RestAction.Updated;
            try
            {
                userNote = await GetUserNote(guildId, userId);
            }
            catch (ResourceNotFoundException)
            {
                userNote = new UserNote
                {
                    GuildId = guildId,
                    UserId = userId
                };
                action = RestAction.Created;
            }
            userNote.UpdatedAt = DateTime.UtcNow;
            userNote.CreatorId = _currentUser.Id;

            userNote.Description = content;

            Database.SaveUserNote(userNote);
            await Database.SaveChangesAsync();

            if (action == RestAction.Created)
            {
                _eventHandler.OnUserNoteCreatedEvent.InvokeAsync(userNote, _currentUser);
            }
            else
            {
                _eventHandler.OnUserNoteUpdatedEvent.InvokeAsync(userNote, _currentUser);
            }

            return userNote;
        }
        public async Task DeleteUserNote(ulong guildId, ulong userId)
        {
            UserNote userNote = await GetUserNote(guildId, userId);

            Database.DeleteUserNote(userNote);
            await Database.SaveChangesAsync();

            _eventHandler.OnUserNoteDeletedEvent.InvokeAsync(userNote, _currentUser);
        }
        public async Task DeleteForGuild(ulong guildId)
        {
            await Database.DeleteUserNoteByGuild(guildId);
            await Database.SaveChangesAsync();
        }
        public async Task<int> CountUserNotesForGuild(ulong guildId)
        {
            return await Database.CountUserNotesForGuild(guildId);
        }
        public async Task<int> CountUserNotes()
        {
            return await Database.CountUserNotes();
        }
    }
}
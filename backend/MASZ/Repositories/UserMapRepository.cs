using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class UserMapRepository : BaseRepository<UserMapRepository>
    {
        private readonly IUser _currentUser;
        private UserMapRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private UserMapRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static UserMapRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static UserMapRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<UserMapping> GetUserMap(ulong guildId, ulong userA, ulong userB)
        {
            UserMapping userMapping = await Database.GetUserMappingByUserIdsAndGuildId(userA, userB, guildId);
            if (userMapping == null)
            {
                throw new ResourceNotFoundException($"UserMap for guild {guildId} and users {userA}/{userB} not found.");
            }
            return userMapping;
        }
        public async Task<UserMapping> GetUserMap(int id)
        {
            UserMapping userMapping = await Database.GetUserMappingById(id);
            if (userMapping == null)
            {
                throw new ResourceNotFoundException($"UserMap for id {id} not found.");
            }
            return userMapping;
        }
        public async Task<List<UserMapping>> GetUserMapsByGuild(ulong guildId)
        {
            return await Database.GetUserMappingsByGuildId(guildId);
        }
        public async Task<List<UserMapping>> GetUserMapsByUser(ulong userId)
        {
            return await Database.GetUserMappingsByUserId(userId);
        }
        public async Task<List<UserMapping>> GetUserMapsByGuildAndUser(ulong guildId, ulong userId)
        {
            return await Database.GetUserMappingsByUserIdAndGuildId(userId, guildId);
        }
        public async Task<UserMapping> CreateOrUpdateUserMap(ulong guildId, ulong userA, ulong userB, string content)
        {
            if (await DiscordAPI.FetchUserInfo(userA, CacheBehavior.Default) == null) throw new InvalidIUserException("User not found", userA);
            if (await DiscordAPI.FetchUserInfo(userB, CacheBehavior.Default) == null) throw new InvalidIUserException("User not found", userB);
            if (userA == userB) throw new InvalidUserMapException();

            UserMapping userMapping;
            RestAction action = RestAction.Updated;
            try
            {
                userMapping = await GetUserMap(guildId, userA, userB);
            }
            catch (ResourceNotFoundException)
            {
                userMapping = new UserMapping
                {
                    GuildId = guildId,
                    UserA = userA,
                    UserB = userB
                };
                action = RestAction.Created;
            }
            userMapping.CreatedAt = DateTime.UtcNow;
            userMapping.CreatorUserId = _currentUser.Id;

            userMapping.Reason = content;

            Database.SaveUserMapping(userMapping);
            await Database.SaveChangesAsync();

            if (action == RestAction.Created)
            {
                _eventHandler.OnUserMapUpdatedEvent.InvokeAsync(userMapping, _currentUser);
            }
            else
            {
                _eventHandler.OnUserMapUpdatedEvent.InvokeAsync(userMapping, _currentUser);
            }

            return userMapping;
        }
        public async Task DeleteUserMap(int id)
        {
            UserMapping userMapping = await GetUserMap(id);

            Database.DeleteUserMapping(userMapping);
            await Database.SaveChangesAsync();

            _eventHandler.OnUserMapDeletedEvent.InvokeAsync(userMapping, _currentUser);
        }
        public async Task DeleteForGuild(ulong guildId)
        {
            await Database.DeleteUserMappingByGuild(guildId);
            await Database.SaveChangesAsync();
        }
        public async Task<int> CountAllUserMapsByGuild(ulong guildId)
        {
            return await Database.CountUserMappingsForGuild(guildId);
        }
        public async Task<int> CountAllUserMaps()
        {
            return await Database.CountUserMappings();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Dtos.Tokens;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace masz.Repositories
{

    public class UserMapRepository : BaseRepository<UserMapRepository>
    {
        private readonly DiscordUser _currentUser;
        private UserMapRepository(IServiceProvider serviceProvider, DiscordUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private UserMapRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
        }
        public static UserMapRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new UserMapRepository(serviceProvider, identity.GetCurrentUser());
        public static UserMapRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new UserMapRepository(serviceProvider);
        public async Task<UserMapping> GetUserMap(ulong guildId, ulong userA, ulong userB)
        {
            UserMapping userMapping = await _database.GetUserMappingByUserIdsAndGuildId(userA, userB, guildId);
            if (userMapping == null)
            {
                _logger.LogWarning($"UserMap for guild {guildId} and users {userA}/{userB} not found.");
                throw new ResourceNotFoundException($"UserMap for guild {guildId} and users {userA}/{userB} not found.");
            }
            return userMapping;
        }
        public async Task<UserMapping> GetUserMap(int id)
        {
            UserMapping userMapping = await _database.GetUserMappingById(id);
            if (userMapping == null)
            {
                _logger.LogWarning($"UserMap for id {id} not found.");
                throw new ResourceNotFoundException($"UserMap for id {id} not found.");
            }
            return userMapping;
        }
        public async Task<List<UserMapping>> GetUserMapsByGuild(ulong guildId)
        {
            return await _database.GetUserMappingsByGuildId(guildId);
        }
        public async Task<List<UserMapping>> GetUserMapsByUser(ulong userId)
        {
            return await _database.GetUserMappingsByUserId(userId);
        }
        public async Task<List<UserMapping>> GetUserMapsByGuildAndUser(ulong guildId, ulong userId)
        {
            return await _database.GetUserMappingsByUserIdAndGuildId(userId, guildId);
        }
        public async Task<UserMapping> CreateOrUpdateUserMap(ulong guildId, ulong userA, ulong userB, string content)
        {
            if( await _discordAPI.FetchUserInfo(userA, CacheBehavior.Default) == null) throw new InvalidDiscordUserException("User not found", userA);
            if( await _discordAPI.FetchUserInfo(userB, CacheBehavior.Default) == null) throw new InvalidDiscordUserException("User not found", userB);

            UserMapping userMapping;
            RestAction action = RestAction.Edited;
            try {
                userMapping = await GetUserMap(guildId, userA, userB);
            } catch (ResourceNotFoundException) {
                userMapping = new UserMapping();
                userMapping.GuildId = guildId;
                userMapping.UserA = userA;
                userMapping.UserB = userB;
                action = RestAction.Created;
            }
            userMapping.CreatedAt = DateTime.UtcNow;
            userMapping.CreatorUserId = _currentUser.Id;

            userMapping.Reason = content;

            _database.SaveUserMapping(userMapping);
            await _database.SaveChangesAsync();

            await _discordAnnouncer.AnnounceUserMapping(userMapping, _currentUser, action);

            return userMapping;
        }
        public async Task DeleteUserMap(int id)
        {
            UserMapping userMapping = await GetUserMap(id);

            _database.DeleteUserMapping(userMapping);
            await _database.SaveChangesAsync();

            await _discordAnnouncer.AnnounceUserMapping(userMapping, _currentUser, RestAction.Deleted);
        }
        public async Task DeleteForGuild(ulong guildId)
        {
            await _database.DeleteUserMappingByGuild(guildId);
            await _database.SaveChangesAsync();
        }
        public async Task<int> CountAllUserMapsByGuild(ulong guildId)
        {
            return await _database.CountUserMappingsForGuild(guildId);
        }
        public async Task<int> CountAllUserMaps()
        {
            return await _database.CountUserMappings();
        }
    }
}
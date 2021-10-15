using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using masz.Dtos.Tokens;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace masz.Repositories
{

    public class InviteRepository : BaseRepository<InviteRepository>
    {
        private InviteRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static InviteRepository CreateDefault(IServiceProvider serviceProvider) => new InviteRepository(serviceProvider);

        public async Task<int> CountInvites()
        {
            return await _database.CountTrackedInvites();
        }
        public async Task<int> CountInvitesForGuild(ulong guildId)
        {
            return await _database.CountTrackedInvitesForGuild(guildId);
        }
        public async Task<List<UserInvite>> GetInvitedForUserAndGuild(ulong userId, ulong guildId)
        {
            return await _database.GetInvitedUsersByUserAndGuild(userId, guildId);
        }
        public async Task<List<UserInvite>> GetInvitedForUser(ulong userId)
        {
            return await _database.GetInvitedUsersByUser(userId);
        }
        public async Task<List<UserInvite>> GetusedInvitesForUserAndGuild(ulong userId, ulong guildId)
        {
            return await _database.GetUsedInvitesByUserAndGuild(userId, guildId);
        }
        public async Task<List<UserInvite>> GetusedInvitesForUser(ulong userId)
        {
            return await _database.GetUsedInvitesByUser(userId);
        }
        public async Task<UserInvite> CreateInvite(UserInvite invite)
        {
            await _database.SaveInvite(invite);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeInviteUsageRegistered(new InviteUsageRegisteredEventArgs(invite));

            return invite;
        }
        public async Task DeleteInvitesByGuild(ulong guildId)
        {
            await _database.DeleteInviteHistoryByGuild(guildId);
            await _database.SaveChangesAsync();
        }
        public async Task<List<UserInvite>> GetInvitesByCode(string code)
        {
            return await _database.GetInvitesByCode(code);
        }
    }
}
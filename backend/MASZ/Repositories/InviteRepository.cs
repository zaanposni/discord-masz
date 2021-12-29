using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class InviteRepository : BaseRepository<InviteRepository>
    {
        private InviteRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static InviteRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<int> CountInvites()
        {
            return await Database.CountTrackedInvites();
        }
        public async Task<int> CountInvitesForGuild(ulong guildId)
        {
            return await Database.CountTrackedInvitesForGuild(guildId);
        }
        public async Task<List<UserInvite>> GetInvitedForUserAndGuild(ulong userId, ulong guildId)
        {
            return await Database.GetInvitedUsersByUserAndGuild(userId, guildId);
        }
        public async Task<List<UserInvite>> GetInvitedForUser(ulong userId)
        {
            return await Database.GetInvitedUsersByUser(userId);
        }
        public async Task<List<UserInvite>> GetusedInvitesForUserAndGuild(ulong userId, ulong guildId)
        {
            return await Database.GetUsedInvitesByUserAndGuild(userId, guildId);
        }
        public async Task<List<UserInvite>> GetusedInvitesForUser(ulong userId)
        {
            return await Database.GetUsedInvitesByUser(userId);
        }
        public async Task<UserInvite> CreateInvite(UserInvite invite)
        {
            await Database.SaveInvite(invite);
            await Database.SaveChangesAsync();

            _eventHandler.OnInviteUsageRegisteredEvent.InvokeAsync(invite);

            return invite;
        }
        public async Task DeleteInvitesByGuild(ulong guildId)
        {
            await Database.DeleteInviteHistoryByGuild(guildId);
            await Database.SaveChangesAsync();
        }
        public async Task<List<UserInvite>> GetInvitesByCode(string code)
        {
            return await Database.GetInvitesByCode(code);
        }
    }
}
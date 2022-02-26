using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class AppealRepository : BaseRepository<AppealRepository>
    {
        private AppealRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public static AppealRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<Appeal> Get(int id)
        {

        }
        public async Task<List<Appeal>> GetForGuild(ulong guildId, int page = 0)
        {

        }
        public async Task<Appeal> Create(Appeal appeal, List<AppealAnswer> answers)
        {

        }
        public async Task<Appeal> Update(Appeal appeal)
        {

        }
        public async Task<bool> UserIsAllowedToCreateNewAppeal(ulong guildId, ulong userId)
        {

        }
        public async Task<List<DbCount>> GetCounts(ulong guildId, DateTime since)
        {
            
        }
    }
}
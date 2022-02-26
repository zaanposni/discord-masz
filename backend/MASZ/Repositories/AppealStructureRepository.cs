using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class AppealStructureRepository : BaseRepository<AppealStructureRepository>
    {
        private AppealStructureRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public static AppealStructureRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<AppealStructure> Get(ulong guildId, int id)
        {

        }
        public async Task<AppealStructure> CreateOrUpdate(AppealStructure)
        {

        }
        public async Task<AppealStructure> Delete(ulong guildId, int id)
        {

        }
        public async Task<bool> ConfiguredForGuild(ulong guildId)
        {

        }
    }
}
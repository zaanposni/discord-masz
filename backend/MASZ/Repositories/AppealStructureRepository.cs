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

        public async Task<AppealStructure> GetById(ulong guildId, int id)
        {
            AppealStructure appealStructure = await Database.GetAppealStructure(guildId, id);
            if (appealStructure == null)
            {
                throw new ResourceNotFoundException($"Appeal Structure {id} not found");
            }
            return appealStructure;
        }
        public async Task<List<AppealStructure>> GetForGuild(ulong guildId)
        {
            return await Database.GetAppealStructure(guildId);
        }
        public async Task<AppealStructure> Create(AppealStructure appealStructure)
        {
            Database.SaveAppealStructure(appealStructure);
            await Database.SaveChangesAsync();
            return appealStructure;
        }
        public async Task<AppealStructure> Update(AppealStructure appealStructure)
        {
            Database.UpdateAppealStructure(appealStructure);
            await Database.SaveChangesAsync();
            return appealStructure;
        }
        public async Task<AppealStructure> DeleteById(ulong guildId, int id)
        {
            AppealStructure appealStructure = await Database.GetAppealStructure(guildId, id);
            if (appealStructure == null)
            {
                throw new ResourceNotFoundException($"Appeal Structure {id} not found");
            }

            List<AppealAnswer> answers = await AppealAnswerRepository.CreateDefault(_serviceProvider).GetByQuestionId(id);

            if (answers.Count > 0)
            {
                appealStructure.Deleted = true;
                return await Update(appealStructure);
            }
            else
            {
                Database.DeleteAppealStructure(appealStructure);
                await Database.SaveChangesAsync();
                return appealStructure;
            }
        }
        public async Task<bool> ConfiguredForGuild(ulong guildId)
        {
            return (await GetForGuild(guildId)).Any(x => !x.Deleted);
        }
    }
}
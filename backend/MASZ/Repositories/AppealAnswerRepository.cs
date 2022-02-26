using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

        public async Task<Appeal> Get(int id)
        {

        }
namespace MASZ.Repositories
{

    public class AppealAnswerRepository : BaseRepository<AppealAnswerRepository>
    {
        private AppealAnswerRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public static AppealAnswerRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<AppealAnswer>> GetForAppeal(int appealId)
        {

        }
        public async Task<AppealAnswer> Create(AppealAnswer answer)
        {

        }
    }
}
using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class AppealAnswerRepository : BaseRepository<AppealAnswerRepository>
    {
        private AppealAnswerRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public static AppealAnswerRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<AppealAnswer>> GetForAppeal(int appealId)
        {
            return await Database.GetAppealAnswers(appealId);
        }
        public async Task<List<AppealAnswer>> GetByQuestionId(int questionId)
        {
            return await Database.GetAppealAnswersByQuestionId(questionId);
        }
        public async Task<AppealAnswer> Create(AppealAnswer answer)
        {
            Database.CreateAppealAnswer(answer);
            await Database.SaveChangesAsync();
            return answer;
        }
    }
}
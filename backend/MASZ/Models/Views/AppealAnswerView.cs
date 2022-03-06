namespace MASZ.Models.Views
{
    public class AppealAnswerView
    {
        public int Id { get; set; }
        public int? AppealId { get; set; }
        public string Answer { get; set; }
        public int? QuestionId { get; set; }

        public AppealAnswerView(AppealAnswer answer)
        {
            Id = answer.Id;
            AppealId = answer.Appeal?.Id;
            Answer = answer.Answer;
            QuestionId = answer.AppealQuestion?.Id;
        }
    }
}

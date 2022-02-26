namespace MASZ.Models.Views
{
    public class AppealAnswerView
    {
        public string Answer { get; set; }
        public AppealStructureView Question { get; set; }

        public AppealAnswerView(AppealAnswer answer)
        {
            Answer = answer.Answer;
            Question = AppealStructureView.CreateOrDefault(answer.AppealQuestion);
        }
    }
}

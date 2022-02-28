using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Appeal
{
    public class AppealAnswerDto
    {
        [Required(ErrorMessage = "QuestionId field is required")]
        public int QuestionId { get; set; }
        [MaxLength(10240)]
        public string Answer { get; set; }
    }
}
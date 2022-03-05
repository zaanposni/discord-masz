using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Appeal
{
    public class AppealDto
    {
        public List<AppealAnswerDto> Answers { get; set; } = new();
    }
}
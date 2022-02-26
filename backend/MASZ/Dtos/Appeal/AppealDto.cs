using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Appeal
{
    public class AppealDto
    {
        [EmailAddress]
        [MaxLength(512)]
        public string Email { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public List<AppealAnswerDto> Answers { get; set; } = new();
    }
}
using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Appeal
{
    public class AppealStructureDto
    {
        [Required(ErrorMessage = "Question field is required")]
        [MaxLength(4096)]
        public string Question { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}
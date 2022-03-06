using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Appeal
{
    public class AppealStructureOrderDto
    {
        [Required(ErrorMessage = "Id field is required")]
        public int Id { get; set; }
        [Required(ErrorMessage = "SortOrder field is required")]
        public int SortOrder { get; set; }
    }
}
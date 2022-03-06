using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.Appeal
{
    public class AppealUpdateDto
    {
        [Required]
        public AppealStatus Status { get; set; }
        [Required]
        public string ModeratorComment { get; set; }
        public Nullable<DateTime> UserCanCreateNewAppeals { get; set; }
    }
}
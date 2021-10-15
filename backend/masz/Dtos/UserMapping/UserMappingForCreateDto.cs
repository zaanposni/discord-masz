using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.UserMapping
{
    public class UserMappingForCreateDto
    {
        [Required(ErrorMessage = "UserA field is required")]
        public ulong UserA { get; set; }

        [Required(ErrorMessage = "UserB field is required")]
        public ulong UserB { get; set; }

        public string Reason { get; set; }
    }
}
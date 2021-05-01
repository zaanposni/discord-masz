using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.UserMapping
{
    public class UserMappingForCreateDto
    {
        [Required(ErrorMessage = "UserA field is required", AllowEmptyStrings=false)]
        public string UserA { get; set; }

        [Required(ErrorMessage = "UserB field is required", AllowEmptyStrings=false)]
        public string UserB { get; set; }

        public string Reason { get; set; }
    }
}
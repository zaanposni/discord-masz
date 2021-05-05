using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.UserMapping
{
    public class UserMappingForCreateDto
    {
        [Required(ErrorMessage = "UserA field is required", AllowEmptyStrings=false)]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the user id can only consist of numbers and must be 18 characters long")]
        public string UserA { get; set; }

        [Required(ErrorMessage = "UserB field is required", AllowEmptyStrings=false)]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the user id can only consist of numbers and must be 18 characters long")]
        public string UserB { get; set; }

        public string Reason { get; set; }
    }
}
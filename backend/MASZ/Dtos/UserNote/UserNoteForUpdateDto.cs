using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.UserNote
{
    public class UserNoteForUpdateDto
    {
        [Required(ErrorMessage = "UserId field is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the user id can only consist of numbers and must be 18 characters long")]
        public ulong UserId { get; set; }

        [Required(ErrorMessage = "Description field is required", AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}
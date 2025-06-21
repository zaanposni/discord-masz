using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.UserNote
{
    public class UserNoteForUpdateDto
    {
        [Required(ErrorMessage = "UserId field is required", AllowEmptyStrings = false)]
        public ulong UserId { get; set; }

        [Required(ErrorMessage = "Description field is required", AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}
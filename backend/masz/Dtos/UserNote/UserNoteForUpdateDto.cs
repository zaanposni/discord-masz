using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.UserNote
{
    public class UserNoteForUpdateDto
    {
        [Required(ErrorMessage = "Description field is required", AllowEmptyStrings=false)]
        public string Description { get; set; }
    }
}
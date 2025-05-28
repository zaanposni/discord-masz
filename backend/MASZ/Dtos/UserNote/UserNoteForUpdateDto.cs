using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.UserNote
{
    public class UserNoteForUpdateDto
    {
        [Required(ErrorMessage = "UserId field is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9]{25}$", ErrorMessage = "the user id can only consist of numbers and must be 18 characters long")]
        /*The id-max is not good, because discord id's can getting longer, for long life suppor of the bot, we should delet the id-max value! i sett it now from 18 to 25*/
        public ulong UserId { get; set; }
    

        [Required(ErrorMessage = "Description field is required", AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}

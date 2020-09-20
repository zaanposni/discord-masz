using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCase
{
    public class ModCaseForCreateDto
    {
        [Required(ErrorMessage = "Title field is required")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description field is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "UserId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the user id can only consist of numbers and must be 18 characters long")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Severity field is required")]
        public int Severity { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OccuredAt { get; set; }
        [MaxLength(100)]
        public string Punishment { get; set; }
        public string[] Labels { get; set; }
        public string Others { get; set; }
    }
}
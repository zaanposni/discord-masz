using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Dtos.ModCase
{
    public class ModCaseForCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{18}$")]
        public string UserId { get; set; }
        [Required]
        public int Severity { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OccuredAt { get; set; }
        public string Punishment { get; set; }
        public string[] Labels { get; set; }
        public string Others { get; set; }
    }
}
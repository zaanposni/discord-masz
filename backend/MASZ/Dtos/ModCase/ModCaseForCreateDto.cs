using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.ModCase
{
    public class ModCaseForCreateDto
    {
        [Required(ErrorMessage = "Title field is required")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description field is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "UserId field is required")]
        public ulong UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OccuredAt { get; set; }
        public string[] Labels { get; set; } = Array.Empty<string>();
        public string Others { get; set; }
        [Required(ErrorMessage = "PunishmentType field is required")]
        [EnumDataType(typeof(PunishmentType))]
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool PunishmentActive { get; set; } = false;
    }
}
using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.ModCase
{
    public class CaseTemplateForCreateDto
    {
        [Required(ErrorMessage = "TemplateName field is required", AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string TemplateName { get; set; }
        [Required(ErrorMessage = "ViewPermission field is required")]
        public ViewPermission ViewPermission { get; set; }
        [Required(ErrorMessage = "Title field is required")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description field is required")]
        public string Description { get; set; }
        public string[] Labels { get; set; } = Array.Empty<string>();
        [Required(ErrorMessage = "PunishmentType field is required")]
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        [Required]
        public bool SendPublicNotification { get; set; } = false;
        [Required]
        public bool HandlePunishment { get; set; } = false;
        [Required]
        public bool AnnounceDm { get; set; } = false;
    }
}
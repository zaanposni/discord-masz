using System;
using System.ComponentModel.DataAnnotations;
using masz.Enums;

namespace masz.Dtos.ModCase
{
    public class CaseTemplateForCreateDto
    {
        [Required(ErrorMessage = "TemplateName field is required", AllowEmptyStrings=false)]
        [MaxLength(100)]
        public string TemplateName { get; set; }
        [Required(ErrorMessage = "ViewPermission field is required")]
        public ViewPermission ViewPermission { get; set; }
        [Required(ErrorMessage = "Title field is required")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description field is required")]
        public string Description { get; set; }
        public string[] Labels { get; set; } = new string[0];
        [Required(ErrorMessage = "PunishmentType field is required")]
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        [Required]
        public bool sendPublicNotification { get; set; } = false;
        [Required]
        public bool handlePunishment { get; set; } = false;
        [Required]
        public bool announceDm { get; set; } = false;
    }
}
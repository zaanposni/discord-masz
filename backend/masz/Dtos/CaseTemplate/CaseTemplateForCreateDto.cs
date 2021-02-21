using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using masz.Models;

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
        [Required(ErrorMessage = "Punishment field is required")]
        [MaxLength(100)]
        public string Punishment { get; set; }
        public string[] Labels { get; set; } = new string[0];
        [Required(ErrorMessage = "PunishmentType field is required")]
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool sendPublicNotification { get; set; } = false;
        public bool handlePunishment { get; set; } = false;
        public bool announceDm { get; set; } = false;
    }
}
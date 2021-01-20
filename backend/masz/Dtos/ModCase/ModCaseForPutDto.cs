using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using masz.Models;

namespace masz.Dtos.ModCase
{
    public class ModCaseForPutDto
    {
        [Required(ErrorMessage = "Title field is required")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description field is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "UserId field is required")]
        [RegularExpression(@"^[0-9]{18}$", ErrorMessage = "the user id can only consist of numbers and must be 18 characters long")]
        public string UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OccuredAt { get; set; }
        [Required(ErrorMessage = "Punishment field is required")]
        [MaxLength(100)]
        public string Punishment { get; set; }
        public string[] Labels { get; set; } = new string[0];
        public string Others { get; set; }
        [Required(ErrorMessage = "PunishmentType field is required")]
        public PunishmentType PunishmentType { get; set; }
        public DateTime? PunishedUntil { get; set; }
        public bool PunishmentActive { get; set; } = false;
    }
}
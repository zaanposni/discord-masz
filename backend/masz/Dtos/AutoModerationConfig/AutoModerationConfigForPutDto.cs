using System;
using System.ComponentModel.DataAnnotations;
using masz.Models;

namespace masz.Dtos.AutoModerationConfig
{
    public class AutoModerationConfigForPutDto
    {
        [Required]
        public AutoModerationType AutoModerationType { get; set; }
        [Required]
        public AutoModerationAction AutoModerationAction { get; set; }
        public PunishmentType? PunishmentType { get; set; }
        public int? PunishmentDurationMinutes { get; set; }
        public string[] IgnoreChannels { get; set; } = new string[0];
        public string[] IgnoreRoles { get; set; } = new string[0];
        public int? TimeLimitMinutes { get; set; }
        public int? Limit { get; set; }
        public string CustomWordFilter { get; set; }
        public bool SendDmNotification { get; set; }
        public bool SendPublicNotification { get; set; }
    }
}
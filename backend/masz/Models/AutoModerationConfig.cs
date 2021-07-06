using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using masz.Dtos.ModCase;

namespace masz.Models
{
    public class AutoModerationConfig
    {
        [Key]
        public int Id { get; set; }
        public string GuildId { get; set; }
        public AutoModerationType AutoModerationType { get; set; }
        public AutoModerationAction AutoModerationAction { get; set; }
        public PunishmentType? PunishmentType { get; set; }
        public int? PunishmentDurationMinutes { get; set; }
        public string[] IgnoreChannels { get; set; }
        public string[] IgnoreRoles { get; set; }
        public int? TimeLimitMinutes { get; set; }
        public int? Limit { get; set; }
        public string CustomWordFilter { get; set; }
        public bool SendDmNotification { get; set; }
        public bool SendPublicNotification { get; set; }
    }
}

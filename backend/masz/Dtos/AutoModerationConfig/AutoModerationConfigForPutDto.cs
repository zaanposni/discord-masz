using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.AutoModerationConfig
{
    public class AutoModerationConfigForPutDto
    {
        [Required]
        public AutoModerationType AutoModerationType { get; set; }
        [Required]
        public AutoModerationAction AutoModerationAction { get; set; }
        public PunishmentType? PunishmentType { get; set; }
        public int? PunishmentDurationMinutes { get; set; }
        public ulong[] IgnoreChannels { get; set; } = Array.Empty<ulong>();
        public ulong[] IgnoreRoles { get; set; } = Array.Empty<ulong>();
        public int? TimeLimitMinutes { get; set; }
        public int? Limit { get; set; }
        public string CustomWordFilter { get; set; }
        public bool SendDmNotification { get; set; }
        public bool SendPublicNotification { get; set; }
        public AutoModerationChannelNotificationBehavior ChannelNotificationBehavior { get; set; }
    }
}
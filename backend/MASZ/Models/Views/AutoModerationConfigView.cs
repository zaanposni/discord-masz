using MASZ.Enums;

namespace MASZ.Models
{
    public class AutoModerationConfigView
    {
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
        public AutoModerationChannelNotificationBehavior ChannelNotificationBehavior { get; set; }
        public AutoModerationConfigView(AutoModerationConfig config)
        {
            Id = config.Id;
            GuildId = config.GuildId.ToString();
            AutoModerationType = config.AutoModerationType;
            AutoModerationAction = config.AutoModerationAction;
            PunishmentType = config.PunishmentType;
            PunishmentDurationMinutes = config.PunishmentDurationMinutes;
            IgnoreChannels = config.IgnoreChannels.Select(x => x.ToString()).ToArray();
            IgnoreRoles = config.IgnoreRoles.Select(x => x.ToString()).ToArray();
            TimeLimitMinutes = config.TimeLimitMinutes;
            Limit = config.Limit;
            CustomWordFilter = config.CustomWordFilter;
            SendDmNotification = config.SendDmNotification;
            SendPublicNotification = config.SendPublicNotification;
            ChannelNotificationBehavior = config.ChannelNotificationBehavior;
        }
    }
}

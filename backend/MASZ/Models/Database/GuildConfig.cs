using MASZ.Enums;

namespace MASZ.Models
{
    public class GuildConfig : ICloneable
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong[] ModRoles { get; set; }
        public ulong[] AdminRoles { get; set; }
        public ulong[] MutedRoles { get; set; }
        public bool ModNotificationDM { get; set; }
        public string ModPublicNotificationWebhook { get; set; }
        public string ModInternalNotificationWebhook { get; set; }
        public bool StrictModPermissionCheck { get; set; }
        public bool ExecuteWhoisOnJoin { get; set; }
        public bool PublishModeratorInfo { get; set; } = true;
        public Language PreferredLanguage { get; set; }
        public int AllowBanAppealAfterDays { get; set; }
        public bool PublicEmbedMode { get; set; }

        public object Clone()
        {
            return new GuildConfig
            {
                Id = Id,
                GuildId = GuildId,
                ModRoles = ModRoles,
                AdminRoles = AdminRoles,
                MutedRoles = MutedRoles,
                ModNotificationDM = ModNotificationDM,
                ModPublicNotificationWebhook = ModPublicNotificationWebhook,
                ModInternalNotificationWebhook = ModInternalNotificationWebhook,
                StrictModPermissionCheck = StrictModPermissionCheck,
                ExecuteWhoisOnJoin = ExecuteWhoisOnJoin,
                PreferredLanguage = PreferredLanguage,
                AllowBanAppealAfterDays = AllowBanAppealAfterDays,
                PublicEmbedMode = PublicEmbedMode
            };
        }
    }
}

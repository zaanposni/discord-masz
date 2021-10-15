using System;
using masz.Enums;

namespace masz.Models
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
        public bool PublishModeratorInfo { get; set; }
        public Language PreferredLanguage { get; set; }

        public object Clone()
        {
            return new GuildConfig {
                Id = this.Id,
                GuildId = this.GuildId,
                ModRoles = this.ModRoles,
                AdminRoles = this.AdminRoles,
                MutedRoles = this.MutedRoles,
                ModNotificationDM = this.ModNotificationDM,
                ModPublicNotificationWebhook = this.ModPublicNotificationWebhook,
                ModInternalNotificationWebhook = this.ModInternalNotificationWebhook,
                StrictModPermissionCheck = this.StrictModPermissionCheck,
                ExecuteWhoisOnJoin = this.ExecuteWhoisOnJoin,
                PreferredLanguage = this.PreferredLanguage
            };
        }
    }
}

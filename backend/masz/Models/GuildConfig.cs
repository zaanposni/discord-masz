using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class GuildConfig : ICloneable
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string ModRoleId { get; set; }
        public string AdminRoleId { get; set; }
        public string MutedRoleId { get; set; }
        public bool ModNotificationDM { get; set; }
        public string ModPublicNotificationWebhook { get; set; }
        public string ModInternalNotificationWebhook { get; set; }

        public object Clone()
        {
            return new GuildConfig {
                Id = this.Id,
                GuildId = this.GuildId,
                ModRoleId = this.ModRoleId,
                AdminRoleId = this.AdminRoleId,
                MutedRoleId = this.MutedRoleId,
                ModNotificationDM = this.ModNotificationDM,
                ModPublicNotificationWebhook = this.ModPublicNotificationWebhook,
                ModInternalNotificationWebhook = this.ModInternalNotificationWebhook
            };
        }
    }
}

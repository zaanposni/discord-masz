using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class GuildConfig
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string ModRoleId { get; set; }
        public string AdminRoleId { get; set; }
        public bool ModNotificationDM { get; set; }
        public string ModPublicNotificationWebhook { get; set; }
        public string ModInternalNotificationWebhook { get; set; }
    }
}

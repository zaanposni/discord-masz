using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Models
{
    public class ModGuild
    {
        public int Id { get; set; }
        public int GuildId { get; set; }
        public int ModRoleId { get; set; }
        public int AdminRoleId { get; set; }
        public int ModNotificationWebhook { get; set; }
    }
}

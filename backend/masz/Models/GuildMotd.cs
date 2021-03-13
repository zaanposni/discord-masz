using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class GuildMotd
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public bool ShowMotd { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class UserMapping
    {
        [Key]
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string UserA { get; set; }
        public string UserB { get; set; }
        public string CreatorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
    }
}

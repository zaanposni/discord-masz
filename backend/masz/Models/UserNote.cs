using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Models
{
    public class UserNote
    {
        [Key]
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string CreatorId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

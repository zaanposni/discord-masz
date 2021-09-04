using System;
using System.ComponentModel.DataAnnotations;

namespace masz.Models
{
    public class UserNote
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public string Description { get; set; }
        public ulong CreatorId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

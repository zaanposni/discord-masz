using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class UserMapping
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserA { get; set; }
        public ulong UserB { get; set; }
        public ulong CreatorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using MASZ.Enums;

namespace MASZ.Models
{
    public class Appeal
    {
        [Key]
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Mail { get; set; }
        public ulong GuildId { get; set; }
        public AppealStatus Status { get; set; }
        public string ModeratorComment { get; set; }
        public ulong LastModeratorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Nullable<DateTime> UserCanCreateNewAppeals { get; set; }
        public Nullable<DateTime> InvalidDueToLaterRejoinAt { get; set; }
    }
}
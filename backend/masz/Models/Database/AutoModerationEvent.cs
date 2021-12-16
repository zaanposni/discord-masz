using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class AutoModerationEvent
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public AutoModerationType AutoModerationType { get; set; }
        public AutoModerationAction AutoModerationAction { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Discriminator { get; set; }
        public ulong MessageId { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? AssociatedCaseId { get; set; }

        public AutoModerationEvent() { }
    }
}

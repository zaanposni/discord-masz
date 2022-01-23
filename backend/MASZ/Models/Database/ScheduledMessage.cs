using MASZ.Enums;
using System.ComponentModel.DataAnnotations;

namespace MASZ.Models
{
    public class ScheduledMessage
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime ScheduledFor { get; set; }
        public ScheduledMessageStatus Status { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong CreatorId { get; set; }
        public ulong LastEditedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public ScheduledMessageFailureReason? FailureReason { get; set; }
    }
}

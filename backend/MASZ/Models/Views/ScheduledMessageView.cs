using MASZ.Enums;

namespace MASZ.Models
{
    public class ScheduledMessageView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime ScheduledFor { get; set; }
        public ScheduledMessageStatus Status { get; set; }
        public string GuildId { get; set; }
        public string ChannelId { get; set; }
        public string CreatorId { get; set; }
        public string LastEditedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public ScheduledMessageFailureReason? FailureReason { get; set; }

        public ScheduledMessageView(ScheduledMessage message)
        {
            Id = message.Id;
            Name = message.Name;
            Content = message.Content;
            ScheduledFor = message.ScheduledFor;
            Status = message.Status;
            GuildId = message.GuildId.ToString();
            ChannelId = message.ChannelId.ToString();
            CreatorId = message.CreatorId.ToString();
            LastEditedById = message.LastEditedById.ToString();
            CreatedAt = message.CreatedAt;
            LastEditedAt = message.LastEditedAt;
            FailureReason = message.FailureReason;
        }
    }
}

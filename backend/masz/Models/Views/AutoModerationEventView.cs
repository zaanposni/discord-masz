using MASZ.Enums;

namespace MASZ.Models
{
    public class AutoModerationEventView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public AutoModerationType AutoModerationType { get; set; }
        public AutoModerationAction AutoModerationAction { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Discriminator { get; set; }
        public string MessageId { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? AssociatedCaseId { get; set; }

        public AutoModerationEventView(AutoModerationEvent autoModerationEvent)
        {
            Id = autoModerationEvent.Id;
            GuildId = autoModerationEvent.GuildId.ToString();
            AutoModerationType = autoModerationEvent.AutoModerationType;
            AutoModerationAction = autoModerationEvent.AutoModerationAction;
            UserId = autoModerationEvent.UserId.ToString();
            Username = autoModerationEvent.Username;
            Nickname = autoModerationEvent.Nickname;
            Discriminator = autoModerationEvent.Discriminator;
            MessageId = autoModerationEvent.MessageId.ToString();
            MessageContent = autoModerationEvent.MessageContent;
            CreatedAt = autoModerationEvent.CreatedAt;
            AssociatedCaseId = autoModerationEvent.AssociatedCaseId;
        }
    }
}

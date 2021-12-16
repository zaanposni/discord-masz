using MASZ.Enums;

namespace MASZ.Dtos.AutoModerationEvent
{
    public class AutoModerationEventForCreateDto
    {
        public string UserId { get; set; }
        public AutoModerationType AutoModerationType { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Discriminator { get; set; }
        public string MessageId { get; set; }
        public string ChannelId { get; set; }
        public string MessageContent { get; set; }
    }
}
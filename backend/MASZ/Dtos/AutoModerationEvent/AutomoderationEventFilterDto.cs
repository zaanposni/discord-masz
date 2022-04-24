using MASZ.Enums;

namespace MASZ.Dtos.AutoModerationEvent
{
    public class AutomoderationEventFilterDto
    {
        public List<string> UserIds { get; set; }
        public List<AutoModerationAction> actions { get; set; }
        public List<AutoModerationType> types { get; set; }
    }
}
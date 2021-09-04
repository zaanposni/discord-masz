using DSharpPlus.Entities;

namespace masz.Models
{
    public class AutoModerationEventTableEntry
    {
        public AutoModerationEvent AutoModerationEvent { get; set; }
        public DiscordUser Suspect { get; set; }
    }
}

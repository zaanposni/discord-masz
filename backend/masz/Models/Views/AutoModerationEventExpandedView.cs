using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class AutoModerationEventExpandedView
    {
        public AutoModerationEventView AutoModerationEvent { get; set; }
        public DiscordUserView Suspect { get; set; }

        public AutoModerationEventExpandedView(AutoModerationEvent moderationEvent, DiscordUser suspect)
        {
            this.AutoModerationEvent = new AutoModerationEventView(moderationEvent);
            this.Suspect = DiscordUserView.CreateOrDefault(suspect);
        }
    }
}

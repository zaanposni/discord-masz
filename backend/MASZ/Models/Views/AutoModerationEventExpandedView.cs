using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class AutoModerationEventExpandedView
    {
        public AutoModerationEventView AutoModerationEvent { get; set; }
        public DiscordUserView Suspect { get; set; }

        public AutoModerationEventExpandedView(AutoModerationEvent moderationEvent, IUser suspect)
        {
            AutoModerationEvent = new AutoModerationEventView(moderationEvent);
            Suspect = DiscordUserView.CreateOrDefault(suspect);
        }
    }
}

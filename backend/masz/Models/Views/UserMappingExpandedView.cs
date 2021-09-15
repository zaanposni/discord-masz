using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class UserMappingExpandedView
    {
        public UserMappingView UserMapping { get; set; }
        public DiscordUserView UserA { get; set; }
        public DiscordUserView UserB { get; set; }
        public DiscordUserView Moderator { get; set; }

        public UserMappingExpandedView(UserMapping userMapping, DiscordUser userA, DiscordUser userB, DiscordUser moderator)
        {
            UserMapping = new UserMappingView(userMapping);
            UserA = DiscordUserView.CreateOrDefault(userA);
            UserB = DiscordUserView.CreateOrDefault(userB);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
        }
    }
}

using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class UserMappingExpandedView
    {
        public UserMappingView UserMapping { get; set; }
        public DiscordUserView UserA { get; set; }
        public DiscordUserView UserB { get; set; }
        public DiscordUserView Moderator { get; set; }

        public UserMappingExpandedView(UserMapping userMapping, IUser userA, IUser userB, IUser moderator)
        {
            UserMapping = new UserMappingView(userMapping);
            UserA = DiscordUserView.CreateOrDefault(userA);
            UserB = DiscordUserView.CreateOrDefault(userB);
            Moderator = DiscordUserView.CreateOrDefault(moderator);
        }
    }
}

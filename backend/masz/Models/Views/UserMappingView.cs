using DSharpPlus.Entities;

namespace masz.Models
{
    public class UserMappingView
    {
        public UserMapping UserMapping { get; set; }
        public DiscordUser UserA { get; set; }
        public DiscordUser UserB { get; set; }
        public DiscordUser Moderator { get; set; }
    }
}

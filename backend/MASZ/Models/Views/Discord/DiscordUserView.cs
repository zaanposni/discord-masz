using Discord;

namespace MASZ.Models.Views
{
    public class DiscordUserView
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string ImageUrl { get; set; }
        public string Locale { get; set; }
        public string Avatar { get; set; }
        public bool Bot { get; set; }

        private DiscordUserView() { }
        private DiscordUserView(IUser user)
        {
            if (user == null) return;
            Id = user.Id.ToString();
            Username = user.Username;
            Discriminator = user.Discriminator;
            ImageUrl = user.GetAvatarUrl(size: 512) ?? user.GetDefaultAvatarUrl();
            Locale = user is ISelfUser sUser ? sUser.Locale : "en-US";
            Avatar = user.AvatarId;
            Bot = user.IsBot;
        }

        public static DiscordUserView CreateOrDefault(IUser user)
        {
            if (user == null) return null;
            if (user.Id == 0) return null;
            return new DiscordUserView(user);
        }
    }
}

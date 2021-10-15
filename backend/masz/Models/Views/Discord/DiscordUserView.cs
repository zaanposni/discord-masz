using DSharpPlus.Entities;

namespace masz.Models.Views
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
        private DiscordUserView(DiscordUser user)
        {
            if (user == null) return;
            Id = user.Id.ToString();
            Username = user.Username;
            Discriminator = user.Discriminator;
            ImageUrl = user.AvatarUrl;
            Locale = user.Locale;
            Avatar = user.AvatarHash;
            Bot = user.IsBot;
        }

        public static DiscordUserView CreateOrDefault(DiscordUser user)
        {
            if (user == null) return null;
            if (user.Id == 0) return null;
            return new DiscordUserView(user);
        }
    }
}

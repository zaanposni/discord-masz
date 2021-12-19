using Discord;

namespace MASZ.Models.Views
{
    public class DiscordGuildView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public List<DiscordRoleView> Roles { get; set; }

        public DiscordGuildView() { }
        public DiscordGuildView(IGuild guild)
        {
            Id = guild.Id.ToString();
            Name = guild.Name;
            IconUrl = getAnimatedOrDefaultAvatar(guild.IconUrl);
            Roles = new List<DiscordRoleView>();
            try
            {
                foreach (IRole role in guild.Roles)
                {
                    Roles.Add(new DiscordRoleView(role));
                }
            }
            catch (Exception) { }
        }
        public DiscordGuildView(UserGuild guild)
        {
            Id = guild.Id.ToString();
            Name = guild.Name;
            IconUrl = getAnimatedOrDefaultAvatar(guild.IconUrl);
            Roles = new List<DiscordRoleView>();
        }

        private string getAnimatedOrDefaultAvatar(string iconUrl)
        {
            if (iconUrl != null)
            {
                if (iconUrl.Split("/").Last().StartsWith("a_"))
                {
                    iconUrl = iconUrl.Replace(".jpg", ".gif");
                }
                if (!iconUrl.Contains("?"))
                {
                    iconUrl += "?size=512";
                }
            }
            return iconUrl;
        }
    }
}

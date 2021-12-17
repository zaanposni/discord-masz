using Discord;

namespace MASZ.Models.Views
{
    public class DiscordGuildView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }  // TODO: change to this in angular
        public List<DiscordRoleView> Roles { get; set; }

        public DiscordGuildView() { }
        public DiscordGuildView(IGuild guild)
        {
            Id = guild.Id.ToString();
            Name = guild.Name;
            IconUrl = guild.IconUrl;
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
            IconUrl = guild.IconUrl;
            Roles = new List<DiscordRoleView>();
        }
    }
}

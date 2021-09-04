using DSharpPlus.Entities;

namespace masz.Models.Views
{
    public class DiscordRoleView
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public int Color { get; set; }
        public int Position { get; set; }
        public string Permissions { get; set; }

        public DiscordRoleView() { }
        public DiscordRoleView(DiscordRole guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            Color = guild.Color.Value;
            Position = guild.Position;
            Permissions = guild.Permissions.GetHashCode().ToString();
        }
    }
}

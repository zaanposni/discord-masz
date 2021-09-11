using DSharpPlus.Entities;

namespace masz.Models.Views
{
    public class DiscordRoleView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Color { get; set; }
        public int Position { get; set; }
        public string Permissions { get; set; }

        public DiscordRoleView() { }
        public DiscordRoleView(DiscordRole role)
        {
            Id = role.Id.ToString();
            Name = role.Name;
            Color = role.Color.Value;
            Position = role.Position;
            Permissions = role.Permissions.GetHashCode().ToString();
        }
    }
}

using System.Collections.Generic;
using DSharpPlus.Entities;

namespace masz.Models.Views
{
    public class DiscordGuildView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconUrl { get; set; }
        public List<DiscordRoleView> Roles { get; set; }

        public DiscordGuildView() { }
        public DiscordGuildView(DiscordGuild guild)
        {
            Id = guild.Id.ToString();
            Name = guild.Name;
            Icon = guild.IconHash;
            IconUrl = guild.IconUrl;
            Roles = new List<DiscordRoleView>();
            try {
                foreach (DiscordRole role in guild.Roles.Values)
                {
                    Roles.Add(new DiscordRoleView(role));
                }
            } catch (System.Exception) { }  // some times this stupid library fails, so we just ignore it
        }
    }
}

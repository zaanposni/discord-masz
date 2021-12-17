using Discord;
using Discord.Rest;

namespace MASZ.Models
{
    public class UserGuild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }

        public UserGuild(IGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            IconUrl = guild.IconUrl;
        }

        public UserGuild(RestUserGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            IconUrl = guild.IconUrl;
        }
    }
}

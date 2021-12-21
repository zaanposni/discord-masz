using Discord;
using MASZ.Extensions;

namespace MASZ.Models
{
    public class UserGuild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public bool IsAdmin { get; set; }

        public UserGuild(IGuildUser user)
        {
            Id = user.Guild.Id;
            Name = user.Guild.Name;
            IconUrl = user.Guild.IconUrl.GetAnimatedOrDefaultAvatar();
            IsAdmin = user.GuildPermissions.Administrator;
        }

        public UserGuild(IUserGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            IconUrl = guild.IconUrl.GetAnimatedOrDefaultAvatar();
            IsAdmin = guild.Permissions.Administrator;
        }
    }
}

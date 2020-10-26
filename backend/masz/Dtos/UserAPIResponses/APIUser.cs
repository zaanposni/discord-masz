using System.Collections.Generic;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Dtos.UserAPIResponses
{
    public class APIUser
    {
        public APIUser(List<string> memberGuilds, List<string> bannedGuilds, List<string> modGuilds, User discordUser, bool isAdmin=false)
        {
            MemberGuilds = memberGuilds;
            BannedGuilds = bannedGuilds;
            ModGuilds = modGuilds;
            DiscordUser = discordUser;
            IsAdmin = isAdmin;
        }

        public List<string> MemberGuilds { get; set; }
        public List<string> BannedGuilds { get; set; }
        public List<string> ModGuilds { get; set; }
        public User DiscordUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}
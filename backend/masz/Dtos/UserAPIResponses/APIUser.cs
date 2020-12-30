using System.Collections.Generic;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Dtos.UserAPIResponses
{
    public class APIUser
    {
        public APIUser(List<Guild> memberGuilds, List<Guild> bannedGuilds, List<Guild> modGuilds, List<Guild> adminGuilds, User discordUser, bool isAdmin=false)
        {
            MemberGuilds = memberGuilds;
            BannedGuilds = bannedGuilds;
            ModGuilds = modGuilds;
            AdminGuilds = adminGuilds;
            DiscordUser = discordUser;
            IsAdmin = isAdmin;
        }

        public List<Guild> MemberGuilds { get; set; }
        public List<Guild> BannedGuilds { get; set; }
        public List<Guild> ModGuilds { get; set; }
        public List<Guild> AdminGuilds { get; set; }
        public User DiscordUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}
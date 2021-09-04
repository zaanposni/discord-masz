using System.Collections.Generic;
using DSharpPlus.Entities;

namespace masz.Dtos.UserAPIResponses
{
    public class APIUser
    {
        public APIUser(List<DiscordGuild> memberGuilds, List<DiscordGuild> bannedGuilds, List<DiscordGuild> modGuilds, List<DiscordGuild> adminGuilds, DiscordUser discordUser, bool isAdmin=false)
        {
            MemberGuilds = memberGuilds;
            BannedGuilds = bannedGuilds;
            ModGuilds = modGuilds;
            AdminGuilds = adminGuilds;
            DiscordUser = discordUser;
            IsAdmin = isAdmin;
        }

        public List<DiscordGuild> MemberGuilds { get; set; }
        public List<DiscordGuild> BannedGuilds { get; set; }
        public List<DiscordGuild> ModGuilds { get; set; }
        public List<DiscordGuild> AdminGuilds { get; set; }
        public DiscordUser DiscordUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}
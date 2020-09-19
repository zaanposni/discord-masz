using System.Collections.Generic;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Dtos.UserAPIResponses
{
    public class APIUser
    {
        public APIUser(List<string> guilds, User discordUser)
        {
            Guilds = guilds;
            DiscordUser = discordUser;
        }

        public List<string> Guilds { get; set; }
        public User DiscordUser { get; set; }
    }
}
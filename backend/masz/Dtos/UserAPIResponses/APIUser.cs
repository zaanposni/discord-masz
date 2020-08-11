using System.Collections.Generic;
using masz.Dtos.DiscordAPIResponses;

namespace masz.Dtos.UserAPIResponses
{
    public class APIUser
    {
        public APIUser(List<string> moderationGuilds, User discordUser)
        {
            ModerationGuilds = moderationGuilds;
            DiscordUser = discordUser;
        }

        public List<string> ModerationGuilds { get; set; }
        public User DiscordUser { get; set; }
    }
}
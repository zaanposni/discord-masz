using Discord;
using MASZ.Models.Views;

namespace MASZ.Dtos.UserAPIResponses
{
    public class APIUser
    {
        public APIUser(List<DiscordGuildView> memberGuilds, List<DiscordGuildView> bannedGuilds, List<DiscordGuildView> modGuilds, List<DiscordGuildView> adminGuilds, IUser user, bool isAdmin = false)
        {
            MemberGuilds = memberGuilds;
            BannedGuilds = bannedGuilds;
            ModGuilds = modGuilds;
            AdminGuilds = adminGuilds;
            DiscordUser = DiscordUserView.CreateOrDefault(user);
            IsAdmin = isAdmin;
        }

        public List<DiscordGuildView> MemberGuilds { get; set; }
        public List<DiscordGuildView> BannedGuilds { get; set; }
        public List<DiscordGuildView> ModGuilds { get; set; }
        public List<DiscordGuildView> AdminGuilds { get; set; }
        public DiscordUserView DiscordUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}
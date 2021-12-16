using Discord;
using MASZ.Models.Views;

namespace MASZ.Models
{
    public class GuildMotdExpandedView
    {
        public GuildMotdView Motd { get; set; }
        public DiscordUserView Creator { get; set; }

        public GuildMotdExpandedView(GuildMotd moderationEvent, IUser creator)
        {
            Motd = new GuildMotdView(moderationEvent);
            Creator = DiscordUserView.CreateOrDefault(creator);
        }
    }
}

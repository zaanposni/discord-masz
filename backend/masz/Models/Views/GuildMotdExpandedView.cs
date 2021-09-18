using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class GuildMotdExpandedView
    {
        public GuildMotdView Motd { get; set; }
        public DiscordUserView Creator { get; set; }

        public GuildMotdExpandedView(GuildMotd moderationEvent, DiscordUser creator)
        {
            Motd = new GuildMotdView(moderationEvent);
            Creator = DiscordUserView.CreateOrDefault(creator);
        }
    }
}

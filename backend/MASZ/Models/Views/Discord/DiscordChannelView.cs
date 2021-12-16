using Discord;

namespace MASZ.Models.Views
{
    public class DiscordChannelView
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public DiscordChannelView() { }
        public DiscordChannelView(IChannel channel)
        {
            Id = channel.Id.ToString();
            Name = channel.Name;
        }
    }
}

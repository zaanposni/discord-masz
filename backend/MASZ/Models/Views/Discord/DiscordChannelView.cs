using Discord;
using MASZ.Extensions;

namespace MASZ.Models.Views
{
    public class DiscordChannelView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public int Type { get; set; }

        public DiscordChannelView() { }
        public DiscordChannelView(IGuildChannel channel)
        {
            Id = channel.Id.ToString();
            Name = channel.Name;
            Position = channel.Position;
            Type = (int)channel.GetChannelType();
        }
    }
}

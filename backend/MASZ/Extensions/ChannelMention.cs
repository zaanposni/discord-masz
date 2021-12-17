using Discord;

namespace MASZ.Extensions
{
    public static class ChannelMention
    {
        public static string Mention(this IChannel channel)
        {
            return $"<#{channel.Id}>";
        }
    }
}
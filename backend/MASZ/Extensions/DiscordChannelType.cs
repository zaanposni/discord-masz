using Discord;

namespace MASZ.Extensions
{
    public static class DiscordChannelType
    {

        public static ChannelType GetChannelType(this IChannel channel)
        {
            return channel switch
            {
                channel is ITextChannel => ChannelType.Text
            }


        }

    }
}

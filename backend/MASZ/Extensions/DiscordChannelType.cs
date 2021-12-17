using Discord;

namespace MASZ.Extensions
{
    public static class DiscordChannelType
    {

        public static ChannelType GetChannelType(this IChannel channel)
        {
            switch (channel.GetType().GetInterfaces())
            {
                case Type[] chnl when chnl.Contains(typeof(IThreadChannel)):
                    return (channel as IThreadChannel).Type switch
                    {
                        ThreadType.PublicThread => ChannelType.PublicThread,
                        ThreadType.PrivateThread => ChannelType.PrivateThread,
                        ThreadType.NewsThread => ChannelType.NewsThread,
                        _ => throw new NotImplementedException(),
                    };
                case Type[] chnl when chnl.Contains(typeof(ITextChannel)): return ChannelType.Text;
                case Type[] chnl when chnl.Contains(typeof(IDMChannel)): return ChannelType.DM;
                case Type[] chnl when chnl.Contains(typeof(IVoiceChannel)): return ChannelType.Voice;
                case Type[] chnl when chnl.Contains(typeof(IGroupChannel)): return ChannelType.Group;
                case Type[] chnl when chnl.Contains(typeof(ICategoryChannel)): return ChannelType.Category;
                case Type[] chnl when chnl.Contains(typeof(INewsChannel)): return ChannelType.News;
                case Type[] chnl when chnl.Contains(typeof(IStageChannel)): return ChannelType.Stage;
                default: throw new NotImplementedException();
            };
    }

}
}

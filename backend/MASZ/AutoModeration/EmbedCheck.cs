using Discord;
using MASZ.Models;

namespace MASZ.AutoModeration
{
    public static class EmbedCheck
    {
        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (message.Embeds == null)
            {
                return false;
            }
            return message.Embeds.Count > config.Limit;
        }
    }
}
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class EmbedCheck
    {
        public static bool Check(DiscordMessage message, AutoModerationConfig config, DiscordClient client)
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
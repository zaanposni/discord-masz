using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class MentionCheck
    {
        public static bool Check(DiscordMessage message, AutoModerationConfig config, DiscordClient client)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (message.MentionedUsers == null)
            {
                return false;
            }
            return message.MentionedUsers.Count > config.Limit;
        }
    }
}
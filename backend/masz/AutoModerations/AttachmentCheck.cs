using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class AttachmentCheck
    {
        public static bool Check(DiscordMessage message, AutoModerationConfig config, DiscordClient client)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (message.Attachments == null)
            {
                return false;
            }
            return message.Attachments.Count > config.Limit;
        }
    }
}
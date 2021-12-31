using Discord;
using MASZ.Models;

namespace MASZ.AutoModeration
{
    public static class AttachmentCheck
    {
        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _)
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
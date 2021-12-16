using Discord;
using MASZ.Models;

namespace MASZ.AutoModeration
{
    public static class MentionCheck
    {
        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (message.MentionedUserIds == null || message.MentionedRoleIds == null)
            {
                return false;
            }
            return message.MentionedRoleIds.Count + message.MentionedUserIds.Count > config.Limit;
        }
    }
}
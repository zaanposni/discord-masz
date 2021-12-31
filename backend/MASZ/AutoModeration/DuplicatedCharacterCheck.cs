using Discord;
using MASZ.Models;
using System.Text.RegularExpressions;

namespace MASZ.AutoModeration
{
    public static class DuplicatedCharacterCheck
    {
        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }
            if (config.Limit <= 0)
            {
                return false;
            }

            Regex regexPattern = new(@"([^0-9`])(?:\s*\1){" + config.Limit.ToString() + @",}");

            return regexPattern.Match(message.Content).Success;
        }
    }
}
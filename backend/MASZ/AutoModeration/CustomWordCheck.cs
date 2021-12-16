using Discord;
using MASZ.Models;
using System.Text.RegularExpressions;

namespace MASZ.AutoModeration
{
    public static class CustomWordCheck
    {
        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(config.CustomWordFilter))
            {
                return false;
            }
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }

            int matches = 0;
            foreach (string word in config.CustomWordFilter.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(word)) continue;
                try
                {
                    matches += Regex.Matches(message.Content, word, RegexOptions.IgnoreCase).Count;
                }
                catch { }
                if (matches > config.Limit)
                {
                    break;
                }
            }
            return matches > config.Limit;
        }
    }
}
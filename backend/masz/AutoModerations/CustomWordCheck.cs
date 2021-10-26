using System;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class CustomWordCheck
    {
        public static bool Check(DiscordMessage message, AutoModerationConfig config, DiscordClient client)
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
                } catch { }
                if (matches > config.Limit)
                {
                    break;
                }
            }
            return matches > config.Limit;
        }
    }
}
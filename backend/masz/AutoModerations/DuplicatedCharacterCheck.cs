using System;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class DuplicatedCharacterCheck
    {
        public static bool Check(DiscordMessage message, AutoModerationConfig config, DiscordClient client)
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

            Regex regexPattern = new Regex(@"([\D\s])(?:\s*\1){" + config.Limit.ToString() + @",}");

            return regexPattern.Match(message.Content).Success;
        }
    }
}
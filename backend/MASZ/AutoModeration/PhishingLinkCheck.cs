using Discord;
using MASZ.Models;
using MASZ.Services;
using System.Text.RegularExpressions;

namespace MASZ.AutoModeration
{
    public static class PhishingLinkCheck
    {
        private static readonly Regex _domainRegex = new(@"https?:\/\/([-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,4}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*))");

        public static bool Check(IMessage message, AutoModerationConfig config, IDiscordClient _, PhishingDetectionService phishingDetectionService)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }

            var foundLinks = _domainRegex.Matches(message.Content);
            int count = foundLinks.Count;
            foreach (Match link in foundLinks)
            {
                if (!phishingDetectionService.Checkstring(link.Groups.Values.Skip(1).First().Value.ToString().Trim()))
                {
                    count--;
                    continue;
                }
            }

            return count > config.Limit;
        }
    }
}
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class LinkCheck
    {
        private static readonly Regex _domainRegex = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,4}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)");

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

            var foundLinks = _domainRegex.Matches(message.Content);
            int count = foundLinks.Count;
            if (! string.IsNullOrEmpty(config.CustomWordFilter))
            {
                foreach (Match link in foundLinks)
                {
                    foreach (var filtered in config.CustomWordFilter.Split('\n'))
                    {
                        Regex _filteredRegex = new Regex(filtered);
                        if (_filteredRegex.IsMatch(link.Value))
                        {
                            count--;
                            break;
                        }
                    }
                }
            }

            return count > config.Limit;
        }
    }
}
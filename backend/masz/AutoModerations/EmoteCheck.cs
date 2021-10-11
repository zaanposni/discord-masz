using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Models;

namespace masz.AutoModerations
{
    public static class EmoteCheck
    {
        private static readonly Regex _emoteRegex = new Regex(@"(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])");
        private static readonly Regex _customEmoteRegex = new Regex(@"<a?:\w*:\d*>");
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

            int customEmotes = _customEmoteRegex.Matches(message.Content).Count;
            if (customEmotes > config.Limit)  // skip normal emote check if possible
            {
                return true;
            }

            int emotes = _emoteRegex.Matches(message.Content).Count;

            return (emotes + customEmotes) > config.Limit;
        }
    }
}
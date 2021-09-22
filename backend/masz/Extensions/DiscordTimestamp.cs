using System;
using masz.Enums;

namespace masz.Extensions
{
    public static class DiscordTimestamp
    {
        public static string GetDiscordTS(this DateTime dateTime, DiscordTimestampFormat format = DiscordTimestampFormat.Default)
        {
            return $"<t:{((DateTimeOffset)dateTime).ToUnixTimeSeconds()}:{(char) format}>";
        }
    }
}
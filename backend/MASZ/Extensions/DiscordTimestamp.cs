using MASZ.Enums;

namespace MASZ.Extensions
{
    public static class DiscordTimestamp
    {
        public static string ToDiscordTS(this DateTime dateTime, DiscordTimestampFormat format = DiscordTimestampFormat.Default)
        {
            return $"<t:{((DateTimeOffset)dateTime).ToUnixTimeSeconds()}:{(char)format}>";
        }
    }
}
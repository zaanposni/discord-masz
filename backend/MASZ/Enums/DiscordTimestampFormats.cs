namespace MASZ.Enums
{
    // Timestamp formats: https://discord.com/developers/docs/reference#message-formatting-formats
    public enum DiscordTimestampFormat
    {
        ShortTime = 't',
        LongTime = 'T',
        ShortDate = 'd',
        LongDate = 'D',
        ShortDateTime = 'f',
        LongDateTime = 'F',
        RelativeTime = 'R',
    }
}
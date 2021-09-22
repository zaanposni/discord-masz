namespace masz.Enums
{
    /*

    enum to quickly access characters of discords timestamp formatting
    https://discord.com/developers/docs/reference#message-formatting-formats

    */
    public enum DiscordTimestampFormat
    {
        ShortTime = 't',
        LongTime = 'T',
        ShortDate = 'd',
        LongDate = 'D',
        ShortDateTime = 'f',
        Default = 'f',
        LongDateTime = 'F',
        RelativeTime = 'R',
    }
}
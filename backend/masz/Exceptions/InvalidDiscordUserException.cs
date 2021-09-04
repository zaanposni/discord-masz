namespace masz.Exceptions
{
    public class InvalidDiscordUserException : BaseAPIException
    {
        public ulong UserId { get; set; }
        public InvalidDiscordUserException(string message, ulong userId) : base(message)
        {
            UserId = userId;
        }
        public InvalidDiscordUserException(ulong userId) : base("Failed to fetch user from API.")
        {
            UserId = userId;
        }
        public InvalidDiscordUserException() : base("Failed to fetch user from API.")
        {
        }
    }
}
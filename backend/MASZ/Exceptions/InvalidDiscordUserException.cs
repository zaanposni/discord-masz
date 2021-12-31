using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class InvalidIUserException : BaseAPIException
    {
        public ulong UserId { get; set; }
        public InvalidIUserException(string message, ulong userId) : base(message, APIError.InvalidDiscordUser)
        {
            UserId = userId;
        }
        public InvalidIUserException(ulong userId) : base("Failed to fetch user '{userId}' from API.", APIError.InvalidDiscordUser)
        {
            UserId = userId;
        }
        public InvalidIUserException() : base("Failed to fetch user from API.", APIError.InvalidDiscordUser)
        {
        }
    }
}
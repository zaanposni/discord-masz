using masz.Enums;

namespace masz.Exceptions
{
    public class InvalidUserMapException : BaseAPIException
    {
        public InvalidUserMapException(string message) : base(message, APIError.CannotBeSameUser)
        {
        }
        public InvalidUserMapException() : base("Cannot create usermap for same user.", APIError.CannotBeSameUser)
        {
        }
    }
}
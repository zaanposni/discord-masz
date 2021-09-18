using masz.Enums;

namespace masz.Exceptions
{
    public class InvalidPathException : BaseAPIException
    {
        public InvalidPathException(string message) : base(message, APIError.InvalidFilePath)
        {
        }
        public InvalidPathException() : base("Invalid identity (token) encountered.", APIError.InvalidFilePath)
        {
        }
    }
}
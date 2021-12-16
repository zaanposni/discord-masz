using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class InvalidPathException : BaseAPIException
    {
        public InvalidPathException(string message) : base(message, APIError.InvalidFilePath)
        {
        }
        public InvalidPathException() : base("Invalid file path provided.", APIError.InvalidFilePath)
        {
        }
    }
}
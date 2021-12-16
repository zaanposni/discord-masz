using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class InvalidIdentityException : BaseAPIException
    {
        public string Token { get; set; }
        public InvalidIdentityException(string message, string token) : base(message, APIError.InvalidIdentity)
        {
            Token = token;
        }
        public InvalidIdentityException(string token) : base("Invalid identity (token) encountered.", APIError.InvalidIdentity)
        {
            Token = token;
        }
        public InvalidIdentityException() : base("Invalid identity (token) encountered.", APIError.InvalidIdentity)
        {
        }
    }
}
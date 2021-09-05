using masz.Enums;

namespace masz.Exceptions
{
    public class UnauthorizedException : BaseAPIException
    {
        public UnauthorizedException(string message) : base(message, APIError.Unauthorized)
        {
        }
        public UnauthorizedException() : base("You are not allowed to do that", APIError.Unauthorized)
        {
        }
    }
}
using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class BaseAPIException : BaseException
    {
        public APIError Error { get; set; } = APIError.Unknown;
        public BaseAPIException(string? message) : base(message)
        {
        }

        public BaseAPIException(string? message, APIError error) : base(message)
        {
            Error = error;
        }

        public BaseAPIException(APIError error) : base(null)
        {
            Error = error;
        }
        public BaseException WithError(APIError error)
        {
            Error = error;
            return this;
        }
    }
}
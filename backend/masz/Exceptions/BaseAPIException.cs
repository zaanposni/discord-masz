#nullable enable

using masz.Enums;

namespace masz.Exceptions
{
    public class BaseAPIException : BaseException
    {
        public APIError error { get; set; } = APIError.Unknown;
        public BaseAPIException(string? message) : base(message)
        {
        }

        public BaseAPIException(string? message, APIError error) : base(message)
        {
            this.error = error;
        }

        public BaseAPIException(APIError error) : base(null)
        {
            this.error = error;
        }
        public BaseException WithError(APIError error)
        {
            this.error = error;
            return this;
        }
    }
}
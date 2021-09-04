#nullable enable

using masz.Enums;

namespace masz.Exceptions
{
    public class BaseAPIException : BaseException
    {
        public APIError error { get; set; }
        public BaseAPIException(string? message) : base(message)
        {
        }
        public BaseException WithError(APIError error)
        {
            this.error = error;
            return this;
        }
    }
}
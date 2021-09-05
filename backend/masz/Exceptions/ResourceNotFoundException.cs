using masz.Enums;

namespace masz.Exceptions
{
    public class ResourceNotFoundException : BaseAPIException
    {
        public ResourceNotFoundException(string message) : base(message, APIError.ResourceNotFound)
        {
        }
        public ResourceNotFoundException() : base(null, APIError.ResourceNotFound)
        {
        }
    }
}
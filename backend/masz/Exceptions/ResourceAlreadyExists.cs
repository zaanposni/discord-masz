using masz.Enums;

namespace masz.Exceptions
{
    public class ResourceAlreadyExists : BaseAPIException
    {
        public ResourceAlreadyExists(string message) : base(message, APIError.ResourceAlreadyExists)
        {
        }
        public ResourceAlreadyExists() : base("Resource already exists.", APIError.ResourceAlreadyExists)
        {
        }
    }
}
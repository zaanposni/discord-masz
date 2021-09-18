using masz.Enums;

namespace masz.Exceptions
{
    public class TooManyTemplatesCreatedException : BaseAPIException
    {
        public TooManyTemplatesCreatedException(string message) : base(message, APIError.TooManyTemplates)
        {
        }
        public TooManyTemplatesCreatedException() : base("Too many templates created.", APIError.TooManyTemplates)
        {
        }
    }
}
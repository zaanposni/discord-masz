using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class InvalidDateForScheduledMessageException : BaseAPIException
    {
        public InvalidDateForScheduledMessageException(string message) : base(message, APIError.InvalidDateForScheduledMessage)
        {
        }
        public InvalidDateForScheduledMessageException() : base("The defined execution day of this message is invalid.", APIError.InvalidDateForScheduledMessage)
        {
        }
    }
}

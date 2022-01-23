using MASZ.Enums;
using MASZ.Models;


namespace MASZ.Exceptions
{
    public class ProtectedScheduledMessageException : BaseAPIException
    {
        public ScheduledMessage ScheduledMessage { get; set; }
        public ProtectedScheduledMessageException(string message, ScheduledMessage scheduledMessage) : base(message, APIError.ProtectedScheduledMessage)
        {
            ScheduledMessage = scheduledMessage;
        }
        public ProtectedScheduledMessageException(ScheduledMessage scheduledMessage) : base("Message is protected.", APIError.ProtectedScheduledMessage)
        {
            ScheduledMessage = scheduledMessage;
        }
        public ProtectedScheduledMessageException() : base("Message is protected.", APIError.ProtectedScheduledMessage)
        {
        }
    }
}

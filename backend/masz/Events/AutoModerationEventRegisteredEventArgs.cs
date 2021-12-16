using MASZ.Models;

namespace MASZ.Events
{
    public class AutoModerationEventRegisteredEventArgs : EventArgs
    {
        private readonly AutoModerationEvent _event;

        public AutoModerationEventRegisteredEventArgs(AutoModerationEvent autoModerationEvent)
        {
            _event = autoModerationEvent;
        }

        public AutoModerationEvent GetEvent()
        {
            return _event;
        }
    }
}
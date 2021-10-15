using System;
using masz.Models;

namespace masz.Events
{
    public class AutoModerationEventRegisteredEventArgs : EventArgs
    {
        private AutoModerationEvent _event;

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
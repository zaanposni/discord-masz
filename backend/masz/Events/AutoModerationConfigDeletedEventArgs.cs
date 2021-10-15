using System;
using masz.Models;

namespace masz.Events
{
    public class AutoModerationConfigDeletedEventArgs : EventArgs
    {
        private AutoModerationConfig _config;

        public AutoModerationConfigDeletedEventArgs(AutoModerationConfig config)
        {
            _config = config;
        }

        public AutoModerationConfig GetConfig()
        {
            return _config;
        }
    }
}
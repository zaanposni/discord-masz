using System;
using masz.Models;

namespace masz.Events
{
    public class AutoModerationConfigUpdatedEventArgs : EventArgs
    {
        private AutoModerationConfig _config;

        public AutoModerationConfigUpdatedEventArgs(AutoModerationConfig config)
        {
            _config = config;
        }

        public AutoModerationConfig GetConfig()
        {
            return _config;
        }
    }
}
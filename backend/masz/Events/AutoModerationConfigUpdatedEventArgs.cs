using MASZ.Models;

namespace MASZ.Events
{
    public class AutoModerationConfigUpdatedEventArgs : EventArgs
    {
        private readonly AutoModerationConfig _config;

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
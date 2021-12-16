using MASZ.Models;

namespace MASZ.Events
{
    public class AutoModerationConfigDeletedEventArgs : EventArgs
    {
        private readonly AutoModerationConfig _config;

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
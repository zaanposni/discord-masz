using MASZ.Models;

namespace MASZ.Events
{
    public class GuildLevelAuditLogConfigUpdatedEventArgs : EventArgs
    {
        private readonly GuildLevelAuditLogConfig _config;

        public GuildLevelAuditLogConfigUpdatedEventArgs(GuildLevelAuditLogConfig config)
        {
            _config = config;
        }

        public GuildLevelAuditLogConfig GetConfig()
        {
            return _config;
        }
    }
}
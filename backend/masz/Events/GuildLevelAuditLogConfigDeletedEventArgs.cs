using MASZ.Models;

namespace MASZ.Events
{
    public class GuildLevelAuditLogConfigDeletedEventArgs : EventArgs
    {
        private readonly GuildLevelAuditLogConfig _config;

        public GuildLevelAuditLogConfigDeletedEventArgs(GuildLevelAuditLogConfig config)
        {
            _config = config;
        }

        public GuildLevelAuditLogConfig GetConfig()
        {
            return _config;
        }
    }
}
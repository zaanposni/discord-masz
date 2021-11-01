using System;
using masz.Models;

namespace masz.Events
{
    public class GuildLevelAuditLogConfigDeletedEventArgs : EventArgs
    {
        private GuildLevelAuditLogConfig _config;

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
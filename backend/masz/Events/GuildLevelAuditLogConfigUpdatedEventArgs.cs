using System;
using masz.Models;

namespace masz.Events
{
    public class GuildLevelAuditLogConfigUpdatedEventArgs : EventArgs
    {
        private GuildLevelAuditLogConfig _config;

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
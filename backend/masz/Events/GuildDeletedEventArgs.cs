using System;
using masz.Models;

namespace masz.Events
{
    public class GuildDeletedEventArgs : EventArgs
    {
        private GuildConfig _guildConfig;

        public GuildDeletedEventArgs(GuildConfig guildConfig)
        {
            _guildConfig = guildConfig;
        }

        public GuildConfig GetGuildConfig()
        {
            return _guildConfig;
        }
    }
}
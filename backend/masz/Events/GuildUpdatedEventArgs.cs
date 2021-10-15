using System;
using masz.Models;

namespace masz.Events
{
    public class GuildUpdatedEventArgs : EventArgs
    {
        private GuildConfig _guildConfig;

        public GuildUpdatedEventArgs(GuildConfig guildConfig)
        {
            _guildConfig = guildConfig;
        }

        public GuildConfig GetGuildConfig()
        {
            return _guildConfig;
        }
    }
}
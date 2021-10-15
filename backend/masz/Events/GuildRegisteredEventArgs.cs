using System;
using masz.Models;

namespace masz.Events
{
    public class GuildRegisteredEventArgs : EventArgs
    {
        private GuildConfig _guildConfig;

        public GuildRegisteredEventArgs(GuildConfig guildConfig)
        {
            _guildConfig = guildConfig;
        }

        public GuildConfig GetGuildConfig()
        {
            return _guildConfig;
        }
    }
}
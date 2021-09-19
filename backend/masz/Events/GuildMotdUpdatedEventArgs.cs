using System;
using masz.Models;

namespace masz.Events
{
    public class GuildMotdUpdatedEventArgs : EventArgs
    {
        private GuildMotd _motd;

        public GuildMotdUpdatedEventArgs(GuildMotd motd)
        {
            _motd = motd;
        }

        public GuildMotd GetGuildMotd()
        {
            return _motd;
        }
    }
}
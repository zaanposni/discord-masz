using MASZ.Models;

namespace MASZ.Events
{
    public class GuildMotdUpdatedEventArgs : EventArgs
    {
        private readonly GuildMotd _motd;

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
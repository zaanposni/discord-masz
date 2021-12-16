using MASZ.Models;

namespace MASZ.Events
{
    public class GuildDeletedEventArgs : EventArgs
    {
        private readonly GuildConfig _guildConfig;

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
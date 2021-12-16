using MASZ.Models;

namespace MASZ.Events
{
    public class GuildRegisteredEventArgs : EventArgs
    {
        private readonly GuildConfig _guildConfig;

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
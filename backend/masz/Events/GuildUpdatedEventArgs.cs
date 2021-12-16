using MASZ.Models;

namespace MASZ.Events
{
    public class GuildUpdatedEventArgs : EventArgs
    {
        private readonly GuildConfig _guildConfig;

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
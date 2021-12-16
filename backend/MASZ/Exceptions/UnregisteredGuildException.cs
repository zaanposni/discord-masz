using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class UnregisteredGuildException : BaseAPIException
    {
        public ulong GuildId { get; set; }
        public UnregisteredGuildException(string message, ulong guildId) : base(message, APIError.GuildUnregistered)
        {
            GuildId = guildId;
        }
        public UnregisteredGuildException(ulong guildId) : base($"Guild {guildId} is not registered.", APIError.GuildUnregistered)
        {
            GuildId = guildId;
        }
        public UnregisteredGuildException() : base("Guild is not registered.", APIError.GuildUnregistered)
        {
        }
    }
}
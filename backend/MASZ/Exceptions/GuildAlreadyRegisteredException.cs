using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class GuildAlreadyRegisteredException : BaseAPIException
    {
        public ulong GuildId { get; set; }
        public GuildAlreadyRegisteredException(string message, ulong guildId) : base(message, APIError.GuildUnregistered)
        {
            GuildId = guildId;
        }
        public GuildAlreadyRegisteredException(ulong guildId) : base($"Guild {guildId} is already registered.", APIError.GuildUnregistered)
        {
            GuildId = guildId;
        }
        public GuildAlreadyRegisteredException() : base("Guild is already registered.", APIError.GuildUnregistered)
        {
        }
    }
}
using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class GuildWithoutMutedRoleException : BaseAPIException
    {
        public ulong GuildId { get; set; }
        public GuildWithoutMutedRoleException(string message, ulong guildId) : base(message, APIError.GuildUndefinedMutedRoles)
        {
            GuildId = guildId;
        }
        public GuildWithoutMutedRoleException(ulong guildId) : base($"Guild {guildId} has no muteroles defined.", APIError.GuildUndefinedMutedRoles)
        {
            GuildId = guildId;
        }
        public GuildWithoutMutedRoleException() : base("Guild has no muteroles defined.", APIError.GuildUndefinedMutedRoles)
        {
        }
    }
}
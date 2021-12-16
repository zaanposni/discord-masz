using MASZ.Enums;

namespace MASZ.Exceptions
{
    public class RoleNotFoundException : BaseAPIException
    {
        public ulong RoleId { get; set; }
        public RoleNotFoundException(string message, ulong roleId) : base(message, APIError.GuildUnregistered)
        {
            RoleId = roleId;
        }
        public RoleNotFoundException(ulong roleId) : base($"Role {roleId} not found.", APIError.RoleNotFound)
        {
            RoleId = roleId;
        }
        public RoleNotFoundException() : base("Role not found.", APIError.RoleNotFound)
        {
        }
    }
}
using Discord;
using MASZ.Models;

namespace MASZ.Services
{
    public interface IIdentityManager
    {
        Task<Identity> GetIdentity(HttpContext httpContext);
        Task<Identity> GetIdentity(IUser user);
        List<Identity> GetCurrentIdentities();
        void ClearAllIdentities();
        void ClearOldIdentities();
        void ClearTokenIdentities();
        Task<Identity> GetIdentityByUserId(ulong userId);
    }
}

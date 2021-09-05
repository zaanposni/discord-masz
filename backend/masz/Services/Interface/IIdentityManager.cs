using DSharpPlus.Entities;
using masz.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IIdentityManager
    {
        Task<Identity> GetIdentity(HttpContext httpContext);
        Task<Identity> GetIdentity(DiscordUser user);
        List<Identity> GetCurrentIdentities();
        void ClearAllIdentities();
        void ClearOldIdentities();
        void ClearTokenIdentities();
        Task<Identity> GetIdentityByUserId(ulong userId);
    }
}

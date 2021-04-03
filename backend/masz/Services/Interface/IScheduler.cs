using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface IScheduler
    {
        void StartTimers();
        void CacheAll();
        Task<List<string>> CacheAllGuildBans(List<string> handledUsers);
        Task<List<string>> CacheAllGuildMembers(List<string> handledUser);
        Task<List<string>> CacheAllKnownUsers(List<string> handledUser);
        Task CacheAllKnownGuilds();
        void CheckDeletedCases();
    }
}
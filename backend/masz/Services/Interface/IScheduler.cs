using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface IScheduler
    {
        void StartTimers();
        void CacheAll();
        Task<List<string>> CacheAllGuildMembers();
        Task<List<string>> CacheAllKnownUsers(List<string> handledUser);
        void CheckDeletedCases();
    }
}
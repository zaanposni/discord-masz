using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface ICacher
    {
        void StartTimer();
        void CacheAll();
        Task<List<string>> CacheAllGuildMembers();
        Task<List<string>> CacheAllKnownUsers(List<string> handledUser);
    }
}
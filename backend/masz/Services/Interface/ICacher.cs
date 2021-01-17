using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface ICacher
    {
        void StartTimer();
        void CacheAll();
        Task CacheAllGuildMembers();
        Task CacheAllKnownUsers();
    }
}
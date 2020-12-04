using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface IPunishmentHandler
    {
        void StartTimer();
        void CheckAllCurrentPunishments();
        Task ExecutePunishment(ModCase modCase, IDatabase database);
        Task UndoPunishment(ModCase modCase, IDatabase database);
    }
}
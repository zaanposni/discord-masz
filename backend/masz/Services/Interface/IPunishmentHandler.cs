using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface IPunishmentHandler
    {
        void StartTimer();
        void CheckAllCurrentPunishments();
        Task ExecutePunishment(ModCase modCase);
        Task UndoPunishment(ModCase modCase);
    }
}
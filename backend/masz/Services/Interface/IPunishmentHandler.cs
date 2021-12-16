using Discord.WebSocket;
using MASZ.Models;

namespace MASZ.Services
{
    public interface IPunishmentHandler
    {
        void StartTimer();
        void CheckAllCurrentPunishments();
        Task ExecutePunishment(ModCase modCase);
        Task UndoPunishment(ModCase modCase);
        Task HandleMemberJoin(SocketGuildUser user);
    }
}
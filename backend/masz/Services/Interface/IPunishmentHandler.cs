using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using masz.Models;

namespace masz.Services
{
    public interface IPunishmentHandler
    {
        void StartTimer();
        void CheckAllCurrentPunishments();
        Task ExecutePunishment(ModCase modCase);
        Task UndoPunishment(ModCase modCase);
        Task HandleMemberJoin(DiscordClient client, GuildMemberAddEventArgs e);
    }
}
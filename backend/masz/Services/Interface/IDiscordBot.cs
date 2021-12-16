using Discord.WebSocket;

namespace MASZ.Services
{
    public interface IDiscordBot
    {
        Task Start();
        DiscordSocketClient GetClient();
        bool IsRunning();
        DateTime? GetLastDisconnectTime();
        int GetPing();
    }
}
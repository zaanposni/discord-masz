using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace masz.Services
{
    public interface IDiscordBot
    {
        Task Start();
        DiscordClient GetClient();
        bool IsRunning();
        DateTime? GetLastDisconnectTime();
    }
}
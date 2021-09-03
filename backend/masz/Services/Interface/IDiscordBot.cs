using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace masz.Services
{
    public interface IDiscordBot
    {
        Task Start();
    }
}
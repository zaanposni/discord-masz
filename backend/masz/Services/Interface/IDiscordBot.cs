using System.Threading.Tasks;
using DSharpPlus;

namespace masz.Services
{
    public interface IDiscordBot
    {
        Task Start();
        DiscordClient GetClient();
    }
}
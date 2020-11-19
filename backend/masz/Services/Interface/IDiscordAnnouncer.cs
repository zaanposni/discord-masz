using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface IDiscordAnnouncer
    {
        Task AnnounceModCase(ModCase modCase, RestAction action, bool announcePublic);
        Task AnnounceComment(ModCaseComment comment, RestAction action);
    }
}

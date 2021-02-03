using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;

namespace masz.Services
{
    public interface IDiscordAnnouncer
    {
        Task AnnounceModCase(ModCase modCase, RestAction action, User actor, bool announcePublic, bool announceDm);
        Task AnnounceComment(ModCaseComment comment, User actor, RestAction action);
        Task AnnounceFile(string filename, ModCase modCase, User actor, RestAction action);
    }
}

using System.Threading.Tasks;
using masz.Models;

namespace masz.Services
{
    public interface IModCaseAnnouncer
    {
        Task AnnounceModCase(ModCase modCase, ModCaseAction action, bool announcePublic);
    }
}

using System.Threading.Tasks;
using masz.Models;
using masz.Translations;

namespace masz.Services
{
    public interface ITranslator
    {
        Task SetContext(string guildId);
        void SetContext(GuildConfig guildConfig);
        void SetContext(Language guildConfig);
        Translation T();
        Translation T(GuildConfig guildConfig);
        Translation T(Language guildConfig);
    }
}
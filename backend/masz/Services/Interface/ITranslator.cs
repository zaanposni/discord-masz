using System.Threading.Tasks;
using masz.Models;
using masz.Translations;

namespace masz.Services
{
    public interface ITranslator
    {
        Task SetContext(ulong guildId);
        void SetContext(GuildConfig guildConfig);
        void SetContext(Language? language);
        Translation T();
        Translation T(GuildConfig guildConfig);
        Translation T(Language language);
        Translation T(Language? language);
        Language GetLanguage();
    }
}
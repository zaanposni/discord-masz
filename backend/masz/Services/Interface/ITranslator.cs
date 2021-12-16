using MASZ.Enums;
using MASZ.Models;
using MASZ.Translations;

namespace MASZ.Services
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
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Translations;

namespace MASZ.Services
{
    public class Translator : ITranslator
    {
        private readonly IInternalConfiguration _config;
        private readonly Translation _translation;
        private readonly IServiceProvider _serviceProvider;

        public Translator() { }

        public Translator(IInternalConfiguration config, IServiceProvider serviceProvider)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _translation = Translation.Ctx(_config.GetDefaultLanguage());
        }

        public async Task SetContext(ulong guildId)
        {
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
            SetContext(guildConfig);
        }

        public void SetContext(GuildConfig guildConfig)
        {
            if (guildConfig != null)
            {
                SetContext(guildConfig.PreferredLanguage);
            }
        }

        public void SetContext(Language? language)
        {
            if (language != null)
            {
                _translation.PreferredLanguage = language.Value;
            }
        }

        public Translation T()
        {
            return _translation;
        }

        public Translation T(GuildConfig guildConfig)
        {
            SetContext(guildConfig);
            return _translation;
        }

        public Translation T(Language language)
        {
            SetContext(language);
            return _translation;
        }

        public Translation T(Language? language)
        {
            if (language.HasValue)
            {
                SetContext(language.Value);
            }
            return _translation;
        }

        public Language GetLanguage()
        {
            return _translation.PreferredLanguage;
        }
    }
}
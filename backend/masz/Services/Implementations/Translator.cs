using System;
using System.Threading.Tasks;
using masz.Models;
using masz.Repositories;
using masz.Translations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class Translator : ITranslator
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly IOptions<InternalConfig> _config;
        private readonly Translation _translation;
        private readonly IServiceProvider _serviceProvider;

        public Translator() { }

        public Translator(ILogger<Scheduler> logger, IOptions<InternalConfig> config, IDatabase context, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = config;
            _serviceProvider = serviceProvider;
            Language useLanguage = Language.en;
            switch (config.Value.DefaultLanguage) {
                case "de":
                    useLanguage = Language.de;
                    break;
                case "it":
                    useLanguage = Language.it;
                    break;
                case "fr":
                    useLanguage = Language.fr;
                    break;
                case "es":
                    useLanguage = Language.es;
                    break;
            }
            _translation = Translation.Ctx(useLanguage);
        }

        public async Task SetContext(ulong guildId)
        {
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);
            SetContext(guildConfig);
        }

        public void SetContext(GuildConfig guildConfig)
        {
            if (guildConfig != null) {
                SetContext(guildConfig.PreferredLanguage);
            }
        }

        public void SetContext(Language language)
        {
            _translation.preferredLanguage = language;
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
            if (language.HasValue) {
                SetContext(language.Value);
            }
            return _translation;
        }

        public Language GetLanguage()
        {
            return _translation.preferredLanguage;
        }
    }
}
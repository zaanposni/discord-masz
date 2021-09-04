using System.Threading.Tasks;
using masz.Models;
using masz.Translations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class Translator : ITranslator
    {
        private readonly ILogger<Scheduler> logger;
        private readonly IOptions<InternalConfig> config;
        private readonly IDatabase context;
        private readonly Translation translation;

        public Translator() { }

        public Translator(ILogger<Scheduler> logger, IOptions<InternalConfig> config, IDatabase context)
        {
            this.logger = logger;
            this.config = config;
            this.context = context;
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
            this.translation = Translation.Ctx(useLanguage);
        }

        public async Task SetContext(ulong guildId)
        {
            GuildConfig guildConfig = await this.context.SelectSpecificGuildConfig(guildId);
            this.SetContext(guildConfig);
        }

        public void SetContext(GuildConfig guildConfig)
        {
            if (guildConfig != null) {
                this.SetContext(guildConfig.PreferredLanguage);
            }
        }

        public void SetContext(Language language)
        {
            this.translation.preferredLanguage = language;
        }

        public Translation T()
        {
            return this.translation;
        }

        public Translation T(GuildConfig guildConfig)
        {
            this.SetContext(guildConfig);
            return this.translation;
        }

        public Translation T(Language language)
        {
            this.SetContext(language);
            return this.translation;
        }

        public Translation T(Language? language)
        {
            if (language.HasValue) {
                this.SetContext(language.Value);
            }
            return this.translation;
        }

        public Language GetLanguage()
        {
            return this.translation.preferredLanguage;
        }
    }
}
using Discord.Interactions;
using MASZ.Enums;

namespace MASZ.Utils
{
    public class TranslationManager : ILocalizationManager
    {
        private readonly Translation _translation = Translation.Ctx();

        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType)
        {
            IEnumerable<string> keys = key.Prepend("commands").Append("desc");
            Dictionary<string, string> result = new Dictionary<string, string>();

            // foreach language
            foreach (Language language in (Language[]) Enum.GetValues(typeof(Language)))
            {
                _translation.PreferredLanguage = language;
                string[] discordLocales = LocaleHelper.MASZLocaleToDiscordLocals(language);
                foreach (string discordLocale in discordLocales)
                {
                    string translation = _translation.GetByJsonPath(String.Join(".", keys));
                    if (translation != "Unknown") {
                        result[discordLocale] = translation;
                    }
                }
            }

            return result;
        }

        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType)
        {
            IEnumerable<string> keys = key.Prepend("commands").Append("name");
            Dictionary<string, string> result = new Dictionary<string, string>();

            // foreach language
            foreach (Language language in (Language[]) Enum.GetValues(typeof(Language)))
            {
                _translation.PreferredLanguage = language;
                string[] discordLocales = LocaleHelper.MASZLocaleToDiscordLocals(language);
                foreach (string discordLocale in discordLocales)
                {
                    string translation = _translation.GetByJsonPath(String.Join(".", keys));
                    if (translation != "Unknown") {
                        result[discordLocale] = translation.ToLower();
                    }
                }
            }

            return result;
        }
    }
}
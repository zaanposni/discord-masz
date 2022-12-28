using MASZ.Enums;

namespace MASZ.Utils
{
    public class LocaleHelper
    {
        public static Language? DiscordLocaleToMASZLocale(string discordLocale)
        {
            switch (discordLocale)
            {
                case "en":
                case "en-US":
                case "en-GB":
                    return Language.en;
                case "de":
                    return Language.de;
                case "fr":
                    return Language.fr;
                case "es":
                case "es-ES":
                    return Language.es;
                case "it":
                    return Language.it;
                case "at":
                    return Language.at;
                case "ru":
                    return Language.ru;
                default:
                    return null;
            }
        }

        public static string[] MASZLocaleToDiscordLocals(Language language)
        {
            switch (language)
            {
                case Language.en:
                    return new string[] { "en-US", "en-GB" };
                case Language.de:
                    return new string[] { "de" };
                case Language.fr:
                    return new string[] { "fr" };
                case Language.es:
                    return new string[] { "es-ES" };
                case Language.it:
                    return new string[] { "it" };
                case Language.at:
                    return new string[] { "de" };  // discord does not have austria locale
                case Language.ru:
                    return new string[] { "ru" };
                default:
                    return new string[] { };
            }
        }
    }
}
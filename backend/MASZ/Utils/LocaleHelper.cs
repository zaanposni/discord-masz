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
    }
}
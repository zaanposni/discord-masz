using Discord.Interactions;
using MASZ.Services;

namespace MASZ.Utils
{
    public class TranslationManager : ILocalizationManager
    {
        private readonly Translation _translation = Translation.Ctx();


        //BASE_TYPE_CHOICES: Value must be one of ('zh-CN', 'no', 'it', 'fi', 'hr', 'th', 'pl', 'el',
        // 'ar', 'ko', 'ru', 'fr', 'bg', 'he', 'en-US', 'lt', 'uk', 'hu', 'nl', 'sv-SE', 'vi', 'es-ES',
        // 'de', 'hi', 'ja', 'id', 'da', 'cs', 'ro', 'zh-TW', 'en-GB', 'tr', 'pt-BR').
        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType)
        {
            Console.WriteLine("GetAllDescriptions");
            Console.WriteLine(String.Join(", ", key));
            Console.WriteLine(destinationType);

            if (key[0] == "url") {
                return new Dictionary<string, string>() {
                    { "en-US", "Displays the URL MASZ is deployed on." },
                    { "de", "Zeigt die URL an, auf der MASZ bereitgestellt ist." }
                };
            }

            return new Dictionary<string, string>();
        }

        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType)
        {
            Console.WriteLine("GetAllNames");
            Console.WriteLine(String.Join(", ", key));
            Console.WriteLine(destinationType);

            return new Dictionary<string, string>();
        }
    }
}
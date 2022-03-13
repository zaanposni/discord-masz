using MASZ.Enums;

namespace MASZ.Models
{
    public class AppSettings
    {
        public int Id { get; set; }
        public string EmbedTitle { get; set; }
        public string EmbedContent { get; set; }
        public bool EmbedShowIcon { get; set; }
        public Language DefaultLanguage { get; set; }
        public string AuditLogWebhookURL { get; set; }
        public bool PublicFileMode { get; set; }

        public string GetEmbedData(string url, string iconUrl)
        {
            return
                "<html>" +
                    "<head>" +
                        "<meta name=\"theme-color\" content=\"#3498db\">" +
                        "<meta property=\"og:site_name\" content=\"MASZ by zaanposni\" />" +
                        "<meta property=\"og:title\" content=\"" + EmbedTitle + "\" />" +
                        "<meta property=\"og:url\" content=\"" + url + "\" />" +
                        (EmbedShowIcon && ! string.IsNullOrWhiteSpace(iconUrl) ? "<meta property=\"og:image\" content=\"" + iconUrl + "\" />" : "") +
                        (string.IsNullOrWhiteSpace(EmbedContent) ? "" : "<meta property=\"og:description\" content=\"" + EmbedContent + "\" />") +
                    "</head>" +
                "</html>";
        }

        public static AppSettings CreateDefault()
        {
            return new AppSettings()
            {
                EmbedTitle = "MASZ - a discord moderation bot",
                EmbedContent = "MASZ is a moderation bot for Discord Moderators. Keep track of all moderation events on your server, search reliably for infractions or setup automoderation to be one step ahead of trolls and rule breakers.",
                AuditLogWebhookURL = string.Empty,
                DefaultLanguage = Language.en,
                PublicFileMode = false
            };
        }
    }
}

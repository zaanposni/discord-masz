namespace MASZ.Models
{
    public class AppSettings
    {
        public int Id { get; set; }
        public string EmbedTitle { get; set; }
        public string EmbedContent { get; set; }

        public string GetEmbedData(string url)
        {
            return
                "<html>" +
                    "<head>" +
                        "<meta property=\"og:site_name\" content=\"MASZ by zaanposni\" />" +
                        "<meta property=\"og:title\" content=\"" + EmbedTitle + "\" />" +
                        "<meta property=\"og:url\" content=\"" + url + "\" />" +
                        "<meta property=\"og:description\" content=\"" + EmbedContent + "\" />" +
                    "</head>" +
                "</html>";
        }

        public static AppSettings CreateDefault()
        {
            return new AppSettings()
            {
                EmbedTitle = "MASZ - a discord moderation bot",
                EmbedContent = "MASZ is a moderation bot for Discord Moderators. Keep track of all moderation events on your server, search reliably for infractions or setup automoderation to be one step ahead of trolls and rule breakers."
            };
        }
    }
}

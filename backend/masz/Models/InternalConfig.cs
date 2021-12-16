namespace MASZ.Models
{
    public class InternalConfig
    {
        public string DiscordBotToken { get; set; }
        public string Version { get; set; }
        public string IDiscordClientId { get; set; }
        public string IDiscordClientSecret { get; set; }
        public string AbsolutePathToFileUpload { get; set; }
        public string ServiceHostName { get; set; }
        public string ServiceBaseUrl { get; set; }
        public List<ulong> SiteAdminIUserIds { get; set; }
        public string DefaultLanguage { get; set; }
    }
}

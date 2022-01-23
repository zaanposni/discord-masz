using MASZ.Enums;

namespace MASZ.Services
{
    public class InternalConfiguration
    {
        private readonly ILogger<InternalConfiguration> _logger;
        private string _discordBotToken;
        private string _clientId;
        private string _clientSecret;
        private string _absolutePathToFileUpload;
        private string _serviceHostName;
        private string _serviceDomain;
        private string _serviceBaseUrl;
        private List<ulong> _siteAdmins;
        private Language _defaultLanguage;
        private string _auditLogWebhookUrl;
        private bool _publicFilesEnabled;
        private bool _demoModeEnabled;
        private bool _customPluginsEnabled;
        private bool _corsEnabled;

        public InternalConfiguration(ILogger<InternalConfiguration> logger)
        {
            _logger = logger;
        }

        public void Init()
        {
            _logger.LogInformation("Initializing internal configuration.");
            _discordBotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
            _clientId = Environment.GetEnvironmentVariable("DISCORD_OAUTH_CLIENT_ID");
            _clientSecret = Environment.GetEnvironmentVariable("DISCORD_OAUTH_CLIENT_SECRET");
            _absolutePathToFileUpload = Environment.GetEnvironmentVariable("ABSOLUTE_PATH_TO_FILE_UPLOAD");
            _serviceHostName = Environment.GetEnvironmentVariable("META_SERVICE_NAME");
            _serviceDomain = Environment.GetEnvironmentVariable("META_SERVICE_DOMAIN");
            _serviceBaseUrl = Environment.GetEnvironmentVariable("META_SERVICE_BASE_URL");
            try
            {
                _siteAdmins = Environment.GetEnvironmentVariable("DISCORD_SITE_ADMINS").Split(",").Select(x => ulong.Parse(x)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not parse DISCORD_SITE_ADMINS.");
                _siteAdmins = new List<ulong>();
            }
            _defaultLanguage = Language.en;
            _auditLogWebhookUrl = string.Empty;
            _publicFilesEnabled = false;
            _demoModeEnabled = string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_DEMO_MODE"));
            _customPluginsEnabled = string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS"));
            _corsEnabled = string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CORS"));
        }

        public string GetBaseUrl()
        {
            return _serviceBaseUrl;
        }

        public string GetBotToken()
        {
            return _discordBotToken;
        }

        public string GetClientId()
        {
            return _clientId;
        }

        public string GetClientSecret()
        {
            return _clientSecret;
        }

        public void SetDefaultLanguage(Language language)
        {
            _defaultLanguage = language;
        }

        public Language GetDefaultLanguage()
        {
            return _defaultLanguage;
        }

        public string GetFileUploadPath()
        {
            return _absolutePathToFileUpload;
        }

        public string GetHostName()
        {
            return _serviceHostName;
        }

        public string GetServiceDomain()
        {
            return _serviceDomain;
        }

        public List<ulong> GetSiteAdmins()
        {
            return _siteAdmins;
        }

        public void SetAuditLogWebhook(string url)
        {
            _auditLogWebhookUrl = url;
        }

        public string GetAuditLogWebhook()
        {
            return _auditLogWebhookUrl;
        }

        public void SetPublicFileModeEnabled(bool mode)
        {
            _publicFilesEnabled = mode;
        }

        public bool IsPublicFileEnabled()
        {
            return _publicFilesEnabled;
        }

        public bool IsDemoModeEnabled()
        {
            return _demoModeEnabled;
        }

        public bool IsCustomPluginModeEnabled()
        {
            return _customPluginsEnabled;
        }

        public bool IsCorsEnabled()
        {
            return _corsEnabled;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using masz.Enums;
using Microsoft.Extensions.Logging;

namespace masz.Services
{
    public class InternalConfiguration: IInternalConfiguration
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
            } catch (Exception e)
            {
                _logger.LogError(e, "Could not parse DISCORD_SITE_ADMINS.");
                _siteAdmins = new List<ulong>();
            }
            switch (Environment.GetEnvironmentVariable("DEFAULT_LANGUAGE")) {
                case "de":
                    _defaultLanguage = Language.de;
                    break;
                case "it":
                    _defaultLanguage = Language.it;
                    break;
                case "fr":
                    _defaultLanguage = Language.fr;
                    break;
                case "es":
                    _defaultLanguage = Language.es;
                    break;
                case "at":
                    _defaultLanguage = Language.at;
                    break;
                case "ru":
                    _defaultLanguage = Language.ru;
                    break;
                default:
                    _defaultLanguage = Language.en;
                    break;
            }
            _auditLogWebhookUrl = Environment.GetEnvironmentVariable("AUDIT_LOG_WEBHOOK_URL");
            _publicFilesEnabled = String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_PUBLIC_FILES"));
            _demoModeEnabled = String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_DEMO_MODE"));
            _customPluginsEnabled = String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS"));
            _corsEnabled = String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_CORS"));
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

        public string GetAuditLogWebhook()
        {
            return _auditLogWebhookUrl;
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
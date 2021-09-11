using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Events;
using masz.Translations;
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
            _siteAdmins = Environment.GetEnvironmentVariable("DISCORD_SITE_ADMINS").Split(",").Select(x => ulong.Parse(x)).ToList();
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
                default:
                    _defaultLanguage = Language.en;
                    break;
            }
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
    }
}
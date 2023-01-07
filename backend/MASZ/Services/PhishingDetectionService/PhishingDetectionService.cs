using MASZ.Extensions;
using MASZ.Models;
using Timer = System.Timers.Timer;

namespace MASZ.Services
{
    public class PhishingDetectionService
    {
        private readonly ILogger<PhishingDetectionService> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordAPIInterface _discordAPI;
        private readonly FilesHandler _filesHandler;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdentityManager _identityManager;
        private readonly InternalEventHandler _eventHandler;
        private readonly List<IDomainList> _domainSources;
        private readonly HashSet<string> _domainList = new HashSet<string>();

        public PhishingDetectionService(ILogger<PhishingDetectionService> logger, InternalConfiguration config, DiscordAPIInterface discord, IServiceProvider serviceProvider, FilesHandler filesHandler, IdentityManager identityManager, InternalEventHandler eventHandler)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discord;
            _serviceProvider = serviceProvider;
            _filesHandler = filesHandler;
            _identityManager = identityManager;
            _eventHandler = eventHandler;

            _domainSources = new List<IDomainList>();

            _domainSources.Add(new DiscordPhishingList());

            if (_config.IsLowMemoryPhishingListEnabled())
            {
                return;
            }

            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/abuse.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/ads.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/crypto.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/drugs.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/everything.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/facebook.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/fraud.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/gambling.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/malware.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/phishing.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/piracy.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/porn.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/ransomware.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/redirect.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/scam.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/tiktok.txt"));
            // _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/torrent.txt"));
            _domainSources.Add(new PiHoleList("https://blocklistproject.github.io/Lists/tracking.txt"));
        }

        public async Task ExecuteAsync()
        {
            _logger.LogWarning("Starting phishing detection schedule timers.");

            Timer MinuteEventTimer = new(TimeSpan.FromHours(24).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            MinuteEventTimer.Elapsed += (s, e) => ReloadLinkList();

            await Task.Run(() => MinuteEventTimer.Start());

            _logger.LogWarning("Started phishing detection schedule timers.");

            ReloadLinkList();
        }

        private void ReloadLinkList()
        {
            _logger.LogInformation("Reloading phishing detection domain list.");
            _domainList.Clear();
            foreach (IDomainList domainList in _domainSources)
            {
                try
                {
                    foreach (string domain in domainList.ReloadDomainList())
                    {
                        _domainList.Add(domain);
                    }
                } catch (Exception e)
                {
                    _logger.LogError(e, "Failed to load domains from phishing detection list");
                }
            }
            _logger.LogInformation("Reloaded phishing detection domain list with count: " + _domainList.Count);
        }

        public bool Checkstring(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            foreach (string toCheck in url.GetAllPossiblePartsFromUrl())
            {
                if (_domainList.Contains(toCheck))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
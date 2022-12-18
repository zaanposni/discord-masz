using RestSharp;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Dtos;
using MASZ.Utils;
using System.Net;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace MASZ.Services
{
    public class TelemetryService : IEvent
    {
        private readonly ILogger<TelemetryService> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordAPIInterface _discordAPI;
        private readonly FilesHandler _filesHandler;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdentityManager _identityManager;
        private readonly InternalEventHandler _eventHandler;
        private readonly string remoteUrl = "https://maszindex.zaanposni.com/api/v1/telemetry/";
        private readonly string hashedServerIdentifier;
        private readonly byte[] hashKey;

        public TelemetryService(ILogger<TelemetryService> logger, InternalConfiguration config, DiscordAPIInterface discord, IServiceProvider serviceProvider, FilesHandler filesHandler, IdentityManager identityManager, InternalEventHandler eventHandler)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discord;
            _serviceProvider = serviceProvider;
            _filesHandler = filesHandler;
            _identityManager = identityManager;
            _eventHandler = eventHandler;

            if (! _config.IsTelemetryEnabled()) return;

            _logger.LogWarning("Telemetry is enabled. This will send anonymous data to the MASZ developers.");

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] serverIdentifier;
                if (_config.GetServiceDomain().Contains("http://localhost"))
                {
                    serverIdentifier = Encoding.UTF8.GetBytes(_config.GetClientId());
                    hashKey = Encoding.UTF8.GetBytes(_config.GetClientId());
                } else
                {
                    serverIdentifier = Encoding.UTF8.GetBytes(_config.GetServiceDomain());
                    hashKey = Encoding.UTF8.GetBytes(_config.GetServiceDomain());
                }

                hashedServerIdentifier = BitConverter.ToString(sha256Hash.ComputeHash(serverIdentifier)).Replace("-", "").ToLower();

                _logger.LogInformation($"Using server identifier: \"{hashedServerIdentifier}\" for telemetry reporting.");
            }
        }

        public void RegisterEvents()
        {
        }

        public async Task ExecuteAsync()
        {
            if (!_config.IsTelemetryEnabled())
            {
                _logger.LogInformation("Telemetry is disabled. Skipping.");
                return;
            }

            _logger.LogWarning("Starting telemetry schedule timers.");

            Timer eventTimer = new(TimeSpan.FromDays(7).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            Timer initialEventTimer = new(TimeSpan.FromMinutes(1).TotalMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            Timer initialCollectionTimer = new(TimeSpan.FromMinutes(1).TotalMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };

            eventTimer.Elapsed += async (s, e) => await CollectWeeklyData();
            initialEventTimer.Elapsed += async (s, e) => await CollectWeeklyData();
            initialCollectionTimer.Elapsed += async (s, e) => await CollectInitialData();

            await Task.Run(() => eventTimer.Start());
            await Task.Run(() => initialEventTimer.Start());
            await Task.Run(() => initialCollectionTimer.Start());

            _logger.LogWarning("Started telemetry schedule timers.");
        }

        public Task CollectInitialData()
        {
            _logger.LogInformation("Collecting initial telemetry data.");
            return Task.CompletedTask;
        }

        public async Task CollectWeeklyData()
        {
            _logger.LogInformation("Collecting weekly telemetry data.");

            StatusRepository repo = StatusRepository.CreateDefault(_serviceProvider);

            StatusDetail botDetails = repo.GetBotStatus();
            StatusDetail dbDetails = await repo.GetDbStatus();

            Process proc = Process.GetCurrentProcess();
            GCMemoryInfo gcMemoryInfo = GC.GetGCMemoryInfo();

            TelemetryDataResourceDto dto = new TelemetryDataResourceDto() {
                HashedServer = hashedServerIdentifier,
                AllocatedMemory = proc.PrivateMemorySize64,
                TotalMemory = gcMemoryInfo.TotalAvailableMemoryBytes,
                FreeMemory = gcMemoryInfo.TotalAvailableMemoryBytes - proc.PrivateMemorySize64,
                CPUUsage = 0,
                DatabaseLatency = dbDetails.ResponseTime ?? -1,
                DiscordLatency = botDetails.ResponseTime ?? -1
            };

            await SendTelemetryData<TelemetryDataResourceDto>("resource", dto);

            _logger.LogInformation("Collected weekly telemetry data.");
        }

        private async Task SendTelemetryData<T>(string resource, T dto)
        {
            RestClient client = new RestClient(remoteUrl);
            RestRequest request = new RestRequest(resource, Method.Post);
            request.AddJsonBody(dto);
            RestResponse response = await client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Failed to send telemetry data to {remoteUrl}/{resource}: {response.StatusCode} - {response.Content}");
            } else {
                _logger.LogInformation($"Successfully sent telemetry data to {remoteUrl}/{resource}: {response.StatusCode}");
            }
        }
    }
}

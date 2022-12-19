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
using MASZ.Exceptions;
using Discord;
using Newtonsoft.Json.Linq;
using RestSharp.Serializers.NewtonsoftJson;

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

            byte[] clientSecret = Encoding.UTF8.GetBytes(_config.GetClientSecret());
            using (HMACSHA256 hmac = new HMACSHA256(clientSecret))
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

                hashedServerIdentifier = BitConverter.ToString(hmac.ComputeHash(serverIdentifier)).Replace("-", "").ToLower();

                _logger.LogInformation($"Using server identifier: \"{hashedServerIdentifier}\" for telemetry reporting.");
            }
        }

        public void RegisterEvents()
        {
            if (! _config.IsTelemetryEnabled()) return;

            _eventHandler.OnIdentityRegistered += async (a) => await OnIdentityRegistered(a);
            _eventHandler.OnModCaseCreated += async (a, b, c, d) => await OnModCaseCreated(a, b);
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

            // =======================================================================================================

            StatusRepository statusRepo = StatusRepository.CreateDefault(_serviceProvider);

            StatusDetail botDetails = statusRepo.GetBotStatus();
            StatusDetail dbDetails = await statusRepo.GetDbStatus();

            Process proc = Process.GetCurrentProcess();
            GCMemoryInfo gcMemoryInfo = GC.GetGCMemoryInfo();

            TelemetryDataResourceDto resourceDto = new TelemetryDataResourceDto() {
                HashedServer = hashedServerIdentifier,
                AllocatedMemory = proc.PrivateMemorySize64,
                TotalMemory = gcMemoryInfo.TotalAvailableMemoryBytes,
                FreeMemory = gcMemoryInfo.TotalAvailableMemoryBytes - proc.PrivateMemorySize64,
                CPUUsage = 0,
                DatabaseLatency = dbDetails.ResponseTime ?? -1,
                DiscordLatency = botDetails.ResponseTime ?? -1
            };

            await SendTelemetryData<TelemetryDataResourceDto>("resource", resourceDto);

            // =======================================================================================================

            List<GuildConfig> guildConfigs = await GuildConfigRepository.CreateDefault(_serviceProvider).GetAllGuildConfigs();
            AppSettings appSettings = await AppSettingsRepository.CreateDefault(_serviceProvider).GetAppSettings();
            List<APIToken> apiTokens = await TokenRepository.CreateDefault(_serviceProvider).GetAllTokens();

            TelemetryDataGlobalFeatureUsageDto globalFeatureUsageDto = new TelemetryDataGlobalFeatureUsageDto {
                HashedServer = hashedServerIdentifier,
                GuildCount = guildConfigs.Count,
                AuditLogEnabled = appSettings.AuditLogWebhookURL != null,
                PublicFileMode = appSettings.PublicFileMode,
                APITokenCount = apiTokens.Count
            };

            await SendTelemetryData<TelemetryDataGlobalFeatureUsageDto>("globalfeatureusage", globalFeatureUsageDto);

            // =======================================================================================================

            foreach (GuildConfig guild in guildConfigs)
            {
                string hashedGuildId = HashStringWithPrivateKey(guild.GuildId.ToString());

                GuildMotd motd = null;
                ZalgoConfig zalgoConfig = null;

                try
                {
                    await GuildMotdRepository.CreateWithBotIdentity(_serviceProvider).GetMotd(guild.GuildId);
                } catch (ResourceNotFoundException) { }
                try
                {
                    zalgoConfig = await ZalgoRepository.CreateWithBotIdentity(_serviceProvider).GetZalgo(guild.GuildId);
                } catch (ResourceNotFoundException) { }

                TelemetryDataGuildFeatureUsageDto guildFeatureUsageDto = new TelemetryDataGuildFeatureUsageDto(
                    hashedServerIdentifier,
                    hashedGuildId,
                    guild,
                    await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetCasesForGuild(guild.GuildId),
                    await AppealRepository.CreateDefault(_serviceProvider).GetForGuild(guild.GuildId),
                    await AppealStructureRepository.CreateDefault(_serviceProvider).GetForGuild(guild.GuildId),
                    await UserNoteRepository.CreateWithBotIdentity(_serviceProvider).GetUserNotesByGuild(guild.GuildId),
                    await UserMapRepository.CreateWithBotIdentity(_serviceProvider).GetUserMapsByGuild(guild.GuildId),
                    await ScheduledMessageRepository.CreateWithBotIdentity(_serviceProvider).GetAllMessages(guild.GuildId),
                    motd,
                    await GuildLevelAuditLogConfigRepository.CreateWithBotIdentity(_serviceProvider).GetConfigsByGuild(guild.GuildId),
                    await AutoModerationConfigRepository.CreateWithBotIdentity(_serviceProvider).GetConfigsByGuild(guild.GuildId),
                    await AutoModerationEventRepository.CreateDefault(_serviceProvider).GetAllEventsForGuild(guild.GuildId),
                    zalgoConfig,
                    await InviteRepository.CreateDefault(_serviceProvider).CountInvitesForGuild(guild.GuildId)
                );

                await SendTelemetryData<TelemetryDataGuildFeatureUsageDto>("guildfeatureusage", guildFeatureUsageDto);
            }

            _logger.LogInformation("Collected weekly telemetry data.");
        }

        private string HashStringWithPrivateKey(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            HMACSHA256 hmac = new HMACSHA256(hashKey);
            string output = BitConverter.ToString(hmac.ComputeHash(inputBytes)).Replace("-", "").ToLower();
            hmac.Dispose();

            return output;
        }

        private async Task SendTelemetryData<T>(string resource, T dto) where T : class
        {
            if (dto is TelemetryDataUsageDto usageDto)
            {
                if (usageDto.AdditionalData == null)
                {
                    usageDto.AdditionalData = new JObject();
                }
            }

            RestClient client = new RestClient(remoteUrl);
            client.UseNewtonsoftJson();
            RestRequest request = new RestRequest(resource, Method.Post);
            request.AddJsonBody<T>(dto);
            try
            {
                RestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Failed to send telemetry data to {remoteUrl}{resource}: {response.StatusCode} - {response.Content}");
                } else {
                    if (dto is TelemetryDataUsageDto telemetryDataUsageDto)
                    {
                        _logger.LogInformation($"Successfully sent telemetry data to {remoteUrl}{resource} usage {telemetryDataUsageDto.ActionType.ToString()}: {response.StatusCode}");
                    } else
                    {
                        _logger.LogInformation($"Successfully sent telemetry data to {remoteUrl}{resource}: {response.StatusCode}");
                    }
                }
            } catch (Exception e)
            {
                _logger.LogError($"Failed to send telemetry data to {remoteUrl}{resource}: {e.Message}");
                return;
            }
        }

        public async Task OnIdentityRegistered(Identity identity)
        {
            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(identity.GetCurrentUser().Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = Enums.TelemetryDataUsageActionType.LOGIN
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnModCaseCreated(ModCase modCase, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = Enums.TelemetryDataUsageActionType.MODCASE_CREATE,
                AdditionalData = new JObject {
                    { "punishmentType", (int) modCase.PunishmentType },
                    { "labelCount", modCase.Labels.Count() },
                    { "creationType", (int) modCase.CreationType }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }
    }
}

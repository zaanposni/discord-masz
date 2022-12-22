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
using MASZ.Enums;
using Discord.Interactions;
using MASZ.Plugins;

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
            _eventHandler.OnModCaseCreated += async (a, b, c, d) => await OnModCaseEvent(a, b, TelemetryDataUsageActionType.MODCASE_CREATE);
            _eventHandler.OnModCaseUpdated += async (a, b, c, d) => await OnModCaseEvent(a, b, TelemetryDataUsageActionType.MODCASE_UPDATE);
            _eventHandler.OnModCaseDeleted += async (a, b, c, d) => await OnModCaseEvent(a, b, TelemetryDataUsageActionType.MODCASE_DELETE);
            _eventHandler.OnModCaseRestored += async (a, b) => await OnModCaseEvent(a, b, TelemetryDataUsageActionType.MODCASE_RESTORE);

            // TODO: on modcase link

            _eventHandler.OnModCaseCommentCreated += async (a, b) => await OnCommentEvent(a, b, TelemetryDataUsageActionType.MODCASE_CREATE_COMMENT);
            _eventHandler.OnModCaseCommentUpdated += async (a, b) => await OnCommentEvent(a, b, TelemetryDataUsageActionType.MODCASE_UPDATE_COMMENT);
            _eventHandler.OnModCaseCommentDeleted += async (a, b) => await OnCommentEvent(a, b, TelemetryDataUsageActionType.MODCASE_DELETE_COMMENT);

            // TODO: on comment lock

            _eventHandler.OnFileUploaded += async (a, b, c) => await OnFileEvent(a, b, c, TelemetryDataUsageActionType.MODCASE_UPLOAD_ATTACHMENT);
            _eventHandler.OnFileDeleted += async (a, b, c) => await OnFileEvent(a, b, c, TelemetryDataUsageActionType.MODCASE_DELETE_ATTACHMENT);

            _eventHandler.OnAppealCreated += async (a, b) => await OnAppealCreated(a, b);
            _eventHandler.OnAppealUpdated += async (a, b, c) => await OnAppealUpdated(a, b);

            _eventHandler.OnUserNoteCreated += async (a, b) => await OnUserNoteEvent(a, b, TelemetryDataUsageActionType.USERNOTE_CREATE);
            _eventHandler.OnUserNoteUpdated += async (a, b) => await OnUserNoteEvent(a, b, TelemetryDataUsageActionType.USERNOTE_UPDATE);
            _eventHandler.OnUserNoteDeleted += async (a, b) => await OnUserNoteEvent(a, b, TelemetryDataUsageActionType.USERNOTE_DELETE);

            _eventHandler.OnUserMapCreated += async (a, b) => await OnUserMapEvent(a, b, TelemetryDataUsageActionType.USERMAP_CREATE);
            _eventHandler.OnUserMapUpdated += async (a, b) => await OnUserMapEvent(a, b, TelemetryDataUsageActionType.USERMAP_UPDATE);
            _eventHandler.OnUserMapDeleted += async (a, b) => await OnUserMapEvent(a, b, TelemetryDataUsageActionType.USERMAP_DELETE);

            // TODO: on scheduled messages

            _eventHandler.OnGuildMotdCreated += async (a, b) => await OnMotdEvent(a, b);
            _eventHandler.OnGuildMotdUpdated += async (a, b) => await OnMotdEvent(a, b);

            _eventHandler.OnGuildLevelAuditLogConfigCreated += async (a, b) => await OnGuildLevelAuditLogConfigEvent(a, b);
            _eventHandler.OnGuildLevelAuditLogConfigUpdated += async (a, b) => await OnGuildLevelAuditLogConfigEvent(a, b);

            _eventHandler.OnAutoModerationConfigCreated += async (a, b) => await OnAutoModerationConfigEvent(a, b);
            _eventHandler.OnAutoModerationConfigUpdated += async (a, b) => await OnAutoModerationConfigEvent(a, b);

            _eventHandler.OnZalgoConfigUpdated += async (a, b) => await OnZalgoConfigEvent(a, b);

            _eventHandler.OnGuildRegistered += async (a, b, c) => await OnGuildEvent(a, b, TelemetryDataUsageActionType.GUILD_ADDED);
            _eventHandler.OnGuildUpdated += async (a, b) => await OnGuildEvent(a, b, TelemetryDataUsageActionType.GUILD_EDITED);
            _eventHandler.OnGuildDeleted += async (a, b) => await OnGuildEvent(a, b, TelemetryDataUsageActionType.GUILD_REMOVED);

            // TODO: on admin settings edited

            _eventHandler.OnCaseTemplateCreated += async (a, b) => await OnCaseTemplateEvent(a, b, TelemetryDataUsageActionType.CASE_TEMPLATE_CREATE);

            _eventHandler.OnApplicationCommandUsed += async (a, b, c) => await OnApplicationCommandUsed(a, b, c);
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

        public async Task CollectInitialData()
        {
            _logger.LogInformation("Collecting initial telemetry data.");

            int pluginCount = -1;

            if (_config.IsCustomPluginModeEnabled())
            {
                pluginCount = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .Where(x => typeof(IBasePlugin).IsAssignableFrom(x) &&
                                            !x.IsInterface &&
                                            !x.IsAbstract &&
                                            x.Namespace.Contains("MASZ.Plugins"))
                                .Count();
            }

            TelemetryDataConfigurationDto configurationDto = new TelemetryDataConfigurationDto() {
                HashedServer = hashedServerIdentifier,
                DeploymentMode = string.IsNullOrEmpty(_config.GetDeployMode()) ? "unknown" : _config.GetDeployMode(),
                DeploymentVersion = _config.GetVersion(),
                DefaultLanguage = (int) _config.GetDefaultLanguage(),
                PublicFileMode = _config.IsPublicFileEnabled(),
                CustomPluginsEnabled = _config.IsCustomPluginModeEnabled(),
                CustomPluginsCount = pluginCount,
                SiteAdminsCount = _config.GetSiteAdmins().Count
            };

            await SendTelemetryData<TelemetryDataConfigurationDto>("configuration", configurationDto);


            _logger.LogInformation("Collected initial telemetry data.");
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

        public async Task OnModCaseEvent(ModCase modCase, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType,
                AdditionalData = new JObject {
                    { "punishmentType", (int) modCase.PunishmentType },
                    { "labelCount", modCase.Labels.Count() },
                    { "creationType", (int) modCase.CreationType }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnCommentEvent(ModCaseComment comment, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnFileEvent(UploadedFile file, ModCase modCase, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType,
                AdditionalData = new JObject {
                    { "contentType", file.ContentType }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnAppealCreated(Appeal appeal, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = TelemetryDataUsageActionType.APPEAL_CREATE
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnAppealUpdated(Appeal appeal, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageActionType telemetryDataUsageActionType = TelemetryDataUsageActionType.UNKNOWN;
            if (appeal.Status == AppealStatus.Pending) return;
            if (appeal.Status == AppealStatus.Approved) telemetryDataUsageActionType = TelemetryDataUsageActionType.APPEAL_ACCEPTED;
            if (appeal.Status == AppealStatus.Declined) telemetryDataUsageActionType = TelemetryDataUsageActionType.APPEAL_DENIED;

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnUserNoteEvent(UserNote userNote, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType,
                AdditionalData = new JObject {
                    { "length", userNote.Description.Length }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnUserMapEvent(UserMapping userMapping, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType,
                AdditionalData = new JObject {
                    { "length", userMapping.Reason.Length }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnMotdEvent(GuildMotd motd, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = TelemetryDataUsageActionType.MOTD_EDITED,
                AdditionalData = new JObject {
                    { "length", motd.Message.Length }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnGuildLevelAuditLogConfigEvent(GuildLevelAuditLogConfig auditLogConfig, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = TelemetryDataUsageActionType.AUDITLOG_EDITED,
                AdditionalData = new JObject {
                    { "eventType", (int) auditLogConfig.GuildAuditLogEvent }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnAutoModerationConfigEvent(AutoModerationConfig autoModerationConfig, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = TelemetryDataUsageActionType.AUTOMOD_EDITED,
                AdditionalData = new JObject {
                    { "eventType", (int) autoModerationConfig.AutoModerationType },
                    { "actionType", (int) autoModerationConfig.AutoModerationAction }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnZalgoConfigEvent(ZalgoConfig zalgoConfig, IUser user)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = TelemetryDataUsageActionType.ZALGO_EDITED,
                AdditionalData = new JObject {
                    { "enabled", zalgoConfig.Enabled }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnGuildEvent(GuildConfig guildConfig, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = HashStringWithPrivateKey(guildConfig.GuildId.ToString()),
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnCaseTemplateEvent(CaseTemplate caseTemplate, IUser user, TelemetryDataUsageActionType telemetryDataUsageActionType)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = null,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = telemetryDataUsageActionType
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }

        public async Task OnApplicationCommandUsed(ICommandInfo commandInfo, IUser user, IGuild guild)
        {
            Identity identity = _identityManager.GetIdentityByUserId(user.Id);

            string hashedGuildId = null;
            if (guild != null) {
                hashedGuildId = HashStringWithPrivateKey(guild.Id.ToString());
            }

            TelemetryDataUsageDto dto = new TelemetryDataUsageDto() {
                HashedServer = hashedServerIdentifier,
                HashedUserId = HashStringWithPrivateKey(user.Id.ToString()),
                HashedGuildId = hashedGuildId,
                UserIsSiteAdmin = identity.IsSiteAdmin(),
                UserIsToken = identity is TokenIdentity,
                ActionType = TelemetryDataUsageActionType.APPLICATION_COMMAND_USED,
                AdditionalData = new JObject {
                    { "name", commandInfo.Name }
                }
            };

            await SendTelemetryData<TelemetryDataUsageDto>("usage", dto);
        }
    }
}

using Discord;
using Discord.WebSocket;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using System.Text;

namespace MASZ.Services
{
    public class AuditLogger : BackgroundService
    {
        private readonly ILogger<AuditLogger> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordAPIInterface _discordAPI;
        private readonly DiscordSocketClient _client;
        private readonly InternalEventHandler _eventHandler;
        private readonly StringBuilder _currentMessage;


        public AuditLogger(ILogger<AuditLogger> logger, InternalConfiguration config, DiscordAPIInterface discordAPI, InternalEventHandler eventHandler, DiscordSocketClient client)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discordAPI;
            _eventHandler = eventHandler;
            _client = client;
            _currentMessage = new StringBuilder();

            RegisterEvents();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            QueueLog($"============== STARTUP ==============");
            QueueLog("`MASZ` started!");
            QueueLog("System time: " + DateTime.Now.ToString());
            QueueLog("System time (UTC): " + DateTime.UtcNow.ToString());
            QueueLog($"Language: `{_config.GetDefaultLanguage()}`");
            QueueLog($"Hostname: `{_config.GetHostName()}`");
            QueueLog($"URL: `{_config.GetBaseUrl()}`");
            QueueLog($"Domain: `{_config.GetServiceDomain()}`");
            QueueLog($"ClientID: `{_config.GetClientId()}`");

            if (_config.IsCorsEnabled())
            {
                QueueLog("CORS support: \u26A0 `ENABLED`");
            }
            else
            {
                QueueLog("CORS support: `DISABLED`");
            }

            if (_config.IsCustomPluginModeEnabled())
            {
                QueueLog("Plugin support: \u26A0 `ENABLED`");
            }
            else
            {
                QueueLog("Plugin support: `DISABLED`");
            }

            if (_config.IsDemoModeEnabled())
            {
                QueueLog("Demo mode: \u26A0 `ENABLED`");
            }
            else
            {
                QueueLog("Demo mode: `DISABLED`");
            }

            if (_config.IsPublicFileEnabled())
            {
                QueueLog("Public file mode: \u26A0 `ENABLED`");
            }
            else
            {
                QueueLog("Public file mode: `DISABLED`");
            }

            QueueLog($"============== /STARTUP =============");
            await ExecuteWebhook();
        }

        public void RegisterEvents()
        {
            _client.Ready += OnBotReady;
            _client.Disconnected += OnDisconnect;

            _eventHandler.OnIdentityRegistered += OnIdentityRegistered;
            _eventHandler.OnTokenCreated += OnTokenCreated;
            _eventHandler.OnTokenDeleted += OnTokenDeleted;
            _eventHandler.OnGuildRegistered += OnGuildRegistered;
            _eventHandler.OnGuildUpdated += OnGuildUpdated;
            _eventHandler.OnGuildDeleted += OnGuildDeleted;
            _eventHandler.OnModCaseCreated += OnModCaseCreated;
            _eventHandler.OnModCaseUpdated += OnModCaseUpdated;
            _eventHandler.OnModCaseDeleted += OnModCaseDeleted;
            _eventHandler.OnModCaseCommentCreated += OnModCaseCommentCreated;
            _eventHandler.OnModCaseCommentUpdated += OnModCaseCommentUpdated;
            _eventHandler.OnModCaseCommentDeleted += OnModCaseCommentDeleted;
            _eventHandler.OnGuildMotdUpdated += OnGuildMotdUpdated;
            _eventHandler.OnFileUploaded += OnFileUploaded;
            _eventHandler.OnInternalCachingDone += OnInternalCachingDone;
        }

        public async void QueueLog(string message)
        {
            message = DateTime.UtcNow.ToDiscordTS() + " " + message[..Math.Min(message.Length, 1950)];
            if (!string.IsNullOrEmpty(_config.GetAuditLogWebhook()))
            {
                if (_currentMessage.Length + message.Length <= 1998)
                {
                    _currentMessage.AppendLine(message);
                }
                else
                {
                    await ExecuteWebhook();
                    _currentMessage.AppendLine(message);
                }
            }
        }

        public async Task ExecuteWebhook()
        {
            if (_currentMessage.Length > 0)
            {
                _logger.LogInformation("Executing auditlog webhook.");
                try
                {
                    await _discordAPI.ExecuteWebhook(_config.GetAuditLogWebhook(), null, _currentMessage.ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error executing auditlog webhook. ");
                }
                _currentMessage.Clear();
            }
        }

        private Task OnFileUploaded(UploadedFile fileInfo)
        {
            QueueLog($"**File** `{fileInfo.Name}` uploaded.");
            return Task.CompletedTask;
        }

        private Task OnGuildMotdUpdated(GuildMotd motd)
        {
            QueueLog($"**Motd** for guild `{motd.GuildId}` updated.");
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentDeleted(ModCaseComment modCaseComment)
        {
            QueueLog($"**Comment** `{modCaseComment.ModCase.GuildId}/{modCaseComment.ModCase.CaseId}/{modCaseComment.Id}` by <@{modCaseComment.UserId}> deleted.");
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentUpdated(ModCaseComment modCaseComment)
        {
            QueueLog($"**Comment** `{modCaseComment.ModCase.GuildId}/{modCaseComment.ModCase.CaseId}/{modCaseComment.Id}` by <@{modCaseComment.UserId}> updated.");
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentCreated(ModCaseComment modCaseComment)
        {
            QueueLog($"**Comment** `{modCaseComment.ModCase.GuildId}/{modCaseComment.ModCase.CaseId}/{modCaseComment.Id}` by <@{modCaseComment.UserId}> created.");
            return Task.CompletedTask;
        }

        private Task OnModCaseDeleted(ModCase modCase)
        {
            QueueLog($"**Modcase** `{modCase.GuildId}/{modCase.CaseId}` for <@{modCase.UserId}> deleted.");
            return Task.CompletedTask;
        }

        private Task OnModCaseUpdated(ModCase modCase)
        {
            QueueLog($"**Modcase** `{modCase.GuildId}/{modCase.CaseId}` for <@{modCase.UserId}> by <@{modCase.LastEditedByModId}> updated.");
            return Task.CompletedTask;
        }

        private Task OnModCaseCreated(ModCase modCase)
        {
            QueueLog($"**Modcase** `{modCase.GuildId}/{modCase.CaseId}` for <@{modCase.UserId}> by <@{modCase.ModId}> created.");
            return Task.CompletedTask;
        }

        private Task OnGuildDeleted(GuildConfig guildConfig)
        {
            QueueLog($"**Guild** `{guildConfig.GuildId}` deleted.");
            return Task.CompletedTask;
        }

        private Task OnGuildUpdated(GuildConfig guildConfig)
        {
            QueueLog($"**Guild** `{guildConfig.GuildId}` updated.");
            return Task.CompletedTask;
        }

        private Task OnGuildRegistered(GuildConfig guildConfig)
        {
            QueueLog($"**Guild** `{guildConfig.GuildId}` registered.");
            return Task.CompletedTask;
        }

        private async Task OnInternalCachingDone(int _, DateTime nextCache)
        {
            QueueLog($"Internal cache refreshed with `{_discordAPI.GetCache().Keys.Count}` entries. Next cache refresh {nextCache.ToDiscordTS(DiscordTimestampFormat.RelativeTime)}.");
            await ExecuteWebhook();
        }

        private async Task OnDisconnect(Exception _)
        {
            QueueLog($"Bot **disconnected** from discord sockets.");
            await ExecuteWebhook();
        }

        private async Task OnBotReady()
        {
            QueueLog($"Bot **connected** to `{_client.Guilds.Count} guild(s)` with `{_client.Latency}ms` latency.");
            await ExecuteWebhook();
        }

        private Task OnTokenDeleted(APIToken token)
        {
            QueueLog($"**Token** `{token.Name.Truncate(1500)}` (`#{token.Id}`) has been deleted.");
            return Task.CompletedTask;
        }

        private Task OnTokenCreated(APIToken token)
        {
            QueueLog($"**Token** `{token.Name.Truncate(1500)}` (`#{token.Id}`) has been created and expires {token.ValidUntil.ToDiscordTS(DiscordTimestampFormat.RelativeTime)}.");
            return Task.CompletedTask;
        }

        private Task OnIdentityRegistered(Identity identity)
        {
            if (identity is DiscordOAuthIdentity dOauth)
            {
                IUser currentUser = dOauth.GetCurrentUser();
                string userDefinition = $"`{currentUser.Username}#{currentUser.Discriminator}` (`{currentUser.Id}`)";
                QueueLog($"{userDefinition} **logged in** using OAuth.");
            }

            return Task.CompletedTask;
        }

    }
}
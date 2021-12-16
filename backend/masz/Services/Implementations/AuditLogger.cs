using Discord;
using MASZ.Enums;
using MASZ.Events;
using MASZ.Extensions;
using System.Text;

namespace MASZ.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;
        private readonly IInternalConfiguration _config;
        private readonly IDiscordAPIInterface _discordAPI;
        private readonly IDiscordBot _discordBot;
        private readonly IEventHandler _eventHandler;
        private readonly StringBuilder _currentMessage;
        public AuditLogger() { }

        public AuditLogger(ILogger<AuditLogger> logger, IInternalConfiguration config, IDiscordAPIInterface discordAPI, IEventHandler eventHandler, IDiscordBot discordBot)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discordAPI;
            _eventHandler = eventHandler;
            _discordBot = discordBot;
            _currentMessage = new StringBuilder();
        }

        public async void QueueLog(string message)
        {
            message = DateTime.UtcNow.ToDiscordTS() + " " + message[..Math.Min(message.Length, 1950)];
            if (!string.IsNullOrEmpty(_config.GetAuditLogWebhook()))
            {
                if (_currentMessage.Length + message.Length <= 1998)  // +2 for newline?
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

        public void RegisterEvents()
        {
            _discordBot.GetClient().Ready += OnBotReady;
            _discordBot.GetClient().Disconnected += OnDisconnect;
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
            _logger.LogInformation("Registered events for audit logger.");
        }

        private Task OnFileUploaded(FileUploadedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**File** `{e.GetFileInfo().Name}` uploaded.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildMotdUpdated(GuildMotdUpdatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Motd** for guild `{e.GetGuildMotd().GuildId}` updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentDeleted(ModCaseCommentDeletedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Comment** `{e.GetModCaseComment().ModCase.GuildId}/{e.GetModCaseComment().ModCase.CaseId}/{e.GetModCaseComment().Id}` by <@{e.GetModCaseComment().UserId}> deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentUpdated(ModCaseCommentUpdatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Comment** `{e.GetModCaseComment().ModCase.GuildId}/{e.GetModCaseComment().ModCase.CaseId}/{e.GetModCaseComment().Id}` by <@{e.GetModCaseComment().UserId}> updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentCreated(ModCaseCommentCreatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Comment** `{e.GetModCaseComment().ModCase.GuildId}/{e.GetModCaseComment().ModCase.CaseId}/{e.GetModCaseComment().Id}` by <@{e.GetModCaseComment().UserId}> created.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseDeleted(ModCaseDeletedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Modcase** `{e.GetModCase().GuildId}/{e.GetModCase().CaseId}` for <@{e.GetModCase().UserId}> deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseUpdated(ModCaseUpdatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Modcase** `{e.GetModCase().GuildId}/{e.GetModCase().CaseId}` for <@{e.GetModCase().UserId}> by <@{e.GetModCase().LastEditedByModId}> updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCreated(ModCaseCreatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Modcase** `{e.GetModCase().GuildId}/{e.GetModCase().CaseId}` for <@{e.GetModCase().UserId}> by <@{e.GetModCase().ModId}> created.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildDeleted(GuildDeletedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Guild** `{e.GetGuildConfig().GuildId}` deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildUpdated(GuildUpdatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Guild** `{e.GetGuildConfig().GuildId}` updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildRegistered(GuildRegisteredEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Guild** `{e.GetGuildConfig().GuildId}` registered.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnInternalCachingDone(InternalCachingDoneEventArgs e)
        {
            Task task = new(async () =>
            {
                QueueLog($"Internal cache refreshed with `{_discordAPI.GetCache().Keys.Count}` entries. Next cache refresh {e.GetNextCache().ToDiscordTS(DiscordTimestampFormat.RelativeTime)}.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnDisconnect(Exception _)
        {
            Task task = new(async () =>
            {
                QueueLog($"Bot **disconnected** from discord sockets.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnBotReady()
        {
            Task task = new(async () =>
            {
                QueueLog($"Bot **connected** to `{_discordBot.GetClient().Guilds.Count} guild(s)` with `{_discordBot.GetClient().Latency}ms` latency.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnTokenDeleted(TokenDeletedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Token** `{e.GetToken().Name.Truncate(1500)}` (`#{e.GetToken().Id}`) has been deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnTokenCreated(TokenCreatedEventArgs e)
        {
            Task task = new(() =>
            {
                QueueLog($"**Token** `{e.GetToken().Name.Truncate(1500)}` (`#{e.GetToken().Id}`) has been created and expires {e.GetToken().ValidUntil.ToDiscordTS(DiscordTimestampFormat.RelativeTime)}.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnIdentityRegistered(IdentityRegisteredEventArgs e)
        {
            Task task = new(() =>
            {
                if (e.IsOAuthIdentity())
                {
                    IUser currentUser = e.GetIdentity().GetCurrentUser();
                    string userDefinition = $"`{currentUser.Username}#{currentUser.Discriminator}` (`{currentUser.Id}`)";
                    QueueLog($"{userDefinition} **logged in** using OAuth.");
                }
            });
            task.Start();
            return Task.CompletedTask;
        }

        public void Startup()
        {
            Task task = new(async () =>
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
            });
            task.Start();
        }
    }
}
using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Enums;
using masz.Events;
using masz.Extensions;
using Microsoft.Extensions.Logging;

namespace masz.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;
        private readonly IInternalConfiguration _config;
        private readonly IDiscordAPIInterface _discordAPI;
        private readonly IDiscordBot _discordBot;
        private readonly IEventHandler _eventHandler;
        private StringBuilder _currentMessage;
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
            message = DateTime.UtcNow.ToDiscordTS() + " " + message.Substring(0, Math.Min(message.Length, 1950));
            if(! string.IsNullOrEmpty(_config.GetAuditLogWebhook()))
            {
                if(_currentMessage.Length + message.Length <= 1998)  // +2 for newline?
                {
                    _currentMessage.AppendLine(message);
                } else
                {
                    await ExecuteWebhook();
                    _currentMessage.AppendLine(message);
                }
            }
        }

        public async Task ExecuteWebhook()
        {
            if (_currentMessage.Length > 0) {
                _logger.LogInformation("Executing auditlog webhook.");
                try
                {
                    await _discordAPI.ExecuteWebhook(_config.GetAuditLogWebhook(), null, _currentMessage.ToString());
                } catch(Exception e)
                {
                    _logger.LogError(e, "Error executing auditlog webhook. ");
                }
                _currentMessage.Clear();
            }
        }

        public void RegisterEvents()
        {
            _discordBot.GetClient().Resumed += OnBotResume;
            _discordBot.GetClient().Ready += OnBotReady;
            _discordBot.GetClient().SocketErrored += OnSocketError;
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
            Task task = new Task(() => {
                QueueLog($"**File** `{e.GetFileInfo().Name}` uploaded.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildMotdUpdated(GuildMotdUpdatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Motd** for guild `{e.GetGuildMotd().GuildId}` updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentDeleted(ModCaseCommentDeletedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Comment** `{e.GetModCaseComment().ModCase.GuildId}/{e.GetModCaseComment().ModCase.CaseId}/{e.GetModCaseComment().Id}` by <@{e.GetModCaseComment().UserId}> deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentUpdated(ModCaseCommentUpdatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Comment** `{e.GetModCaseComment().ModCase.GuildId}/{e.GetModCaseComment().ModCase.CaseId}/{e.GetModCaseComment().Id}` by <@{e.GetModCaseComment().UserId}> updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCommentCreated(ModCaseCommentCreatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Comment** `{e.GetModCaseComment().ModCase.GuildId}/{e.GetModCaseComment().ModCase.CaseId}/{e.GetModCaseComment().Id}` by <@{e.GetModCaseComment().UserId}> created.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseDeleted(ModCaseDeletedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Modcase** `{e.GetModCase().GuildId}/{e.GetModCase().CaseId}` for <@{e.GetModCase().UserId}> deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseUpdated(ModCaseUpdatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Modcase** `{e.GetModCase().GuildId}/{e.GetModCase().CaseId}` for <@{e.GetModCase().UserId}> by <@{e.GetModCase().LastEditedByModId}> updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnModCaseCreated(ModCaseCreatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Modcase** `{e.GetModCase().GuildId}/{e.GetModCase().CaseId}` for <@{e.GetModCase().UserId}> by <@{e.GetModCase().ModId}> created.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildDeleted(GuildDeletedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Guild** `{e.GetGuildConfig().GuildId}` deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildUpdated(GuildUpdatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Guild** `{e.GetGuildConfig().GuildId}` updated.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnGuildRegistered(GuildRegisteredEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Guild** `{e.GetGuildConfig().GuildId}` registered.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnInternalCachingDone(InternalCachingDoneEventArgs e)
        {
            Task task = new Task(async () => {
                QueueLog($"Internal cache refreshed with `{_discordAPI.GetCache().Keys.Count}` entries. Next cache refresh {e.GetNextCache().ToDiscordTS(DiscordTimestampFormat.RelativeTime)}.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnSocketError(DiscordClient sender, SocketErrorEventArgs e)
        {
            Task task = new Task(async () => {
                QueueLog($"Bot **disconnected** from discord sockets.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnBotResume(DiscordClient sender, ReadyEventArgs e)
        {
            Task task = new Task(async () => {
                QueueLog($"Bot **reconnected** to `{sender.Guilds.Count} guild(s)` with `{sender.Ping}ms` latency.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnBotReady(DiscordClient sender, ReadyEventArgs e)
        {
            Task task = new Task(async () => {
                QueueLog($"Bot **connected** to `{sender.Guilds.Count} guild(s)` with `{sender.Ping}ms` latency.");
                await ExecuteWebhook();
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnTokenDeleted(TokenDeletedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Token** `{e.GetToken().Name.Truncate(1500)}` (`#{e.GetToken().Id}`) has been deleted.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnTokenCreated(TokenCreatedEventArgs e)
        {
            Task task = new Task(() => {
                QueueLog($"**Token** `{e.GetToken().Name.Truncate(1500)}` (`#{e.GetToken().Id}`) has been created and expires {e.GetToken().ValidUntil.ToDiscordTS(DiscordTimestampFormat.RelativeTime)}.");
            });
            task.Start();
            return Task.CompletedTask;
        }

        private Task OnIdentityRegistered(IdentityRegisteredEventArgs e)
        {
            Task task = new Task(() => {
                if (e.IsOAuthIdentity())
                {
                    DiscordUser currentUser = e.GetIdentity().GetCurrentUser();
                    string userDefinition = $"`{currentUser.Username}#{currentUser.Discriminator}` (`{currentUser.Id}`)";
                    QueueLog($"{userDefinition} **logged in** using OAuth.");
                }
            });
            task.Start();
            return Task.CompletedTask;
        }

        public void Startup()
        {
            Task task = new Task(async () => {
                QueueLog($"============== STARTUP ==============");
                QueueLog("`MASZ` started!");
                QueueLog("System time: " + DateTime.Now.ToString());
                QueueLog("System time (UTC): " + DateTime.UtcNow.ToString());
                QueueLog($"Language: `{_config.GetDefaultLanguage()}`");
                QueueLog($"Hostname: `{_config.GetHostName()}`");
                QueueLog($"URL: `{_config.GetBaseUrl()}`");
                QueueLog($"Domain: `{_config.GetServiceDomain()}`");
                QueueLog($"ClientID: `{_config.GetClientId()}`");

                if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_CORS")))
                {
                    QueueLog("CORS support: \u26A0 `ENABLED`");
                } else {
                    QueueLog("CORS support: `DISABLED`");
                }

                if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS")))
                {
                    QueueLog("Plugin support: \u26A0 `ENABLED`");
                } else {
                    QueueLog("Plugin support: `DISABLED`");
                }

                if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_DEMO_MODE")))
                {
                    QueueLog("Demo mode: \u26A0 `ENABLED`");
                } else {
                    QueueLog("Demo mode: `DISABLED`");
                }

                QueueLog($"============== /STARTUP =============");

                await ExecuteWebhook();
            });
            task.Start();
        }
    }
}
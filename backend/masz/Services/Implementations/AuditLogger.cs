using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
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
        private readonly IEventHandler _eventHandler;
        private StringBuilder _currentMessage;
        public AuditLogger() { }

        public AuditLogger(ILogger<AuditLogger> logger, IInternalConfiguration config, IDiscordAPIInterface discordAPI, IEventHandler eventHandler)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discordAPI;
            _eventHandler = eventHandler;
            _currentMessage = new StringBuilder();
        }

        private async void QueueLog(string message)
        {
            message = DateTime.UtcNow.ToDiscordTS() + " " + message.Substring(0, Math.Min(message.Length, 1950));
            if(! string.IsNullOrEmpty(_config.GetAuditLogWebhook()))
            {
                if(_currentMessage.Length + message.Length <= 1998)  // +2 for newline?
                {
                    _currentMessage.AppendLine(message);
                } else
                {
                    await _discordAPI.ExecuteWebhook(_config.GetAuditLogWebhook(), null, _currentMessage.ToString());
                    _currentMessage.Clear();
                    _currentMessage.AppendLine(message);
                }
            }
        }

        public void RegisterEvents()
        {
            _eventHandler.OnIdentityRegistered += OnIdentityRegistered;
            _eventHandler.OnTokenCreated += OnTokenCreated;
            _eventHandler.OnTokenDeleted += OnTokenDeleted;
            _logger.LogInformation("Registered events for audit logger.");
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
    }
}
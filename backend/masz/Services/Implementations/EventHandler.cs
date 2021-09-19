using System;
using System.Threading.Tasks;
using masz.Events;
using Microsoft.Extensions.Logging;

namespace masz.Services
{
    public class EventHandler: IEventHandler
    {
        private readonly ILogger<EventHandler> _logger;
        public EventHandler(ILogger<EventHandler> logger)
        {
            _logger = logger;
        }
        private async Task Invoke<T>(AsyncEventHandler<T> eventHandler, T eventArgs) where T : EventArgs
        {
            try {
                if (eventHandler != null) {
                    await eventHandler(eventArgs);
                }
            } catch (Exception e) {
                _logger.LogError(e, $"Error while handling event '{eventHandler?.Method?.Name}'.");
            }
        }
        public async Task InvokeIdentityRegistered(IdentityRegisteredEventArgs eventArgs) => await Invoke(OnIdentityRegistered, eventArgs);
        public async Task InvokeTokenCreated(TokenCreatedEventArgs eventArgs) => await Invoke(OnTokenCreated, eventArgs);
        public async Task InvokeTokenDeleted(TokenDeletedEventArgs eventArgs) => await Invoke(OnTokenDeleted, eventArgs);
        public async Task InvokeAutoModerationConfigUpdated(AutoModerationConfigUpdatedEventArgs eventArgs) => await Invoke(OnAutoModerationConfigUpdated, eventArgs);
        public async Task InvokeAutoModerationConfigDeleted(AutoModerationConfigDeletedEventArgs eventArgs) => await Invoke(OnAutoModerationConfigDeleted, eventArgs);
        public async Task InvokeAutoModerationEventRegistered(AutoModerationEventRegisteredEventArgs eventArgs) => await Invoke(OnAutoModerationEventRegistered, eventArgs);
        public Task InvokeCaseTemplateCreated(CaseTemplateCreatedEventArgs eventArgs) => Invoke(OnCaseTemplateCreated, eventArgs);
        public Task InvokeCaseTemplateDeleted(CaseTemplateDeletedEventArgs eventArgs) => Invoke(OnCaseTemplateDeleted, eventArgs);

        public event AsyncEventHandler<IdentityRegisteredEventArgs> OnIdentityRegistered;
        public event AsyncEventHandler<TokenCreatedEventArgs> OnTokenCreated;
        public event AsyncEventHandler<TokenDeletedEventArgs> OnTokenDeleted;
        public event AsyncEventHandler<AutoModerationConfigUpdatedEventArgs> OnAutoModerationConfigUpdated;
        public event AsyncEventHandler<AutoModerationConfigDeletedEventArgs> OnAutoModerationConfigDeleted;
        public event AsyncEventHandler<AutoModerationEventRegisteredEventArgs> OnAutoModerationEventRegistered;
        public event AsyncEventHandler<CaseTemplateCreatedEventArgs> OnCaseTemplateCreated;
        public event AsyncEventHandler<CaseTemplateDeletedEventArgs> OnCaseTemplateDeleted;
    }
}
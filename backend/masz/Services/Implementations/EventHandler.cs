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
        public Task InvokeFileUploaded(FileUploadedEventArgs eventArgs) => Invoke(OnFileUploaded, eventArgs);
        public Task InvokeGuildRegistered(GuildRegisteredEventArgs eventArgs) => Invoke(OnGuildRegistered, eventArgs);
        public Task InvokeGuildUpdated(GuildUpdatedEventArgs eventArgs) => Invoke(OnGuildUpdated, eventArgs);
        public Task InvokeGuildDeleted(GuildDeletedEventArgs eventArgs) => Invoke(OnGuildDeleted, eventArgs);
        public Task InvokeGuildMotdUpdated(GuildMotdUpdatedEventArgs eventArgs) => Invoke(OnGuildMotdUpdated, eventArgs);
        public Task InvokeInviteUsageRegistered(InviteUsageRegisteredEventArgs eventArgs) => Invoke(OnInviteUsageRegistered, eventArgs);
        public Task InvokeModCaseCommentCreated(ModCaseCommentCreatedEventArgs eventArgs) => Invoke(OnModCaseCommentCreated, eventArgs);
        public Task InvokeModCaseCommentUpdated(ModCaseCommentUpdatedEventArgs eventArgs) => Invoke(OnModCaseCommentUpdated, eventArgs);
        public Task InvokeModCaseCommentDeleted(ModCaseCommentDeletedEventArgs eventArgs) => Invoke(OnModCaseCommentDeleted, eventArgs);
        public Task InvokeModCaseCreated(ModCaseCreatedEventArgs eventArgs) => Invoke(OnModCaseCreated, eventArgs);
        public Task InvokeModCaseUpdated(ModCaseUpdatedEventArgs eventArgs) => Invoke(OnModCaseUpdated, eventArgs);
        public Task InvokeModCaseDeleted(ModCaseDeletedEventArgs eventArgs) => Invoke(OnModCaseDeleted, eventArgs);
        public Task InvokeModCaseMarkedToBeDeleted(ModCaseMarkedToBeDeletedEventArgs eventArgs) => Invoke(OnModCaseMarkedToBeDeleted, eventArgs);
        public Task InvokeModCaseRestored(ModCaseRestoredEventArgs eventArgs) => Invoke(OnModCaseRestored, eventArgs);
        public Task InvokeUserMapUpdated(UserMapUpdatedEventArgs eventArgs) => Invoke(OnUserMapUpdated, eventArgs);
        public Task InvokeUserMapDeleted(UserMapDeletedEventArgs eventArgs) => Invoke(OnUserMapDeleted, eventArgs);
        public Task InvokeUserNoteUpdated(UserNoteUpdatedEventArgs eventArgs) => Invoke(OnUserNoteUpdated, eventArgs);
        public Task InvokeUserNoteDeleted(UserNoteDeletedEventArgs eventArgs) => Invoke(OnUserNoteDeleted, eventArgs);

        public event AsyncEventHandler<IdentityRegisteredEventArgs> OnIdentityRegistered;
        public event AsyncEventHandler<TokenCreatedEventArgs> OnTokenCreated;
        public event AsyncEventHandler<TokenDeletedEventArgs> OnTokenDeleted;
        public event AsyncEventHandler<AutoModerationConfigUpdatedEventArgs> OnAutoModerationConfigUpdated;
        public event AsyncEventHandler<AutoModerationConfigDeletedEventArgs> OnAutoModerationConfigDeleted;
        public event AsyncEventHandler<AutoModerationEventRegisteredEventArgs> OnAutoModerationEventRegistered;
        public event AsyncEventHandler<CaseTemplateCreatedEventArgs> OnCaseTemplateCreated;
        public event AsyncEventHandler<CaseTemplateDeletedEventArgs> OnCaseTemplateDeleted;
        public event AsyncEventHandler<FileUploadedEventArgs> OnFileUploaded;
        public event AsyncEventHandler<GuildRegisteredEventArgs> OnGuildRegistered;
        public event AsyncEventHandler<GuildUpdatedEventArgs> OnGuildUpdated;
        public event AsyncEventHandler<GuildDeletedEventArgs> OnGuildDeleted;
        public event AsyncEventHandler<GuildMotdUpdatedEventArgs> OnGuildMotdUpdated;
        public event AsyncEventHandler<InviteUsageRegisteredEventArgs> OnInviteUsageRegistered;
        public event AsyncEventHandler<ModCaseCommentCreatedEventArgs> OnModCaseCommentCreated;
        public event AsyncEventHandler<ModCaseCommentUpdatedEventArgs> OnModCaseCommentUpdated;
        public event AsyncEventHandler<ModCaseCommentDeletedEventArgs> OnModCaseCommentDeleted;
        public event AsyncEventHandler<ModCaseCreatedEventArgs> OnModCaseCreated;
        public event AsyncEventHandler<ModCaseUpdatedEventArgs> OnModCaseUpdated;
        public event AsyncEventHandler<ModCaseDeletedEventArgs> OnModCaseDeleted;
        public event AsyncEventHandler<ModCaseMarkedToBeDeletedEventArgs> OnModCaseMarkedToBeDeleted;
        public event AsyncEventHandler<ModCaseRestoredEventArgs> OnModCaseRestored;
        public event AsyncEventHandler<UserMapUpdatedEventArgs> OnUserMapUpdated;
        public event AsyncEventHandler<UserMapDeletedEventArgs> OnUserMapDeleted;
        public event AsyncEventHandler<UserNoteUpdatedEventArgs> OnUserNoteUpdated;
        public event AsyncEventHandler<UserNoteDeletedEventArgs> OnUserNoteDeleted;
    }
}
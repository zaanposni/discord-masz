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
        public async Task InvokeCaseTemplateCreated(CaseTemplateCreatedEventArgs eventArgs) => await Invoke(OnCaseTemplateCreated, eventArgs);
        public async Task InvokeCaseTemplateDeleted(CaseTemplateDeletedEventArgs eventArgs) => await Invoke(OnCaseTemplateDeleted, eventArgs);
        public async Task InvokeFileUploaded(FileUploadedEventArgs eventArgs) => await Invoke(OnFileUploaded, eventArgs);
        public async Task InvokeGuildRegistered(GuildRegisteredEventArgs eventArgs) => await Invoke(OnGuildRegistered, eventArgs);
        public async Task InvokeGuildUpdated(GuildUpdatedEventArgs eventArgs) => await Invoke(OnGuildUpdated, eventArgs);
        public async Task InvokeGuildDeleted(GuildDeletedEventArgs eventArgs) => await Invoke(OnGuildDeleted, eventArgs);
        public async Task InvokeGuildMotdUpdated(GuildMotdUpdatedEventArgs eventArgs) => await Invoke(OnGuildMotdUpdated, eventArgs);
        public async Task InvokeInviteUsageRegistered(InviteUsageRegisteredEventArgs eventArgs) => await Invoke(OnInviteUsageRegistered, eventArgs);
        public async Task InvokeModCaseCommentCreated(ModCaseCommentCreatedEventArgs eventArgs) => await Invoke(OnModCaseCommentCreated, eventArgs);
        public async Task InvokeModCaseCommentUpdated(ModCaseCommentUpdatedEventArgs eventArgs) => await Invoke(OnModCaseCommentUpdated, eventArgs);
        public async Task InvokeModCaseCommentDeleted(ModCaseCommentDeletedEventArgs eventArgs) => await Invoke(OnModCaseCommentDeleted, eventArgs);
        public async Task InvokeModCaseCreated(ModCaseCreatedEventArgs eventArgs) => await Invoke(OnModCaseCreated, eventArgs);
        public async Task InvokeModCaseUpdated(ModCaseUpdatedEventArgs eventArgs) => await Invoke(OnModCaseUpdated, eventArgs);
        public async Task InvokeModCaseDeleted(ModCaseDeletedEventArgs eventArgs) => await Invoke(OnModCaseDeleted, eventArgs);
        public async Task InvokeModCaseMarkedToBeDeleted(ModCaseMarkedToBeDeletedEventArgs eventArgs) => await Invoke(OnModCaseMarkedToBeDeleted, eventArgs);
        public async Task InvokeModCaseRestored(ModCaseRestoredEventArgs eventArgs) => await Invoke(OnModCaseRestored, eventArgs);
        public async Task InvokeUserMapUpdated(UserMapUpdatedEventArgs eventArgs) => await Invoke(OnUserMapUpdated, eventArgs);
        public async Task InvokeUserMapDeleted(UserMapDeletedEventArgs eventArgs) => await Invoke(OnUserMapDeleted, eventArgs);
        public async Task InvokeUserNoteUpdated(UserNoteUpdatedEventArgs eventArgs) => await Invoke(OnUserNoteUpdated, eventArgs);
        public async Task InvokeUserNoteDeleted(UserNoteDeletedEventArgs eventArgs) => await Invoke(OnUserNoteDeleted, eventArgs);

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
using System.Threading.Tasks;
using masz.Events;

namespace masz.Services
{
    public interface IEventHandler
    {
        event AsyncEventHandler<IdentityRegisteredEventArgs> OnIdentityRegistered;
        event AsyncEventHandler<TokenCreatedEventArgs> OnTokenCreated;
        event AsyncEventHandler<TokenDeletedEventArgs> OnTokenDeleted;
        event AsyncEventHandler<AutoModerationConfigUpdatedEventArgs> OnAutoModerationConfigUpdated;
        event AsyncEventHandler<AutoModerationConfigDeletedEventArgs> OnAutoModerationConfigDeleted;
        event AsyncEventHandler<AutoModerationEventRegisteredEventArgs> OnAutoModerationEventRegistered;
        event AsyncEventHandler<CaseTemplateCreatedEventArgs> OnCaseTemplateCreated;
        event AsyncEventHandler<CaseTemplateDeletedEventArgs> OnCaseTemplateDeleted;
        event AsyncEventHandler<FileUploadedEventArgs> OnFileUploaded;
        event AsyncEventHandler<GuildRegisteredEventArgs> OnGuildRegistered;
        event AsyncEventHandler<GuildUpdatedEventArgs> OnGuildUpdated;
        event AsyncEventHandler<GuildDeletedEventArgs> OnGuildDeleted;
        event AsyncEventHandler<GuildMotdUpdatedEventArgs> OnGuildMotdUpdated;
        event AsyncEventHandler<InviteUsageRegisteredEventArgs> OnInviteUsageRegistered;
        event AsyncEventHandler<ModCaseCommentCreatedEventArgs> OnModCaseCommentCreated;
        event AsyncEventHandler<ModCaseCommentUpdatedEventArgs> OnModCaseCommentUpdated;
        event AsyncEventHandler<ModCaseCommentDeletedEventArgs> OnModCaseCommentDeleted;
        event AsyncEventHandler<ModCaseCreatedEventArgs> OnModCaseCreated;
        event AsyncEventHandler<ModCaseUpdatedEventArgs> OnModCaseUpdated;
        event AsyncEventHandler<ModCaseDeletedEventArgs> OnModCaseDeleted;
        event AsyncEventHandler<ModCaseMarkedToBeDeletedEventArgs> OnModCaseMarkedToBeDeleted;
        event AsyncEventHandler<ModCaseRestoredEventArgs> OnModCaseRestored;
        event AsyncEventHandler<UserMapUpdatedEventArgs> OnUserMapUpdated;
        event AsyncEventHandler<UserMapDeletedEventArgs> OnUserMapDeleted;
        event AsyncEventHandler<UserNoteUpdatedEventArgs> OnUserNoteUpdated;
        event AsyncEventHandler<UserNoteDeletedEventArgs> OnUserNoteDeleted;

        Task InvokeIdentityRegistered(IdentityRegisteredEventArgs eventArgs);
        Task InvokeTokenCreated(TokenCreatedEventArgs eventArgs);
        Task InvokeTokenDeleted(TokenDeletedEventArgs eventArgs);
        Task InvokeAutoModerationConfigUpdated(AutoModerationConfigUpdatedEventArgs eventArgs);
        Task InvokeAutoModerationConfigDeleted(AutoModerationConfigDeletedEventArgs eventArgs);
        Task InvokeAutoModerationEventRegistered(AutoModerationEventRegisteredEventArgs eventArgs);
        Task InvokeCaseTemplateCreated(CaseTemplateCreatedEventArgs eventArgs);
        Task InvokeCaseTemplateDeleted(CaseTemplateDeletedEventArgs eventArgs);
        Task InvokeFileUploaded(FileUploadedEventArgs eventArgs);
        Task InvokeGuildRegistered(GuildRegisteredEventArgs eventArgs);
        Task InvokeGuildUpdated(GuildUpdatedEventArgs eventArgs);
        Task InvokeGuildDeleted(GuildDeletedEventArgs eventArgs);
        Task InvokeGuildMotdUpdated(GuildMotdUpdatedEventArgs eventArgs);
        Task InvokeInviteUsageRegistered(InviteUsageRegisteredEventArgs eventArgs);
        Task InvokeModCaseCommentCreated(ModCaseCommentCreatedEventArgs eventArgs);
        Task InvokeModCaseCommentUpdated(ModCaseCommentUpdatedEventArgs eventArgs);
        Task InvokeModCaseCommentDeleted(ModCaseCommentDeletedEventArgs eventArgs);
        Task InvokeModCaseCreated(ModCaseCreatedEventArgs eventArgs);
        Task InvokeModCaseUpdated(ModCaseUpdatedEventArgs eventArgs);
        Task InvokeModCaseDeleted(ModCaseDeletedEventArgs eventArgs);
        Task InvokeModCaseMarkedToBeDeleted(ModCaseMarkedToBeDeletedEventArgs eventArgs);
        Task InvokeModCaseRestored(ModCaseRestoredEventArgs eventArgs);
        Task InvokeUserMapUpdated(UserMapUpdatedEventArgs eventArgs);
        Task InvokeUserMapDeleted(UserMapDeletedEventArgs eventArgs);
        Task InvokeUserNoteUpdated(UserNoteUpdatedEventArgs eventArgs);
        Task InvokeUserNoteDeleted(UserNoteDeletedEventArgs eventArgs);
    }
}

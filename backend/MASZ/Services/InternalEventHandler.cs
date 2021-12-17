using MASZ.Models;
using MASZ.Utils;

namespace MASZ.Services
{
    public class InternalEventHandler
    {

        public event Func<Identity, Task> OnIdentityRegistered
        {
            add { OnIdentityRegisteredEvent.Add(value); }
            remove { OnIdentityRegisteredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<Identity, Task>> OnIdentityRegisteredEvent = new();

        public event Func<APIToken, Task> OnTokenCreated
        {
            add { OnTokenCreatedEvent.Add(value); }
            remove { OnTokenCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<APIToken, Task>> OnTokenCreatedEvent = new();

        public event Func<APIToken, Task> OnTokenDeleted
        {
            add { OnTokenDeletedEvent.Add(value); }
            remove { OnTokenDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<APIToken, Task>> OnTokenDeletedEvent = new();

        public event Func<AutoModerationConfig, Task> OnAutoModerationConfigUpdated
        {
            add { OnAutoModerationConfigUpdatedEvent.Add(value); }
            remove { OnAutoModerationConfigUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationConfig, Task>> OnAutoModerationConfigUpdatedEvent = new();

        public event Func<AutoModerationConfig, Task> OnAutoModerationConfigDeleted
        {
            add { OnAutoModerationConfigDeletedEvent.Add(value); }
            remove { OnAutoModerationConfigDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationConfig, Task>> OnAutoModerationConfigDeletedEvent = new();

        public event Func<AutoModerationEvent, Task> OnAutoModerationEventRegistered
        {
            add { OnAutoModerationEventRegisteredEvent.Add(value); }
            remove { OnAutoModerationEventRegisteredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationEvent, Task>> OnAutoModerationEventRegisteredEvent = new();

        public event Func<CaseTemplate, Task> OnCaseTemplateCreated
        {
            add { OnCaseTemplateCreatedEvent.Add(value); }
            remove { OnCaseTemplateCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<CaseTemplate, Task>> OnCaseTemplateCreatedEvent = new();

        public event Func<CaseTemplate, Task> OnCaseTemplateDeleted
        {
            add { OnCaseTemplateDeletedEvent.Add(value); }
            remove { OnCaseTemplateDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<CaseTemplate, Task>> OnCaseTemplateDeletedEvent = new();

        public event Func<UploadedFile, Task> OnFileUploaded
        {
            add { OnFileUploadedEvent.Add(value); }
            remove { OnFileUploadedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UploadedFile, Task>> OnFileUploadedEvent = new();

        public event Func<GuildConfig, Task> OnGuildRegistered
        {
            add { OnGuildRegisteredEvent.Add(value); }
            remove { OnGuildRegisteredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildConfig, Task>> OnGuildRegisteredEvent = new();

        public event Func<GuildConfig, Task> OnGuildUpdated
        {
            add { OnGuildUpdatedEvent.Add(value); }
            remove { OnGuildUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildConfig, Task>> OnGuildUpdatedEvent = new();

        public event Func<GuildConfig, Task> OnGuildDeleted
        {
            add { OnGuildDeletedEvent.Add(value); }
            remove { OnGuildDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildConfig, Task>> OnGuildDeletedEvent = new();

        public event Func<GuildMotd, Task> OnGuildMotdUpdated
        {
            add { OnGuildMotdUpdatedEvent.Add(value); }
            remove { OnGuildMotdUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildMotd, Task>> OnGuildMotdUpdatedEvent = new();

        public event Func<UserInvite, Task> OnInviteUsageRegistered
        {
            add { OnInviteUsageRegisteredEvent.Add(value); }
            remove { OnInviteUsageRegisteredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserInvite, Task>> OnInviteUsageRegisteredEvent = new();

        public event Func<ModCaseComment, Task> OnModCaseCommentCreated
        {
            add { OnModCaseCommentCreatedEvent.Add(value); }
            remove { OnModCaseCommentCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCaseComment, Task>> OnModCaseCommentCreatedEvent = new();

        public event Func<ModCaseComment, Task> OnModCaseCommentUpdated
        {
            add { OnModCaseCommentUpdatedEvent.Add(value); }
            remove { OnModCaseCommentUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCaseComment, Task>> OnModCaseCommentUpdatedEvent = new();

        public event Func<ModCaseComment, Task> OnModCaseCommentDeleted
        {
            add { OnModCaseCommentDeletedEvent.Add(value); }
            remove { OnModCaseCommentDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCaseComment, Task>> OnModCaseCommentDeletedEvent = new();

        public event Func<ModCase, Task> OnModCaseCreated
        {
            add { OnModCaseCreatedEvent.Add(value); }
            remove { OnModCaseCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, Task>> OnModCaseCreatedEvent = new();

        public event Func<ModCase, Task> OnModCaseUpdated
        {
            add { OnModCaseUpdatedEvent.Add(value); }
            remove { OnModCaseUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, Task>> OnModCaseUpdatedEvent = new();

        public event Func<ModCase, Task> OnModCaseDeleted
        {
            add { OnModCaseDeletedEvent.Add(value); }
            remove { OnModCaseDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, Task>> OnModCaseDeletedEvent = new();

        public event Func<ModCase, Task> OnModCaseMarkedToBeDeleted
        {
            add { OnModCaseMarkedToBeDeletedEvent.Add(value); }
            remove { OnModCaseMarkedToBeDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, Task>> OnModCaseMarkedToBeDeletedEvent = new();

        public event Func<ModCase, Task> OnModCaseRestored
        {
            add { OnModCaseRestoredEvent.Add(value); }
            remove { OnModCaseRestoredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, Task>> OnModCaseRestoredEvent = new();

        public event Func<UserMapping, Task> OnUserMapUpdated
        {
            add { OnUserMapUpdatedEvent.Add(value); }
            remove { OnUserMapUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserMapping, Task>> OnUserMapUpdatedEvent = new();

        public event Func<UserMapping, Task> OnUserMapDeleted
        {
            add { OnUserMapDeletedEvent.Add(value); }
            remove { OnUserMapDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserMapping, Task>> OnUserMapDeletedEvent = new();

        public event Func<UserNote, Task> OnUserNoteDeleted
        {
            add { OnUserNoteDeletedEvent.Add(value); }
            remove { OnUserNoteDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserNote, Task>> OnUserNoteDeletedEvent = new();

        public event Func<UserNote, Task> OnUserNoteUpdated
        {
            add { OnUserNoteUpdatedEvent.Add(value); }
            remove { OnUserNoteUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserNote, Task>> OnUserNoteUpdatedEvent = new();

        public event Func<int, DateTime, Task> OnInternalCachingDone
        {
            add { OnInternalCachingDoneEvent.Add(value); }
            remove { OnInternalCachingDoneEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<int, DateTime, Task>> OnInternalCachingDoneEvent = new();

        public event Func<GuildLevelAuditLogConfig, Task> OnGuildLevelAuditLogConfigUpdated
        {
            add { OnGuildLevelAuditLogConfigUpdatedEvent.Add(value); }
            remove { OnGuildLevelAuditLogConfigUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildLevelAuditLogConfig, Task>> OnGuildLevelAuditLogConfigUpdatedEvent = new();

        public event Func<GuildLevelAuditLogConfig, Task> OnGuildLevelAuditLogConfigDeleted
        {
            add { OnGuildLevelAuditLogConfigDeletedEvent.Add(value); }
            remove { OnGuildLevelAuditLogConfigDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildLevelAuditLogConfig, Task>> OnGuildLevelAuditLogConfigDeletedEvent = new();

    }
}
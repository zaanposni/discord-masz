using Discord;
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

        public event Func<AutoModerationConfig, IUser, Task> OnAutoModerationConfigCreated
        {
            add { OnAutoModerationConfigCreatedEvent.Add(value); }
            remove { OnAutoModerationConfigCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationConfig, IUser, Task>> OnAutoModerationConfigCreatedEvent = new();

        public event Func<AutoModerationConfig, IUser, Task> OnAutoModerationConfigUpdated
        {
            add { OnAutoModerationConfigUpdatedEvent.Add(value); }
            remove { OnAutoModerationConfigUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationConfig, IUser, Task>> OnAutoModerationConfigUpdatedEvent = new();

        public event Func<AutoModerationConfig, IUser, Task> OnAutoModerationConfigDeleted
        {
            add { OnAutoModerationConfigDeletedEvent.Add(value); }
            remove { OnAutoModerationConfigDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationConfig, IUser, Task>> OnAutoModerationConfigDeletedEvent = new();


        public event Func<AutoModerationEvent, AutoModerationConfig, GuildConfig, ITextChannel, IUser, Task> OnAutoModerationEventRegistered
        {
            add { OnAutoModerationEventRegisteredEvent.Add(value); }
            remove { OnAutoModerationEventRegisteredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<AutoModerationEvent, AutoModerationConfig, GuildConfig, ITextChannel, IUser, Task>> OnAutoModerationEventRegisteredEvent = new();

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

        /// <summary>
        /// Invoked when a file is uploaded
        /// </summary>
        /// <typeparam name="UploadedFile">The created file.</typeparam>
        /// <typeparam name="ModCase">The modcase the file was uploaded to.</typeparam>
        /// <typeparam name="IUser">The actor who created the file.</typeparam>
        public event Func<UploadedFile, ModCase, IUser, Task> OnFileUploaded
        {
            add { OnFileUploadedEvent.Add(value); }
            remove { OnFileUploadedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UploadedFile, ModCase, IUser, Task>> OnFileUploadedEvent = new();

        /// <summary>
        /// Invoked when a file is deleted
        /// </summary>
        /// <typeparam name="UploadedFile">The deleted file.</typeparam>
        /// <typeparam name="ModCase">The modcase the file was deleted fropm.</typeparam>
        /// <typeparam name="IUser">The actor who deleted the file.</typeparam>
        public event Func<UploadedFile, ModCase, IUser, Task> OnFileDeleted
        {
            add { OnFileDeletedEvent.Add(value); }
            remove { OnFileDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UploadedFile, ModCase, IUser, Task>> OnFileDeletedEvent = new();

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

        public event Func<GuildMotd, IUser, Task> OnGuildMotdCreated
        {
            add { OnGuildMotdCreatedEvent.Add(value); }
            remove { OnGuildMotdCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildMotd, IUser, Task>> OnGuildMotdCreatedEvent = new();

        public event Func<GuildMotd, IUser, Task> OnGuildMotdUpdated
        {
            add { OnGuildMotdUpdatedEvent.Add(value); }
            remove { OnGuildMotdUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildMotd, IUser, Task>> OnGuildMotdUpdatedEvent = new();

        public event Func<UserInvite, Task> OnInviteUsageRegistered
        {
            add { OnInviteUsageRegisteredEvent.Add(value); }
            remove { OnInviteUsageRegisteredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserInvite, Task>> OnInviteUsageRegisteredEvent = new();

        /// <summary>
        /// Invoked when a modcase comment is created
        /// </summary>
        /// <typeparam name="ModCase">The created comment.</typeparam>
        /// <typeparam name="IUser">The actor who created the comment.</typeparam>
        public event Func<ModCaseComment, IUser, Task> OnModCaseCommentCreated
        {
            add { OnModCaseCommentCreatedEvent.Add(value); }
            remove { OnModCaseCommentCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCaseComment, IUser, Task>> OnModCaseCommentCreatedEvent = new();

        /// <summary>
        /// Invoked when a modcase comment is updated
        /// </summary>
        /// <typeparam name="ModCase">The updated comment.</typeparam>
        /// <typeparam name="IUser">The actor who updated the comment.</typeparam>
        public event Func<ModCaseComment, IUser, Task> OnModCaseCommentUpdated
        {
            add { OnModCaseCommentUpdatedEvent.Add(value); }
            remove { OnModCaseCommentUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCaseComment, IUser, Task>> OnModCaseCommentUpdatedEvent = new();

        /// <summary>
        /// Invoked when a modcase comment is deleted
        /// </summary>
        /// <typeparam name="ModCase">The deleted comment.</typeparam>
        /// <typeparam name="IUser">The actor who deleted the comment.</typeparam>
        public event Func<ModCaseComment, IUser, Task> OnModCaseCommentDeleted
        {
            add { OnModCaseCommentDeletedEvent.Add(value); }
            remove { OnModCaseCommentDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCaseComment, IUser, Task>> OnModCaseCommentDeletedEvent = new();

        /// <summary>
        /// Invoked when a modcase is created
        /// </summary>
        /// <typeparam name="ModCase">The created ModCase.</typeparam>
        /// <typeparam name="IUser">The actor who created the ModCase.</typeparam>
        /// <typeparam name="bool">The announcePublic flag.</typeparam>
        /// <typeparam name="bool">The announceDM flag.</typeparam>
        public event Func<ModCase, IUser, bool, bool, Task> OnModCaseCreated
        {
            add { OnModCaseCreatedEvent.Add(value); }
            remove { OnModCaseCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, IUser, bool, bool, Task>> OnModCaseCreatedEvent = new();

        /// <summary>
        /// Invoked when a modcase is updated
        /// </summary>
        /// <typeparam name="ModCase">The updated ModCase.</typeparam>
        /// <typeparam name="IUser">The actor who updated the ModCase.</typeparam>
        /// <typeparam name="bool">The announcePublic flag.</typeparam>
        /// <typeparam name="bool">The announceDM flag.</typeparam>
        public event Func<ModCase, IUser, bool, bool, Task> OnModCaseUpdated
        {
            add { OnModCaseUpdatedEvent.Add(value); }
            remove { OnModCaseUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, IUser, bool, bool, Task>> OnModCaseUpdatedEvent = new();

        /// <summary>
        /// Invoked when a modcase is (force) deleted
        /// </summary>
        /// <typeparam name="ModCase">The deleted ModCase.</typeparam>
        /// <typeparam name="IUser">The actor who deleted the ModCase.</typeparam>
        /// <typeparam name="bool">The announcePublic flag.</typeparam>
        /// <typeparam name="bool">The announceDM flag.</typeparam>
        public event Func<ModCase, IUser, bool, bool, Task> OnModCaseDeleted
        {
            add { OnModCaseDeletedEvent.Add(value); }
            remove { OnModCaseDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, IUser, bool, bool, Task>> OnModCaseDeletedEvent = new();

        /// <summary>
        /// Invoked when a modcase is marked to be deleted
        /// </summary>
        /// <typeparam name="ModCase">The marked ModCase.</typeparam>
        /// <typeparam name="IUser">The actor who marked the ModCase.</typeparam>
        /// <typeparam name="bool">The announcePublic flag.</typeparam>
        /// <typeparam name="bool">The announceDM flag.</typeparam>
        public event Func<ModCase, IUser, bool, bool, Task> OnModCaseMarkedToBeDeleted
        {
            add { OnModCaseMarkedToBeDeletedEvent.Add(value); }
            remove { OnModCaseMarkedToBeDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, IUser, bool, bool, Task>> OnModCaseMarkedToBeDeletedEvent = new();

        public event Func<ModCase, Task> OnModCaseRestored
        {
            add { OnModCaseRestoredEvent.Add(value); }
            remove { OnModCaseRestoredEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<ModCase, Task>> OnModCaseRestoredEvent = new();

        /// <summary>
        /// Invoked when a usermap is created
        /// </summary>
        /// <typeparam name="UserMapping">The created usermap.</typeparam>
        /// <typeparam name="IUser">The actor who created the usermap.</typeparam>
        public event Func<UserMapping, IUser, Task> OnUserMapCreated
        {
            add { OnUserMapCreatedEvent.Add(value); }
            remove { OnUserMapCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserMapping, IUser, Task>> OnUserMapCreatedEvent = new();

        /// <summary>
        /// Invoked when a usermap is updated
        /// </summary>
        /// <typeparam name="UserMapping">The updated usermap.</typeparam>
        /// <typeparam name="IUser">The actor who updated the usermap.</typeparam>
        public event Func<UserMapping, IUser, Task> OnUserMapUpdated
        {
            add { OnUserMapUpdatedEvent.Add(value); }
            remove { OnUserMapUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserMapping, IUser, Task>> OnUserMapUpdatedEvent = new();

        /// <summary>
        /// Invoked when a usermap is deleted
        /// </summary>
        /// <typeparam name="UserMapping">The deleted usermap.</typeparam>
        /// <typeparam name="IUser">The actor who deleted the usermap.</typeparam>
        public event Func<UserMapping, IUser, Task> OnUserMapDeleted
        {
            add { OnUserMapDeletedEvent.Add(value); }
            remove { OnUserMapDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserMapping, IUser, Task>> OnUserMapDeletedEvent = new();

        /// <summary>
        /// Invoked when a usernote is deleted
        /// </summary>
        /// <typeparam name="UserNote">The deleted usernote.</typeparam>
        /// <typeparam name="IUser">The actor who deleted the usernote.</typeparam>
        public event Func<UserNote, IUser, Task> OnUserNoteDeleted
        {
            add { OnUserNoteDeletedEvent.Add(value); }
            remove { OnUserNoteDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserNote, IUser, Task>> OnUserNoteDeletedEvent = new();

        /// <summary>
        /// Invoked when a usernote is updated
        /// </summary>
        /// <typeparam name="UserNote">The updated usernote.</typeparam>
        /// <typeparam name="IUser">The actor who updated the usernote.</typeparam>
        public event Func<UserNote, IUser, Task> OnUserNoteUpdated
        {
            add { OnUserNoteUpdatedEvent.Add(value); }
            remove { OnUserNoteUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserNote, IUser, Task>> OnUserNoteUpdatedEvent = new();

        /// <summary>
        /// Invoked when a usernote is created
        /// </summary>
        /// <typeparam name="UserNote">The created usernote.</typeparam>
        /// <typeparam name="IUser">The actor who created the usernote.</typeparam>
        public event Func<UserNote, IUser, Task> OnUserNoteCreated
        {
            add { OnUserNoteCreatedEvent.Add(value); }
            remove { OnUserNoteCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<UserNote, IUser, Task>> OnUserNoteCreatedEvent = new();

        public event Func<int, DateTime, Task> OnInternalCachingDone
        {
            add { OnInternalCachingDoneEvent.Add(value); }
            remove { OnInternalCachingDoneEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<int, DateTime, Task>> OnInternalCachingDoneEvent = new();

        public event Func<GuildLevelAuditLogConfig, IUser, Task> OnGuildLevelAuditLogConfigCreated
        {
            add { OnGuildLevelAuditLogConfigCreatedEvent.Add(value); }
            remove { OnGuildLevelAuditLogConfigCreatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildLevelAuditLogConfig, IUser, Task>> OnGuildLevelAuditLogConfigCreatedEvent = new();

        public event Func<GuildLevelAuditLogConfig, IUser, Task> OnGuildLevelAuditLogConfigUpdated
        {
            add { OnGuildLevelAuditLogConfigUpdatedEvent.Add(value); }
            remove { OnGuildLevelAuditLogConfigUpdatedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildLevelAuditLogConfig, IUser, Task>> OnGuildLevelAuditLogConfigUpdatedEvent = new();

        public event Func<GuildLevelAuditLogConfig, IUser, Task> OnGuildLevelAuditLogConfigDeleted
        {
            add { OnGuildLevelAuditLogConfigDeletedEvent.Add(value); }
            remove { OnGuildLevelAuditLogConfigDeletedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<GuildLevelAuditLogConfig, IUser, Task>> OnGuildLevelAuditLogConfigDeletedEvent = new();

    }
}
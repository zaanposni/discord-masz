namespace MASZ.Dtos
{
    public class TelemetryDataGuildFeatureUsageDto
    {
        public string HashedServer { get; set; }
        public string HashedGuildId { get; set; }

        // config

        public int AdminRolesCount { get; set; }
        public int ModRolesCount { get; set; }
        public bool StrictPermissionCheckEnabled { get; set; }
        public int MutedRolesCount { get; set; }
        public bool InternalWebhookEnabled { get; set; }
        public bool PublicWebhookEnabled { get; set; }
        public bool PublicEmbedModeEnabled { get; set; }
        public bool PublishModeratorInformationEnabled { get; set; }
        public bool ExecuteWhoisOnJoinEnabled { get; set; }
        public string Language { get; set; }

        // cases

        public int ModCaseCount { get; set; }
        public int ModCaseBanCount { get; set; }
        public int ModCaseKickCount { get; set; }
        public int ModCaseWarnCount { get; set; }
        public int ModCaseMuteCount { get; set; }
        public int UniqueLabelsCount { get; set; }
        public int ModCaseImportedCount { get; set; }
        public int ModCaseAutomoderatedCount { get; set; }
        public int ModCaseByCommandCount { get; set; }
        public int ModCaseWithCommentCount { get; set; }

        // comments

        public int CommentCount { get; set; }

        // files

        public int FileCount { get; set; }
        public int FileAverageSize { get; set; }
        public int FileAllSize { get; set; }

        // automod events

        public int AutomodEventCount { get; set; }

        // appeals

        public int AppealCount { get; set; }
        public int AppealAcceptedCount { get; set; }
        public int AppealRejectedCount { get; set; }
        public int AppealPendingCount { get; set; }
        public int AppealQuestionsCount { get; set; }

        // usernotes

        public int UsernoteCount { get; set; }

        // usermaps

        public int UsermapCount { get; set; }

        // messages

        public int MessageCount { get; set; }
        public DateTime? LastMessage { get; set; }

        // motd

        public bool MotdEnabled { get; set; }

        // auditlog

        public bool AuditLogMessageSentEnabled { get; set; }
        public bool AuditLogMessageEditedEnabled { get; set; }
        public bool AuditLogMessageDeletedEnabled { get; set; }
        public bool AuditLogUsernameChangedEnabled { get; set; }
        public bool AuditLogNicknameChangedEnabled { get; set; }
        public bool AuditLogAvatarChangedEnabled { get; set; }
        public bool AuditLogMemberRolesUpdatedEnabled { get; set; }
        public bool AuditLogMemberJoinedEnabled { get; set; }
        public bool AuditLogMemberLeftEnabled { get; set; }
        public bool AuditLogMemberBannedEnabled { get; set; }
        public bool AuditLogMemberUnbannedEnabled { get; set; }
        public bool AuditLogInviteCreatedEnabled { get; set; }
        public bool AuditLogInviteDeletedEnabled { get; set; }
        public bool AuditLogThreadCreatedEnabled { get; set; }
        public bool AuditLogMemberJoinedVoiceChannelEnabled { get; set; }
        public bool AuditLogMemberLeftVoiceChannelEnabled { get; set; }
        public bool AuditLogMemberSwitchedVoiceChannelEnabled { get; set; }
        public bool AuditLogReactionAddedEnabled { get; set; }
        public bool AuditLogReactionRemovedEnabled { get; set; }

        // automod config

        public bool AutomodInviteEnabled { get; set; }
        public bool AutomodEmotesEnabled { get; set; }
        public bool AutomodMentionsEnabled { get; set; }
        public bool AutomodAttachmentsEnabled { get; set; }
        public bool AutomodEmbedsEnabled { get; set; }
        public bool AutomodLimitsEnabled { get; set; }
        public bool AutomodCustomWordsEnabled { get; set; }
        public bool AutomodSpamEnabled { get; set; }
        public bool AutomodDuplicatesEnabled { get; set; }
        public bool AutomodLinksEnabled { get; set; }

        // zalgo

        public bool ZalgoEnabled { get; set; }
        public bool ZalgoTryRenameEnabled { get; set; }

        // invites

        public int TrackedInvitesCount { get; set; }
    }
}
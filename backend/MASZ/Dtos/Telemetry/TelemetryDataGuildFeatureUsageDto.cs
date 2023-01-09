using System.Linq;
using MASZ.Enums;
using MASZ.Models;

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
        public bool AutomodPhishingLinksEnabled { get; set; }

        // zalgo

        public bool ZalgoEnabled { get; set; }
        public bool ZalgoTryRenameEnabled { get; set; }

        // invites

        public int TrackedInvitesCount { get; set; }

        // evidence

        public int EvidenceCount { get; set; }

        public TelemetryDataGuildFeatureUsageDto(
            string hashedServer,
            string hashedGuildId,
            MASZ.Models.GuildConfig guildConfig,
            List<MASZ.Models.ModCase> modCases,
            List<MASZ.Models.Appeal> appeals,
            List<MASZ.Models.AppealStructure> appealStructures,
            List<MASZ.Models.UserNote> userNotes,
            List<MASZ.Models.UserMapping> userMappings,
            List<MASZ.Models.ScheduledMessage> scheduledMessages,
            MASZ.Models.GuildMotd guildMotd,
            List<MASZ.Models.GuildLevelAuditLogConfig> auditLogConfigs,
            List<MASZ.Models.AutoModerationConfig> automodConfigs,
            List<MASZ.Models.AutoModerationEvent> automodEvents,
            MASZ.Models.ZalgoConfig zalgoConfig,
            int trackedInviteCount,
            int evidenceCount
        )
        {
            HashedServer = hashedServer;
            HashedGuildId = hashedGuildId;
            AdminRolesCount = guildConfig.AdminRoles.Count();
            ModRolesCount = guildConfig.ModRoles.Count();
            StrictPermissionCheckEnabled = guildConfig.StrictModPermissionCheck;
            MutedRolesCount = guildConfig.MutedRoles.Count();
            InternalWebhookEnabled = guildConfig.ModInternalNotificationWebhook != null;
            PublicWebhookEnabled = guildConfig.ModPublicNotificationWebhook != null;
            PublicEmbedModeEnabled = guildConfig.PublicEmbedMode;
            PublishModeratorInformationEnabled = guildConfig.PublishModeratorInfo;
            ExecuteWhoisOnJoinEnabled = guildConfig.ExecuteWhoisOnJoin;
            Language = guildConfig.PreferredLanguage.ToString();
            ModCaseCount = modCases.Count();
            ModCaseBanCount = modCases.Where(x => x.PunishmentType == PunishmentType.Ban).Count();
            ModCaseKickCount = modCases.Where(x => x.PunishmentType == PunishmentType.Kick).Count();
            ModCaseWarnCount = modCases.Where(x => x.PunishmentType == PunishmentType.Warn).Count();
            ModCaseMuteCount = modCases.Where(x => x.PunishmentType == PunishmentType.Mute).Count();
            UniqueLabelsCount = modCases.SelectMany(x => x.Labels).Distinct().Count();
            ModCaseImportedCount = modCases.Where(x => x.CreationType == CaseCreationType.Imported).Count();
            ModCaseAutomoderatedCount = modCases.Where(x => x.CreationType == CaseCreationType.AutoModeration).Count();
            ModCaseByCommandCount = modCases.Where(x => x.CreationType == CaseCreationType.ByCommand).Count();
            ModCaseWithCommentCount = modCases.Where(x => x.Comments.Count() > 0).Count();
            CommentCount = modCases.Select(x => x.Comments.Count).Sum();
            FileCount = -1;
            FileAverageSize = -1;
            FileAllSize = -1;
            AutomodEventCount = automodEvents.Count();
            AppealCount = appeals.Count();
            AppealAcceptedCount = appeals.Where(x => x.Status == AppealStatus.Approved).Count();
            AppealRejectedCount = appeals.Where(x => x.Status == AppealStatus.Declined).Count();
            AppealPendingCount = appeals.Where(x => x.Status == AppealStatus.Pending).Count();
            AppealQuestionsCount = appealStructures.Where(x => !x.Deleted).Count();
            UsernoteCount = userNotes.Count();
            UsermapCount = userMappings.Count();
            MessageCount = scheduledMessages.Count();
            LastMessage = scheduledMessages.Count() > 0 ? scheduledMessages.Where(x => x.Status == ScheduledMessageStatus.Sent).Max(x => x.ScheduledFor) : null;
            MotdEnabled = guildMotd != null && guildMotd.ShowMotd;
            AuditLogMessageSentEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.MessageSent) != null;
            AuditLogMessageEditedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.MessageUpdated) != null;
            AuditLogMessageDeletedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.MessageDeleted) != null;
            AuditLogUsernameChangedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.UsernameUpdated) != null;
            AuditLogNicknameChangedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.NicknameUpdated) != null;
            AuditLogAvatarChangedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.AvatarUpdated) != null;
            AuditLogMemberRolesUpdatedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.MemberRolesUpdated) != null;
            AuditLogMemberJoinedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.MemberJoined) != null;
            AuditLogMemberLeftEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.MemberRemoved) != null;
            AuditLogMemberBannedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.BanAdded) != null;
            AuditLogMemberUnbannedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.BanRemoved) != null;
            AuditLogInviteCreatedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.InviteCreated) != null;
            AuditLogInviteDeletedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.InviteDeleted) != null;
            AuditLogThreadCreatedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.ThreadCreated) != null;
            AuditLogMemberJoinedVoiceChannelEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.VoiceJoined) != null;
            AuditLogMemberLeftVoiceChannelEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.VoiceLeft) != null;
            AuditLogMemberSwitchedVoiceChannelEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.VoiceMoved) != null;
            AuditLogReactionAddedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.ReactionAdded) != null;
            AuditLogReactionRemovedEnabled = auditLogConfigs.Find(x => x.GuildAuditLogEvent == GuildAuditLogEvent.ReactionRemoved) != null;
            AutomodInviteEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.InvitePosted) != null;
            AutomodEmotesEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyEmotes) != null;
            AutomodMentionsEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyMentions) != null;
            AutomodAttachmentsEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyAttachments) != null;
            AutomodEmbedsEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyEmbeds) != null;
            AutomodLimitsEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyAutoModerations) != null;
            AutomodCustomWordsEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.CustomWordFilter) != null;
            AutomodSpamEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyMessages) != null;
            AutomodDuplicatesEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyDuplicatedCharacters) != null;
            AutomodLinksEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyLinks) != null;
            AutomodPhishingLinksEnabled = automodConfigs.Find(x => x.AutoModerationType == AutoModerationType.TooManyPhishingLinks) != null;
            ZalgoEnabled = zalgoConfig != null && zalgoConfig.Enabled;
            ZalgoTryRenameEnabled = zalgoConfig != null && zalgoConfig.renameNormal;
            TrackedInvitesCount = trackedInviteCount;
            EvidenceCount = evidenceCount;
        }
    }
}
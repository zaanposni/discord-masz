using masz.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IDatabase
    {
        Task SaveChangesAsync();
        Task<bool> CanConnectAsync();

        Task<GuildConfig> SelectSpecificGuildConfig(ulong guildId);
        Task<List<GuildConfig>> SelectAllGuildConfigs();
        void DeleteSpecificGuildConfig(GuildConfig guildConfig);
        void UpdateGuildConfig(GuildConfig guildConfig);
        Task SaveGuildConfig(GuildConfig guildConfig);
        Task<int> CountAllGuildConfigs();


        Task<int> GetHighestCaseIdForGuild(ulong guildId);
        Task<ModCase> SelectSpecificModCase(ulong guildId, int modCaseId);
        Task<List<ModCase>> SelectAllModcasesMarkedAsDeleted();
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(ulong guildId, ulong userId);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(ulong guildId, ulong userId, ModcaseTableType tableType);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(ulong guildId, ulong userId, int startPage, int pageSize);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(ulong guildId, ulong userId, int startPage, int pageSize, ModcaseTableType tableType);
        Task<List<ModCase>> SelectAllModCasesForGuild(ulong guildId);
        Task<List<ModCase>> SelectAllModCasesForGuild(ulong guildId, ModcaseTableType tableType);
        Task<List<ModCase>> SelectAllModCasesForGuild(ulong guildId, int startPage, int pageSize);
        Task<List<ModCase>> SelectAllModCasesForGuild(ulong guildId, int startPage, int pageSize, ModcaseTableType tableType);
        Task<int> CountAllModCases();
        Task<int> CountAllModCasesForGuild(ulong guildId);
        Task<int> CountAllActivePunishmentsForGuild(ulong guildId);
        Task<int> CountAllActivePunishmentsForGuild(ulong guildId, PunishmentType type);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishmentForGuild(ulong guildId);
        Task<List<ModCase>> SelectAllModCasesWithActiveMuteForGuildAndUser(ulong guildId, ulong userId);
        Task<List<ModCase>> SelectAllModCasesThatHaveParallelPunishment(ModCase modCase);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishments();
        Task<List<ModCase>> SelectAllModCases();
        Task<List<ModCase>> SelectAllModCasesForSpecificUser(ulong userId);
        Task<List<ModCase>> SelectLatestModCases(DateTime timeLimit, int limit);
        Task<List<DbCount>> GetCaseCountGraph(ulong guildId, DateTime since);
        Task<List<DbCount>> GetPunishmentCountGraph(ulong guildId, DateTime since);
        Task DeleteAllModCasesForGuild(ulong guildId);
        void DeleteSpecificModCase(ModCase modCase);
        void UpdateModCase(ModCase modCase);
        Task SaveModCase(ModCase modCase);


        Task<int> CountAllModerationEvents();
        Task<List<DbCount>> GetModerationCountGraph(ulong guildId, DateTime since);
        Task<List<AutoModerationTypeSplit>> GetModerationSplitGraph(ulong guildId, DateTime since);
        Task<int> CountAllModerationEventsForGuild(ulong guildId);
        Task<int> CountAllModerationEventsForSpecificUserOnGuild(ulong guildId, ulong userId);
        Task<List<AutoModerationEvent>> SelectAllModerationEvents();
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUser(ulong userId);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(ulong guildId);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(ulong guildId, int startPage, int pageSize);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUserOnGuild(ulong guildId, ulong userId, int startPage, int pageSize);
        Task DeleteAllModerationEventsForGuild(ulong guildId);
        Task SaveModerationEvent(AutoModerationEvent modEvent);


        Task<List<AutoModerationConfig>> SelectAllModerationConfigsForGuild(ulong guildId);
        Task<AutoModerationConfig> SelectModerationConfigForGuildAndType(ulong guildId, AutoModerationType type);
        void PutModerationConfig(AutoModerationConfig modConfig);
        void DeleteSpecificModerationConfig(AutoModerationConfig modConfig);
        Task DeleteAllModerationConfigsForGuild(ulong guildId);


        Task SaveModCaseComment(ModCaseComment comment);
        void UpdateModCaseComment(ModCaseComment comment);
        void DeleteSpecificModCaseComment(ModCaseComment comment);
        Task<ModCaseComment> SelectSpecificModCaseComment(int commentId);
        Task<List<ModCaseComment>> SelectLastModCaseCommentsByGuild(ulong guildId);
        Task<int> CountCommentsForGuild(ulong guildId);

        Task SaveCaseTemplate(CaseTemplate template);
        void DeleteSpecificCaseTemplate(CaseTemplate template);
        Task<CaseTemplate> GetSpecificCaseTemplate(int templateId);
        Task<List<CaseTemplate>> GetAllCaseTemplates();
        Task<List<CaseTemplate>> GetAllTemplatesFromUser(ulong userId);
        Task DeleteAllTemplatesForGuild(ulong guildId);
        Task<int> CountAllCaseTemplates();

        Task<GuildMotd> GetMotdForGuild(ulong guildId);
        void SaveMotd(GuildMotd motd);
        Task DeleteMotdForGuild(ulong guildId);

        Task SaveToken(APIToken token);
        void DeleteToken(APIToken token);
        Task<int> CountAllAPITokens();
        Task<List<APIToken>> GetAllAPIToken();
        Task<APIToken> GetAPIToken();
        Task<APIToken> GetAPIToken(int id);

        Task<List<UserInvite>> GetInvitedUsersByUser(ulong userId);
        Task<List<UserInvite>> GetInvitedUsersByUserAndGuild(ulong userId, ulong guildId);
        Task<List<UserInvite>> GetUsedInvitesByUser(ulong userId);
        Task<List<UserInvite>> GetUsedInvitesByUserAndGuild(ulong userId, ulong guildId);
        Task<List<UserInvite>> GetInvitesByCode(string code);
        Task SaveInvite(UserInvite invite);
        Task<int> CountTrackedInvites();
        Task<int> CountTrackedInvitesForGuild(ulong guildId);
        Task DeleteInviteHistoryByGuild(ulong guildId);

        Task<List<UserMapping>> SelectLatestUserMappings(DateTime timeLimit, int limit);
        Task<UserMapping> GetUserMappingById(int id);
        Task<List<UserMapping>> GetUserMappingsByUserId(ulong userId);
        Task<List<UserMapping>> GetUserMappingsByUserIdAndGuildId(ulong userId, ulong guildId);
        Task<UserMapping> GetUserMappingByUserIdsAndGuildId(ulong userAId, ulong userBId, ulong guildId);
        Task<List<UserMapping>> GetUserMappingsByGuildId(ulong guildId);
        Task<int> CountUserMappings();
        Task<int> CountUserMappingsForGuild(ulong guildId);
        void DeleteUserMapping(UserMapping userMapping);
        void SaveUserMapping(UserMapping userMapping);
        Task DeleteUserMappingByGuild(ulong guildId);

        Task<List<UserNote>> SelectLatestUserNotes(DateTime timeLimit, int limit);
        Task<UserNote> GetUserNoteById(int id);
        Task<List<UserNote>> GetUserNotesByUserId(ulong userId);
        Task<List<UserNote>> GetUserNotesByGuildId(ulong guildId);
        Task<UserNote> GetUserNoteByUserIdAndGuildId(ulong userId, ulong guildId);
        Task<int> CountUserNotes();
        Task<int> CountUserNotesForGuild(ulong guildId);
        void DeleteUserNote(UserNote userNote);
        void SaveUserNote(UserNote userNote);
        Task DeleteUserNoteByGuild(ulong guildId);
    }
}

using masz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public interface IDatabase
    {
        Task SaveChangesAsync();

        Task<GuildConfig> SelectSpecificGuildConfig(string guildId);
        Task<List<GuildConfig>> SelectAllGuildConfigs();
        void DeleteSpecificGuildConfig(GuildConfig guildConfig);
        void UpdateGuildConfig(GuildConfig guildConfig);
        Task SaveGuildConfig(GuildConfig guildConfig);
        Task<int> CountAllGuildConfigs();


        Task<int> GetHighestCaseIdForGuild(string guildId);
        Task<ModCase> SelectSpecificModCase(string guildId, string modCaseId);
        Task<List<ModCase>> SelectAllModcasesMarkedAsDeleted();
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, ModcaseTableType tableType);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize, ModcaseTableType tableType);
        Task<List<ModCase>> SelectAllModCasesForGuild(string guildId);
        Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, ModcaseTableType tableType);
        Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, int startPage, int pageSize);
        Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, int startPage, int pageSize, ModcaseTableType tableType);
        Task<int> CountAllModCases();
        Task<int> CountAllModCasesForGuild(string guildId);
        Task<int> CountAllActivePunishmentsForGuild(string guildId);
        Task<int> CountAllActivePunishmentsForGuild(string guildId, PunishmentType type);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishmentForGuild(string guildId);
        Task<List<ModCase>> SelectAllModCasesThatHaveParallelPunishment(ModCase modCase);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishments();
        Task<List<ModCase>> SelectAllModCases();
        Task<List<ModCase>> SelectAllModCasesForSpecificUser(string userId);
        Task<List<ModCase>> SelectLatestModCases(DateTime timeLimit, int limit);
        Task<List<DbCount>> GetCaseCountGraph(string guildId, DateTime since);
        Task<List<DbCount>> GetPunishmentCountGraph(string guildId, DateTime since);
        Task DeleteAllModCasesForGuild(string guildid);
        void DeleteSpecificModCase(ModCase modCase);
        void UpdateModCase(ModCase modCase);
        Task SaveModCase(ModCase modCase);


        Task<int> CountAllModerationEvents();
        Task<List<DbCount>> GetModerationCountGraph(string guildId, DateTime since);
        Task<List<AutoModerationTypeSplit>> GetModerationSplitGraph(string guildId, DateTime since);
        Task<int> CountAllModerationEventsForGuild(string guildId);
        Task<int> CountAllModerationEventsForSpecificUserOnGuild(string guildId, string userId);
        Task<List<AutoModerationEvent>> SelectAllModerationEvents();
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUser(string userId);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(string guildId);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(string guildId, int startPage, int pageSize);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize);
        Task DeleteAllModerationEventsForGuild(string guildid);
        Task SaveModerationEvent(AutoModerationEvent modEvent);


        Task<List<AutoModerationConfig>> SelectAllModerationConfigsForGuild(string guildId);
        Task<AutoModerationConfig> SelectModerationConfigForGuildAndType(string guildId, AutoModerationType type);
        void PutModerationConfig(AutoModerationConfig modConfig);
        void DeleteSpecificModerationConfig(AutoModerationConfig modConfig);
        Task DeleteAllModerationConfigsForGuild(string guildId);


        Task SaveModCaseComment(ModCaseComment comment);
        void UpdateModCaseComment(ModCaseComment comment);
        void DeleteSpecificModCaseComment(ModCaseComment comment);
        Task<ModCaseComment> SelectSpecificModCaseComment(int commentId);
        Task<List<ModCaseComment>> SelectLastModCaseCommentsByGuild(string guildId);

        Task SaveCaseTemplate(CaseTemplate template);
        void DeleteSpecificCaseTemplate(CaseTemplate template);
        Task<CaseTemplate> GetSpecificCaseTemplate(string templateId);
        Task<List<CaseTemplate>> GetAllCaseTemplates();
        Task<List<CaseTemplate>> GetAllTemplatesFromUser(string userId);
        Task DeleteAllTemplatesForGuild(string guildId);

        Task<GuildMotd> GetMotdForGuild(string guildId);
        void SaveMotd(GuildMotd motd);
        Task DeleteMotdForGuild(string guildId);

        Task SaveToken(APIToken token);
        void DeleteToken(APIToken token);
        Task<List<APIToken>> GetAllAPIToken();
        Task<APIToken> GetAPIToken();
        Task<APIToken> GetAPIToken(int id);

        Task<List<UserInvite>> GetInvitedUsersByUserId(string userId);
        Task<List<UserInvite>> GetUsedInvitesByUserId(string userId);
        Task<int> CountTrackedInvites();
        Task DeleteInviteHistoryByGuild(string guildId);

        Task<UserMapping> GetUserMappingById(string id);
        Task<List<UserMapping>> GetUserMappingsByUserId(string userId);
        Task<List<UserMapping>> GetUserMappingsByUserIdAndGuildId(string userId, string guildId);
        Task<UserMapping> GetUserMappingByUserIdsAndGuildId(string userAId, string userBId, string guildId);
        Task<List<UserMapping>> GetUserMappingsByGuildId(string guildId);
        Task<int> CountUserMappings();
        void DeleteUserMapping(UserMapping userMapping);
        void SaveUserMapping(UserMapping userMapping);
        Task DeleteUserMappingByGuild(string guildId);

        Task<UserNote> GetUserNoteById(string id);
        Task<List<UserNote>> GetUserNotesByUserId(string userId);
        Task<List<UserNote>> GetUserNotesByGuildId(string guildId);
        Task<UserNote> GetUserNoteByUserIdAndGuildId(string userId, string guildId);
        Task<int> CountUserNotes();
        void DeleteUserNote(UserNote userNote);
        void SaveUserNote(UserNote userNote);
        Task DeleteUserNoteByGuild(string guildId);
    }
}

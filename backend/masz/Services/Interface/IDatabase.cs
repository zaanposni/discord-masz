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


        Task<int> GetHighestCaseIdForGuild(string guildId);
        Task<ModCase> SelectSpecificModCase(string guildId, string modCaseId);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId);
        Task<List<ModCase>> SelectAllModCasesForGuild(string guildId);
        Task<int> CountAllModCasesForGuild(string guildId);
        Task<int> CountAllActivePunishmentsForGuild(string guildId);
        Task<int> CountAllActivePunishmentsForGuild(string guildId, PunishmentType type);
        Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize);
        Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, int startPage, int pageSize);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishmentForGuild(string guildId);
        Task<List<ModCase>> SelectAllModCasesThatHaveParallelPunishment(ModCase modCase);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishments();
        Task<List<ModCase>> SelectAllModCases();
        Task<List<DbCount>> GetCaseCountGraph(string guildId, DateTime since);
        Task<List<DbCount>> GetPunishmentCountGraph(string guildId, DateTime since);
        Task DeleteAllModCasesForGuild(string guildid);
        void DeleteSpecificModCase(ModCase modCase);
        void UpdateModCase(ModCase modCase);
        Task SaveModCase(ModCase modCase);


        Task<int> CountAllModerationEvents();
        Task<List<DbCount>> GetModerationCountGraph(string guildId, DateTime since);
        Task<int> CountAllModerationEventsForGuild(string guildId);
        Task<int> CountAllModerationEventsForSpecificUserOnGuild(string guildId, string userId);
        Task<List<AutoModerationEvent>> SelectAllModerationEvents();
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

        Task SaveCaseTemplate(CaseTemplate template);
        void DeleteSpecificCaseTemplate(CaseTemplate template);
        Task<CaseTemplate> GetSpecificCaseTemplate(string templateId);
        Task<List<CaseTemplate>> GetAllCaseTemplates();
        Task<List<CaseTemplate>> GetAllTemplatesFromUser(string userId);

        Task<GuildMotd> GetMotdForGuild(string guildId);
        void SaveMotd(GuildMotd motd);
    }
}

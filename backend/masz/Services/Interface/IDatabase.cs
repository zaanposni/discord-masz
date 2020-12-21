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
        Task<List<ModCase>> SelectAllModCasesWithActivePunishmentForGuild(string guildId);
        Task<List<ModCase>> SelectAllModCasesThatHaveParallelPunishment(ModCase modCase);
        Task<List<ModCase>> SelectAllModCasesWithActivePunishments();
        Task<List<ModCase>> SelectAllModCases();
        Task DeleteAllModCasesForGuild(string guildid);
        void DeleteSpecificModCase(ModCase modCase);
        void UpdateModCase(ModCase modCase);
        Task SaveModCase(ModCase modCase);


        Task<int> CountAllModerationEvents();
        Task<int> CountAllModerationEventsForGuild(string guildId);
        Task<List<AutoModerationEvent>> SelectAllModerationEvents();
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(string guildId);
        Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUserOnGuild(string guildId, string userId);
        Task DeleteAllModerationEventsForGuild(string guildid);


        Task SaveModCaseComment(ModCaseComment comment);
        void UpdateModCaseComment(ModCaseComment comment);
        void DeleteSpecificModCaseComment(ModCaseComment comment);
        Task<ModCaseComment> SelectSpecificModCaseComment(int commentId);
    }
}

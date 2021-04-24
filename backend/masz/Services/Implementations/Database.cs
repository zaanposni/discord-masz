using masz.data;
using masz.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masz.Services
{
    public class Database : IDatabase
    {
        private readonly ILogger<Database> logger;
        private readonly DataContext context;

        public Database() { }

        public Database(ILogger<Database> logger, DataContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        // ==================================================================================
        // 
        // Guildconfig
        //
        // ==================================================================================
        public async Task<GuildConfig> SelectSpecificGuildConfig(string guildId)
        {
            return await context.GuildConfigs.AsQueryable().FirstOrDefaultAsync(x => x.GuildId == guildId);
        }

        public async Task<List<GuildConfig>> SelectAllGuildConfigs()
        {
            return await context.GuildConfigs.AsQueryable().ToListAsync();
        }

        public void DeleteSpecificGuildConfig(GuildConfig guildConfig)
        {
            context.GuildConfigs.Remove(guildConfig);
        }

        public void UpdateGuildConfig(GuildConfig guildConfig)
        {
            context.GuildConfigs.Update(guildConfig);
        }

        public async Task SaveGuildConfig(GuildConfig guildConfig)
        {
            await context.GuildConfigs.AddAsync(guildConfig);
        }

        // ==================================================================================
        // 
        // ModCase
        //
        // ==================================================================================

        public async Task<List<ModCase>> SelectAllModcasesMarkedAsDeleted()
        {
            return await context.ModCases.AsQueryable().Where(x => x.MarkedToDeleteAt < DateTime.UtcNow).ToListAsync();
        }
        public async Task<ModCase> SelectSpecificModCase(string guildId, string modCaseId)
        {
            return await context.ModCases.Include(c => c.Comments).AsQueryable().FirstOrDefaultAsync(x => x.GuildId == guildId && x.CaseId.ToString() == modCaseId);
        }

        public async Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).OrderByDescending(x => x.CaseId).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, ModcaseTableType tableType)
        {
            switch(tableType) {
                case ModcaseTableType.OnlyPunishments:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId && x.PunishmentActive).OrderByDescending(x => x.CaseId).ToListAsync();
                case ModcaseTableType.OnlyBin:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId && x.MarkedToDeleteAt != null).OrderByDescending(x => x.CaseId).ToListAsync();
                default:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).OrderByDescending(x => x.CaseId).ToListAsync();
            }
        }

        public async Task<List<ModCase>> SelectAllModCasesForGuild(string guildId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId).OrderByDescending(x => x.CaseId).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, ModcaseTableType tableType)
        {
            switch(tableType) {
                case ModcaseTableType.OnlyPunishments:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.PunishmentActive).OrderByDescending(x => x.CaseId).ToListAsync();
                case ModcaseTableType.OnlyBin:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.MarkedToDeleteAt != null).OrderByDescending(x => x.CaseId).ToListAsync();
                default:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId).OrderByDescending(x => x.CaseId).ToListAsync();
            }
        }

        public async Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize, ModcaseTableType tableType)
        {
            switch(tableType) {
                case ModcaseTableType.OnlyPunishments:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId && x.PunishmentActive).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
                case ModcaseTableType.OnlyBin:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId && x.MarkedToDeleteAt != null).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
                default:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
            }
        }

        public async Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, int startPage, int pageSize)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesForGuild(string guildId, int startPage, int pageSize, ModcaseTableType tableType)
        {
            switch(tableType) {
                case ModcaseTableType.OnlyPunishments:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.PunishmentActive).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
                case ModcaseTableType.OnlyBin:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.MarkedToDeleteAt != null).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
                default:
                    return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId).OrderByDescending(x => x.CaseId).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
            }
        }

        public async Task<List<ModCase>> SelectAllModCasesWithActivePunishmentForGuild(string guildId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.PunishmentActive == true).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesThatHaveParallelPunishment(ModCase modCase)
        {
            return await context.ModCases.AsQueryable().Where(
                x =>
                  x.GuildId == modCase.GuildId &&
                  x.UserId == modCase.UserId &&
                  x.CaseId != modCase.CaseId && 
                  x.MarkedToDeleteAt == null &&
                  x.PunishmentType == modCase.PunishmentType &&
                  x.PunishmentActive == true &&
                  (
                      x.PunishedUntil == null ||
                      x.PunishedUntil > DateTime.UtcNow  
                  )).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesWithActivePunishments()
        {
            return await context.ModCases.AsQueryable().Where(x => x.PunishmentActive == true && x.MarkedToDeleteAt == null).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCases()
        {
            return await context.ModCases.AsQueryable().ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesForSpecificUser(string userId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<ModCase>> SelectLatestModCases(DateTime timeLimit, int limit = 1000)
        {
            return await context.ModCases.AsQueryable().Where(x => x.CreatedAt >= timeLimit).OrderByDescending(x => x.CreatedAt).Take(limit).ToListAsync();
        }

        public async Task<int> CountAllModCasesForGuild(string guildId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId).CountAsync();
        }

        public async Task<int> CountAllActivePunishmentsForGuild(string guildId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.PunishmentActive == true).CountAsync();
        }

        public async Task<int> CountAllActivePunishmentsForGuild(string guildId, PunishmentType type)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.PunishmentActive == true && x.PunishmentType == type).CountAsync();
        }
        
        public async Task<List<DbCount>> GetCaseCountGraph(string guildId, DateTime since)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.CreatedAt > since)
            .GroupBy(x => new { Month = x.CreatedAt.Month, Year = x.CreatedAt.Year }).Select(x => new DbCount { Year = x.Key.Year, Month = x.Key.Month, Count = x.Count() }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToListAsync();
        }

        public async Task<List<DbCount>> GetPunishmentCountGraph(string guildId, DateTime since)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.CreatedAt > since && x.PunishmentType != PunishmentType.None)
            .GroupBy(x => new { Month = x.CreatedAt.Month, Year = x.CreatedAt.Year }).Select(x => new DbCount { Year = x.Key.Year, Month = x.Key.Month, Count = x.Count() }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToListAsync();
        }

        public async Task DeleteAllModCasesForGuild(string guildid)
        {
            var cases = await context.ModCases.AsQueryable().Where(x => x.GuildId == guildid).ToListAsync();
            context.ModCases.RemoveRange(cases);
        }

        public void DeleteSpecificModCase(ModCase modCase)
        {
            context.ModCases.Remove(modCase);
        }

        public void UpdateModCase(ModCase modCase)
        {
            context.ModCases.Update(modCase);
        }

        public async Task SaveModCase(ModCase modCase)
        {
            await context.ModCases.AddAsync(modCase);
        }

        public async Task<int> GetHighestCaseIdForGuild(string guildId)
        {
            var query = context.ModCases.AsQueryable().Where(x => x.GuildId == guildId);
            if(await query.CountAsync() == 0) {
                return 0;
            }
            return await query.MaxAsync(p => p.CaseId);
        }

        // ==================================================================================
        // 
        // AutoModerationEvents
        //
        // ==================================================================================

        public async Task<int> CountAllModerationEvents()
        {
            return await context.AutoModerationEvents.AsQueryable().CountAsync();
        }
        
        public async Task<List<DbCount>> GetModerationCountGraph(string guildId, DateTime since)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId && x.CreatedAt > since)
                .GroupBy(x => new { Month = x.CreatedAt.Month, Year = x.CreatedAt.Year }).Select(x => new DbCount { Year = x.Key.Year, Month = x.Key.Month, Count = x.Count() }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToListAsync();
        }
        public async Task<List<AutoModerationTypeSplit>> GetModerationSplitGraph(string guildId, DateTime since)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId && x.CreatedAt > since)
                .GroupBy(x => new { Type = x.AutoModerationType }).Select(x => new AutoModerationTypeSplit { Type = x.Key.Type, Count = x.Count() }).ToListAsync();
        }

        public async Task<int> CountAllModerationEventsForGuild(string guildId)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId).CountAsync();
        }

        public async Task<int> CountAllModerationEventsForSpecificUserOnGuild(string guildId, string userId)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).CountAsync();
        }

        public async Task<List<AutoModerationEvent>> SelectAllModerationEvents()
        {
            return await context.AutoModerationEvents.AsQueryable().ToListAsync();
        }
        public async Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUser(string userId)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.UserId == userId).ToListAsync();
        }
        public async Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(string guildId)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId).OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
        public async Task<List<AutoModerationEvent>> SelectAllModerationEventsForGuild(string guildId, int startPage, int pageSize)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId).OrderByDescending(x => x.CreatedAt).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<List<AutoModerationEvent>> SelectAllModerationEventsForSpecificUserOnGuild(string guildId, string userId, int startPage, int pageSize)
        {
            return await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).OrderByDescending(x => x.CreatedAt).Skip(startPage*pageSize).Take(pageSize).ToListAsync();
        }

        public async Task DeleteAllModerationEventsForGuild(string guildid)
        {
            var events = await context.AutoModerationEvents.AsQueryable().Where(x => x.GuildId == guildid).ToListAsync();
            context.AutoModerationEvents.RemoveRange(events);
        }
        
        public async Task SaveModerationEvent(AutoModerationEvent modEvent)
        {
            await context.AutoModerationEvents.AddAsync(modEvent);
        }

        // ==================================================================================
        // 
        // AutoModerationConfig
        //
        // ==================================================================================

        public async Task<List<AutoModerationConfig>> SelectAllModerationConfigsForGuild(string guildId)
        {
            return await context.AutoModerationConfigs.AsQueryable().Where(x => x.GuildId == guildId).ToListAsync();
        }
        public async Task<AutoModerationConfig> SelectModerationConfigForGuildAndType(string guildId, AutoModerationType type)
        {
            return await context.AutoModerationConfigs.AsQueryable().FirstOrDefaultAsync(x => x.GuildId == guildId && x.AutoModerationType == type);
        }
        public void PutModerationConfig(AutoModerationConfig modConfig)
        {
            context.AutoModerationConfigs.Update(modConfig);
        }
        public void DeleteSpecificModerationConfig(AutoModerationConfig modConfig)
        {
            context.AutoModerationConfigs.Remove(modConfig);
        }
        public async Task DeleteAllModerationConfigsForGuild(string guildId)
        {
            var events = await context.AutoModerationConfigs.AsQueryable().Where(x => x.GuildId == guildId).ToListAsync();
            context.AutoModerationConfigs.RemoveRange(events);
        }

        // ==================================================================================
        // 
        // Comments
        //
        // ==================================================================================

        public async Task SaveModCaseComment(ModCaseComment comment)
        {
            await context.ModCaseComments.AddAsync(comment);
        }

        public void UpdateModCaseComment(ModCaseComment comment)
        {
            context.ModCaseComments.Update(comment);
        }

        public void DeleteSpecificModCaseComment(ModCaseComment comment)
        {
            context.ModCaseComments.Remove(comment);
        }

        public async Task<ModCaseComment> SelectSpecificModCaseComment(int commentId)
        {
            return await context.ModCaseComments.AsQueryable().FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<List<ModCaseComment>> SelectLastModCaseCommentsByGuild(string guildId)
        {
            return await context.ModCaseComments.Include(x => x.ModCase).AsQueryable().Where(x => x.ModCase.GuildId == guildId).OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync();
        }

        // ==================================================================================
        // 
        // CaseTemplates
        //
        // ==================================================================================

        public async Task SaveCaseTemplate(CaseTemplate template)
        {
            await context.CaseTemplates.AddAsync(template);
        }

        public void DeleteSpecificCaseTemplate(CaseTemplate template)
        {
            context.CaseTemplates.Remove(template);
        }

        public async Task<CaseTemplate> GetSpecificCaseTemplate(string templateId)
        {
            return await context.CaseTemplates.AsQueryable().FirstOrDefaultAsync(x => x.Id.ToString() == templateId);
        }

        public async Task<List<CaseTemplate>> GetAllCaseTemplates()
        {
            return await context.CaseTemplates.AsQueryable().OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<List<CaseTemplate>> GetAllTemplatesFromUser(string userId)
        {
            return await context.CaseTemplates.AsQueryable().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task DeleteAllTemplatesForGuild(string guildId)
        {
            var templates = await context.CaseTemplates.AsQueryable().Where(x => x.CreatedForGuildId == guildId).ToListAsync();
            context.CaseTemplates.RemoveRange(templates);
        }

        // ==================================================================================
        // 
        // Motd
        //
        // ==================================================================================

        public async Task<GuildMotd> GetMotdForGuild(string guildId)
        {
            return await context.GuildMotds.AsQueryable().Where(x => x.GuildId == guildId).FirstOrDefaultAsync();
        }
        public void SaveMotd(GuildMotd motd)
        {
            context.GuildMotds.Update(motd);
        }
        public async Task DeleteMotdForGuild(string guildId)
        {
            var motd = await context.GuildMotds.AsQueryable().Where(x => x.GuildId == guildId).ToListAsync();
            context.GuildMotds.RemoveRange(motd);
        }

        // ==================================================================================
        // 
        // Tokens
        //
        // ==================================================================================

        public async Task SaveToken(APIToken token)
        {
            await context.APITokens.AddAsync(token);
        }

        public void DeleteToken(APIToken token)
        {
            context.APITokens.Remove(token);
        }

        public async Task<List<APIToken>> GetAllAPIToken()
        {
            return await context.APITokens.AsQueryable().ToListAsync();
        }

        public async Task<APIToken> GetAPIToken()
        {
            return await context.APITokens.AsQueryable().FirstOrDefaultAsync();
        }

        public async Task<APIToken> GetAPIToken(int id)
        {
            return await context.APITokens.AsQueryable().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        // ==================================================================================
        // 
        // UserInvites
        //
        // ==================================================================================

        public async Task<List<UserInvite>> GetInvitedUsersByUserId(string userId)
        {
            return await context.UserInvites.AsQueryable().Where(x => x.InviteIssuerId == userId).ToListAsync();
        }

        public async Task<List<UserInvite>> GetUsedInvitesByUserId(string userId)
        {
            return await context.UserInvites.AsQueryable().Where(x => x.JoinedUserId == userId).ToListAsync();
        }

        public async Task<int> CountTrackedInvites()
        {
            return await context.UserInvites.AsQueryable().CountAsync();
        }

        public async Task DeleteInviteHistoryByGuild(string guildId)
        {
            var userinvites = await context.UserInvites.AsQueryable().Where(x => x.GuildId == guildId).ToListAsync();
            context.UserInvites.RemoveRange(userinvites);
        }
    }
}

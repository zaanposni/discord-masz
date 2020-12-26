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

        public async Task<ModCase> SelectSpecificModCase(string guildId, string modCaseId)
        {
            return await context.ModCases.Include(c => c.Comments).AsQueryable().FirstOrDefaultAsync(x => x.GuildId == guildId && x.CaseId.ToString() == modCaseId);
        }

        public async Task<List<ModCase>> SelectAllModcasesForSpecificUserOnGuild(string guildId, string userId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId && x.UserId == userId).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesForGuild(string guildId)
        {
            return await context.ModCases.AsQueryable().Where(x => x.GuildId == guildId).ToListAsync();
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
                  x.PunishmentType == modCase.PunishmentType &&
                  x.PunishmentActive == true &&
                  (
                      x.PunishedUntil == null ||
                      x.PunishedUntil > DateTime.UtcNow  
                  )).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCasesWithActivePunishments()
        {
            return await context.ModCases.AsQueryable().Where(x => x.PunishmentActive == true).ToListAsync();
        }

        public async Task<List<ModCase>> SelectAllModCases()
        {
            return await context.ModCases.AsQueryable().ToListAsync();
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
    }
}

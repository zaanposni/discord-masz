using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class GuildConfigRepository : BaseRepository<GuildConfigRepository>
    {
        private GuildConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static GuildConfigRepository CreateDefault(IServiceProvider serviceProvider) => new GuildConfigRepository(serviceProvider);
        public async Task<GuildConfig> GetGuildConfig(ulong guildId)
        {
            GuildConfig guildConfig = await _database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                _logger.LogWarning($"GuildConfig with id {guildId} not found.");
                throw new ResourceNotFoundException($"GuildConfig with id {guildId} not found.");
            }
            return guildConfig;
        }

        public async Task<List<GuildConfig>> GetAllGuildConfig()
        {
            return await _database.SelectAllGuildConfigs();
        }

        public async Task<GuildConfig> CreateGuildConfig(GuildConfig guildConfig)
        {
            await _database.SaveGuildConfig(guildConfig);
            await _database.SaveChangesAsync();
            return guildConfig;
        }

        public async Task<GuildConfig> UpdateGuildConfig(GuildConfig guildConfig)
        {
            _database.UpdateGuildConfig(guildConfig);
            await _database.SaveChangesAsync();
            return guildConfig;
        }

        public async Task<GuildConfig> DeleteGuildConfig(ulong guildId, bool deleteData = false)
        {
            GuildConfig guildConfig = await GetGuildConfig(guildId);
            if (deleteData) {
                await _database.DeleteAllModCasesForGuild(guildId);
                try {
                    _filesHandler.DeleteDirectory(Path.Combine(_config.Value.AbsolutePathToFileUpload, guildId.ToString()));
                } catch (Exception e) {
                    _logger.LogError(e, "Failed to delete files directory for guilds.");
                }
                await _database.DeleteAllModerationConfigsForGuild(guildId);
                await _database.DeleteAllModerationEventsForGuild(guildId);
                await _database.DeleteAllTemplatesForGuild(guildId);
                await _database.DeleteMotdForGuild(guildId);
                await _database.DeleteInviteHistoryByGuild(guildId);
                await _database.DeleteUserNoteByGuild(guildId);
                await _database.DeleteUserMappingByGuild(guildId);
            }

            _database.DeleteSpecificGuildConfig(guildConfig);
            await _database.SaveChangesAsync();
            return guildConfig;
        }
    }
}
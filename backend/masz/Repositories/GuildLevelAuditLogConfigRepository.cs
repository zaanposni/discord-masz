using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Enums;
using masz.Events;
using masz.Exceptions;
using masz.Models;

namespace masz.Repositories
{

    public class GuildLevelAuditLogConfigRepository : BaseRepository<GuildLevelAuditLogConfigRepository>
    {
        private GuildLevelAuditLogConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static GuildLevelAuditLogConfigRepository CreateDefault(IServiceProvider serviceProvider) => new GuildLevelAuditLogConfigRepository(serviceProvider);

        public async Task<List<GuildLevelAuditLogConfig>> GetConfigsByGuild(ulong guildId)
        {
            return await _database.SelectAllAuditLogConfigsForGuild(guildId);
        }

        public async Task<GuildLevelAuditLogConfig> GetConfigsByGuildAndType(ulong guildId, GuildAuditLogEvent type)
        {
            GuildLevelAuditLogConfig config = await _database.SelectAuditLogConfigForGuildAndType(guildId, type);
            if (config == null)
            {
                throw new ResourceNotFoundException($"GuildAuditLog config {guildId}/{type} does not exist.");
            }
            return config;
        }

        public async Task<GuildLevelAuditLogConfig> UpdateConfig(GuildLevelAuditLogConfig newValue)
        {
            if (! Enum.IsDefined(typeof(GuildAuditLogEvent), newValue.GuildAuditLogEvent)) {
                throw new BaseAPIException("Invalid auditlog event type.", APIError.InvalidAuditLogEvent);
            }

            GuildLevelAuditLogConfig auditLogConfig;
            try
            {
                auditLogConfig = await GetConfigsByGuildAndType(newValue.GuildId, newValue.GuildAuditLogEvent);
            } catch (ResourceNotFoundException)
            {
                auditLogConfig = new GuildLevelAuditLogConfig();
            }
            auditLogConfig.GuildId = newValue.GuildId;
            auditLogConfig.GuildAuditLogEvent = newValue.GuildAuditLogEvent;
            auditLogConfig.ChannelId = newValue.ChannelId;
            auditLogConfig.PingRoles = newValue.PingRoles;

            _database.PutAuditLogConfig(auditLogConfig);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeGuildLevelAuditLogConfigUpdated(new GuildLevelAuditLogConfigUpdatedEventArgs(auditLogConfig));

            return auditLogConfig;
        }

        public async Task DeleteConfigsForGuild(ulong guildId)
        {
            await _database.DeleteAllAuditLogConfigsForGuild(guildId);
            await _database.SaveChangesAsync();
        }

        public async Task<GuildLevelAuditLogConfig> DeleteConfigForGuild(ulong guildId, GuildAuditLogEvent type)
        {
            GuildLevelAuditLogConfig config = await GetConfigsByGuildAndType(guildId, type);

            _database.DeleteSpecificAuditLogConfig(config);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeGuildLevelAuditLogConfigDeleted(new GuildLevelAuditLogConfigDeletedEventArgs(config));

            return config;
        }
    }
}
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class GuildLevelAuditLogConfigRepository : BaseRepository<GuildLevelAuditLogConfigRepository>
    {
        private GuildLevelAuditLogConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static GuildLevelAuditLogConfigRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<GuildLevelAuditLogConfig>> GetConfigsByGuild(ulong guildId)
        {
            return await Database.SelectAllAuditLogConfigsForGuild(guildId);
        }

        public async Task<GuildLevelAuditLogConfig> GetConfigsByGuildAndType(ulong guildId, GuildAuditLogEvent type)
        {
            GuildLevelAuditLogConfig config = await Database.SelectAuditLogConfigForGuildAndType(guildId, type);
            if (config == null)
            {
                throw new ResourceNotFoundException($"GuildAuditLog config {guildId}/{type} does not exist.");
            }
            return config;
        }

        public async Task<GuildLevelAuditLogConfig> UpdateConfig(GuildLevelAuditLogConfig newValue)
        {
            if (!Enum.IsDefined(typeof(GuildAuditLogEvent), newValue.GuildAuditLogEvent))
            {
                throw new BaseAPIException("Invalid auditlog event type.", APIError.InvalidAuditLogEvent);
            }

            GuildLevelAuditLogConfig auditLogConfig;
            try
            {
                auditLogConfig = await GetConfigsByGuildAndType(newValue.GuildId, newValue.GuildAuditLogEvent);
            }
            catch (ResourceNotFoundException)
            {
                auditLogConfig = new GuildLevelAuditLogConfig();
            }
            auditLogConfig.GuildId = newValue.GuildId;
            auditLogConfig.GuildAuditLogEvent = newValue.GuildAuditLogEvent;
            auditLogConfig.ChannelId = newValue.ChannelId;
            auditLogConfig.PingRoles = newValue.PingRoles;

            Database.PutAuditLogConfig(auditLogConfig);
            await Database.SaveChangesAsync();

            await _eventHandler.OnGuildLevelAuditLogConfigUpdatedEvent.InvokeAsync(auditLogConfig);

            return auditLogConfig;
        }

        public async Task DeleteConfigsForGuild(ulong guildId)
        {
            await Database.DeleteAllAuditLogConfigsForGuild(guildId);
            await Database.SaveChangesAsync();
        }

        public async Task<GuildLevelAuditLogConfig> DeleteConfigForGuild(ulong guildId, GuildAuditLogEvent type)
        {
            GuildLevelAuditLogConfig config = await GetConfigsByGuildAndType(guildId, type);

            Database.DeleteSpecificAuditLogConfig(config);
            await Database.SaveChangesAsync();

            await _eventHandler.OnGuildLevelAuditLogConfigDeletedEvent.InvokeAsync(config);

            return config;
        }
    }
}
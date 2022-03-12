using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class GuildLevelAuditLogConfigRepository : BaseRepository<GuildLevelAuditLogConfigRepository>
    {
        private readonly IUser _currentUser;
        private GuildLevelAuditLogConfigRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private GuildLevelAuditLogConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }

        public static GuildLevelAuditLogConfigRepository CreateDefault(IServiceProvider serviceProvider, IUser currentUser) => new(serviceProvider, currentUser);
        public static GuildLevelAuditLogConfigRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);

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

            RestAction action = RestAction.Updated;
            GuildLevelAuditLogConfig auditLogConfig;
            try
            {
                auditLogConfig = await GetConfigsByGuildAndType(newValue.GuildId, newValue.GuildAuditLogEvent);
            }
            catch (ResourceNotFoundException)
            {
                auditLogConfig = new GuildLevelAuditLogConfig();
                action = RestAction.Created;
            }
            auditLogConfig.GuildId = newValue.GuildId;
            auditLogConfig.GuildAuditLogEvent = newValue.GuildAuditLogEvent;
            auditLogConfig.ChannelId = newValue.ChannelId;
            auditLogConfig.PingRoles = newValue.PingRoles;
            auditLogConfig.IgnoreChannels = newValue.IgnoreChannels;
            auditLogConfig.IgnoreRoles = newValue.IgnoreRoles;

            Database.PutAuditLogConfig(auditLogConfig);
            await Database.SaveChangesAsync();

            if (action == RestAction.Created)
            {
                _eventHandler.OnGuildLevelAuditLogConfigCreatedEvent.InvokeAsync(auditLogConfig, _currentUser);
            }
            else
            {
                _eventHandler.OnGuildLevelAuditLogConfigUpdatedEvent.InvokeAsync(auditLogConfig, _currentUser);
            }

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

            _eventHandler.OnGuildLevelAuditLogConfigDeletedEvent.InvokeAsync(config, _currentUser);

            return config;
        }
    }
}
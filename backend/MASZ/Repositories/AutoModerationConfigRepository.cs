using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Extensions;

namespace MASZ.Repositories
{

    public class AutoModerationConfigRepository : BaseRepository<AutoModerationConfigRepository>
    {
        private AutoModerationConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static AutoModerationConfigRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<AutoModerationConfig>> GetConfigsByGuild(ulong guildId)
        {
            return await Database.SelectAllModerationConfigsForGuild(guildId);
        }

        public async Task<AutoModerationConfig> GetConfigsByGuildAndType(ulong guildId, AutoModerationType type)
        {
            AutoModerationConfig config = await Database.SelectModerationConfigForGuildAndType(guildId, type);
            if (config == null)
            {
                throw new ResourceNotFoundException($"Automod config {guildId}/{type} does not exist.");
            }
            return config;
        }

        public async Task<AutoModerationConfig> UpdateConfig(AutoModerationConfig newValue)
        {
            if (!Enum.IsDefined(typeof(AutoModerationType), newValue.AutoModerationType))
            {
                throw new BaseAPIException("Invalid automod type.", APIError.InvalidAutomoderationType);
            }
            if (!Enum.IsDefined(typeof(AutoModerationAction), newValue.AutoModerationAction))
            {
                throw new BaseAPIException("Invalid automod action.", APIError.InvalidAutomoderationAction);
            }

            AutoModerationConfig autoModerationConfig;
            try
            {
                autoModerationConfig = await GetConfigsByGuildAndType(newValue.GuildId, newValue.AutoModerationType);
            }
            catch (ResourceNotFoundException)
            {
                autoModerationConfig = new AutoModerationConfig();
            }
            autoModerationConfig.GuildId = newValue.GuildId;
            autoModerationConfig.AutoModerationType = newValue.AutoModerationType;
            autoModerationConfig.AutoModerationAction = newValue.AutoModerationAction;
            autoModerationConfig.PunishmentType = newValue.PunishmentType;
            autoModerationConfig.PunishmentDurationMinutes = newValue.PunishmentDurationMinutes;
            autoModerationConfig.IgnoreChannels = newValue.IgnoreChannels;
            autoModerationConfig.IgnoreRoles = newValue.IgnoreRoles;
            autoModerationConfig.TimeLimitMinutes = newValue.TimeLimitMinutes;
            autoModerationConfig.Limit = newValue.Limit;
            autoModerationConfig.CustomWordFilter = newValue.CustomWordFilter;
            autoModerationConfig.SendDmNotification = newValue.SendDmNotification;
            autoModerationConfig.SendPublicNotification = newValue.SendPublicNotification;
            autoModerationConfig.ChannelNotificationBehavior = newValue.ChannelNotificationBehavior;

            Database.PutModerationConfig(autoModerationConfig);
            await Database.SaveChangesAsync();

            await _eventHandler.OnAutoModerationConfigUpdatedEvent.InvokeAsync(autoModerationConfig);

            return autoModerationConfig;
        }

        public async Task DeleteConfigsForGuild(ulong guildId)
        {
            await Database.DeleteAllModerationConfigsForGuild(guildId);
            await Database.SaveChangesAsync();
        }

        public async Task<AutoModerationConfig> DeleteConfigForGuild(ulong guildId, AutoModerationType type)
        {
            AutoModerationConfig config = await GetConfigsByGuildAndType(guildId, type);

            Database.DeleteSpecificModerationConfig(config);
            await Database.SaveChangesAsync();

            await _eventHandler.OnAutoModerationConfigDeletedEvent.InvokeAsync(config);

            return config;
        }
    }
}
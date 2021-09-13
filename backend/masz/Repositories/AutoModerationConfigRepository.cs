using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using masz.Dtos.Tokens;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace masz.Repositories
{

    public class AutoModerationConfigRepository : BaseRepository<AutoModerationConfigRepository>
    {
        private AutoModerationConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static AutoModerationConfigRepository CreateDefault(IServiceProvider serviceProvider) => new AutoModerationConfigRepository(serviceProvider);

        public async Task<List<AutoModerationConfig>> GetConfigsByGuild(ulong guildId)
        {
            return await _database.SelectAllModerationConfigsForGuild(guildId);
        }

        public async Task<AutoModerationConfig> GetConfigsByGuildAndType(ulong guildId, AutoModerationType type)
        {
            AutoModerationConfig config = await _database.SelectModerationConfigForGuildAndType(guildId, type);
            if (config == null)
            {
                throw new ResourceNotFoundException($"Automod config {guildId}/{type} does not exist.");
            }
            return config;
        }

        public async Task<AutoModerationConfig> UpdateConfig(AutoModerationConfig newValue)
        {
            AutoModerationConfig autoModerationConfig;
            try
            {
                autoModerationConfig = await GetConfigsByGuildAndType(newValue.GuildId, newValue.AutoModerationType);
            } catch (ResourceNotFoundException)
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

            _database.PutModerationConfig(autoModerationConfig);
            await _database.SaveChangesAsync();
            return autoModerationConfig;
        }

        public async Task DeleteConfigsForGuild(ulong guildId)
        {
            await _database.DeleteAllModerationConfigsForGuild(guildId);
            await _database.SaveChangesAsync();
        }
    }
}
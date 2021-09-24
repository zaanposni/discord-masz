using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Events;
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

        public async Task<List<GuildConfig>> GetAllGuildConfigs()
        {
            return await _database.SelectAllGuildConfigs();
        }

        public async Task<int> CountGuildConfigs()
        {
            return await _database.CountAllGuildConfigs();
        }

        public async Task<GuildConfig> CreateGuildConfig(GuildConfig guildConfig)
        {
            DiscordGuild guild = await _discordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.IgnoreCache);
            if (guild == null)
            {
                throw new ResourceNotFoundException($"Guild with id {guildConfig.GuildId} not found.");
            }
            foreach (ulong role in guildConfig.ModRoles)
            {
                if (! guild.Roles.ContainsKey(role))
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.AdminRoles)
            {
                if (! guild.Roles.ContainsKey(role))
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.MutedRoles)
            {
                if (! guild.Roles.ContainsKey(role))
                {
                    throw new RoleNotFoundException(role);
                }
            }

            if (guildConfig.ModInternalNotificationWebhook != null) {
                guildConfig.ModInternalNotificationWebhook = guildConfig.ModInternalNotificationWebhook.Replace("discord.com", "discordapp.com");
            }
            if (guildConfig.ModPublicNotificationWebhook != null) {
                guildConfig.ModPublicNotificationWebhook = guildConfig.ModPublicNotificationWebhook.Replace("discord.com", "discordapp.com");
            }

            await _database.SaveGuildConfig(guildConfig);
            await _database.SaveChangesAsync();

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                await _discordAnnouncer.AnnounceTipsInNewGuild(guildConfig);
            }

            await _eventHandler.InvokeGuildRegistered(new GuildRegisteredEventArgs(guildConfig));

            return guildConfig;
        }

        public async Task<GuildConfig> UpdateGuildConfig(GuildConfig guildConfig)
        {
            DiscordGuild guild = await _discordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.IgnoreCache);
            if (guild == null)
            {
                throw new ResourceNotFoundException($"Guild with id {guildConfig.GuildId} not found.");
            }
            foreach (ulong role in guildConfig.ModRoles)
            {
                if (! guild.Roles.ContainsKey(role))
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.AdminRoles)
            {
                if (! guild.Roles.ContainsKey(role))
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.MutedRoles)
            {
                if (! guild.Roles.ContainsKey(role))
                {
                    throw new RoleNotFoundException(role);
                }
            }

            if (guildConfig.ModInternalNotificationWebhook != null) {
                guildConfig.ModInternalNotificationWebhook = guildConfig.ModInternalNotificationWebhook.Replace("discord.com", "discordapp.com");
            }
            if (guildConfig.ModPublicNotificationWebhook != null) {
                guildConfig.ModPublicNotificationWebhook = guildConfig.ModPublicNotificationWebhook.Replace("discord.com", "discordapp.com");
            }

            _database.UpdateGuildConfig(guildConfig);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeGuildUpdated(new GuildUpdatedEventArgs(guildConfig));

            return guildConfig;
        }

        public async Task<GuildConfig> DeleteGuildConfig(ulong guildId, bool deleteData = false)
        {
            GuildConfig guildConfig = await GetGuildConfig(guildId);
            if (deleteData) {
                await _database.DeleteAllModCasesForGuild(guildId);
                try {
                    _filesHandler.DeleteDirectory(Path.Combine(_config.GetFileUploadPath(), guildId.ToString()));
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

            await _eventHandler.InvokeGuildDeleted(new GuildDeletedEventArgs(guildConfig));

            return guildConfig;
        }
    }
}
using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Extensions;

namespace MASZ.Repositories
{

    public class GuildConfigRepository : BaseRepository<GuildConfigRepository>
    {
        private GuildConfigRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static GuildConfigRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);
        public async Task<GuildConfig> GetGuildConfig(ulong guildId)
        {
            GuildConfig guildConfig = await Database.SelectSpecificGuildConfig(guildId);
            if (guildConfig == null)
            {
                throw new ResourceNotFoundException($"GuildConfig with id {guildId} not found.");
            }
            return guildConfig;
        }

        public async Task<List<GuildConfig>> GetAllGuildConfigs()
        {
            return await Database.SelectAllGuildConfigs();
        }

        public async Task<int> CountGuildConfigs()
        {
            return await Database.CountAllGuildConfigs();
        }

        public async Task<GuildConfig> CreateGuildConfig(GuildConfig guildConfig)
        {
            IGuild guild = DiscordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.IgnoreCache);
            if (guild == null)
            {
                throw new ResourceNotFoundException($"Guild with id {guildConfig.GuildId} not found.");
            }
            foreach (ulong role in guildConfig.ModRoles)
            {
                if (!guild.Roles.Where(r => r.Id == role).Any())
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.AdminRoles)
            {
                if (!guild.Roles.Where(r => r.Id == role).Any())
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.MutedRoles)
            {
                if (!guild.Roles.Where(r => r.Id == role).Any())
                {
                    throw new RoleNotFoundException(role);
                }
            }

            if (guildConfig.ModInternalNotificationWebhook != null)
            {
                guildConfig.ModInternalNotificationWebhook = guildConfig.ModInternalNotificationWebhook.Replace("discord.com", "discordapp.com");
            }
            if (guildConfig.ModPublicNotificationWebhook != null)
            {
                guildConfig.ModPublicNotificationWebhook = guildConfig.ModPublicNotificationWebhook.Replace("discord.com", "discordapp.com");
            }

            await Database.SaveGuildConfig(guildConfig);
            await Database.SaveChangesAsync();

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                await _discordAnnouncer.AnnounceTipsInNewGuild(guildConfig);
            }

            await _eventHandler.OnGuildRegisteredEvent.InvokeAsync(guildConfig);

            return guildConfig;
        }

        public async Task<GuildConfig> UpdateGuildConfig(GuildConfig guildConfig)
        {
            IGuild guild = DiscordAPI.FetchGuildInfo(guildConfig.GuildId, CacheBehavior.IgnoreCache);
            if (guild == null)
            {
                throw new ResourceNotFoundException($"Guild with id {guildConfig.GuildId} not found.");
            }
            foreach (ulong role in guildConfig.ModRoles)
            {
                if (!guild.Roles.Where(r => r.Id == role).Any())
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.AdminRoles)
            {
                if (!guild.Roles.Where(r => r.Id == role).Any())
                {
                    throw new RoleNotFoundException(role);
                }
            }
            foreach (ulong role in guildConfig.MutedRoles)
            {
                if (!guild.Roles.Where(r => r.Id == role).Any())
                {
                    throw new RoleNotFoundException(role);
                }
            }

            if (guildConfig.ModInternalNotificationWebhook != null)
            {
                guildConfig.ModInternalNotificationWebhook = guildConfig.ModInternalNotificationWebhook.Replace("discord.com", "discordapp.com");
            }
            if (guildConfig.ModPublicNotificationWebhook != null)
            {
                guildConfig.ModPublicNotificationWebhook = guildConfig.ModPublicNotificationWebhook.Replace("discord.com", "discordapp.com");
            }

            Database.UpdateGuildConfig(guildConfig);
            await Database.SaveChangesAsync();

            await _eventHandler.OnGuildUpdatedEvent.InvokeAsync(guildConfig);

            return guildConfig;
        }

        public async Task<GuildConfig> DeleteGuildConfig(ulong guildId, bool deleteData = false)
        {
            GuildConfig guildConfig = await GetGuildConfig(guildId);
            if (deleteData)
            {
                await Database.DeleteAllModCasesForGuild(guildId);
                try
                {
                    _filesHandler.DeleteDirectory(Path.Combine(_config.GetFileUploadPath(), guildId.ToString()));
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to delete files directory for guilds.");
                }
                await Database.DeleteAllModerationConfigsForGuild(guildId);
                await Database.DeleteAllModerationEventsForGuild(guildId);
                await Database.DeleteAllTemplatesForGuild(guildId);
                await Database.DeleteMotdForGuild(guildId);
                await Database.DeleteInviteHistoryByGuild(guildId);
                await Database.DeleteUserNoteByGuild(guildId);
                await Database.DeleteUserMappingByGuild(guildId);
            }

            Database.DeleteSpecificGuildConfig(guildConfig);
            await Database.SaveChangesAsync();

            await _eventHandler.OnGuildDeletedEvent.InvokeAsync(guildConfig);

            return guildConfig;
        }
    }
}
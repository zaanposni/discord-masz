using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class Scheduler : IScheduler
    {
        private readonly ILogger<Scheduler> logger;
        private readonly IOptions<InternalConfig> config;
        private readonly IDiscordAPIInterface discord;
        private readonly IFilesHandler filesHandler;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IIdentityManager identityManager;
        private DateTime nextCacheSchedule;

        public Scheduler() { }

        public Scheduler(ILogger<Scheduler> logger, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IServiceScopeFactory serviceScopeFactory, IFilesHandler filesHandler, IIdentityManager identityManager)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.serviceScopeFactory = serviceScopeFactory;
            this.filesHandler = filesHandler;
            this.identityManager = identityManager;
        }

        public void StartTimers()
        {
            logger.LogWarning("Starting schedule timers.");
            Task task = new Task(() =>
                {
                    while (true)
                    {
                        CheckDeletedCases();
                        CacheAll();
                        this.identityManager.ClearOldIdentities();
                        this.nextCacheSchedule = DateTime.UtcNow.AddMinutes(15);
                        Thread.Sleep(1000 * 60 * 15);  // 15 minutes
                    }
                });
                task.Start();
            logger.LogWarning("Started schedule timers.");
        }

        public async void CheckDeletedCases()
        {
            logger.LogInformation("Casebin | Checking case bin and delete old cases.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                foreach (ModCase modCase in await database.SelectAllModcasesMarkedAsDeleted())
                {
                    try {
                        filesHandler.DeleteDirectory(Path.Combine(config.Value.AbsolutePathToFileUpload, modCase.GuildId, modCase.CaseId.ToString()));
                    } catch (Exception e) {
                        logger.LogError(e, "Failed to delete files directory for modcase.");
                    }
                    database.DeleteSpecificModCase(modCase);
                }
                await database.SaveChangesAsync();
            }
            logger.LogInformation("Casebin | Done.");
        }

        public async void CacheAll()
        {
            await CacheAllKnownGuilds();
            List<string> handledUsers = new List<string>();
            handledUsers = await CacheAllGuildBans(handledUsers);
            handledUsers = await CacheAllGuildMembers(handledUsers);
            await CacheAllKnownUsers(handledUsers);
        }

        public async Task CacheAllKnownGuilds()
        {
            logger.LogInformation("Cacher | Cache all registered guilds.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    await discord.FetchGuildInfo(guild.GuildId, CacheBehavior.IgnoreCache);
                    await discord.FetchGuildChannels(guild.GuildId, CacheBehavior.IgnoreCache);
                }
            }
            logger.LogInformation("Cacher | Done - Cache all registered guilds.");
        }
        public async Task<List<string>> CacheAllGuildMembers(List<string> handledUsers)
        {
            logger.LogInformation("Cacher | Cache all members of registered guilds.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    var members = await discord.FetchGuildMembers(guild.GuildId, CacheBehavior.IgnoreCache);
                    if (members != null) {
                        foreach (var item in members)
                        {
                            if (!handledUsers.Contains(item.User.Id)) {
                                handledUsers.Add(item.User.Id);
                            }
                        }
                    }
                }
            }
            logger.LogInformation("Cacher | Done - Cached all members of registered guilds.");
            return handledUsers;
        }

        public async Task<List<string>> CacheAllGuildBans(List<string> handledUsers)
        {
            logger.LogInformation("Cacher | Cache all bans of registered guilds.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    List<Ban> bans = await this.discord.GetGuildBans(guild.GuildId, CacheBehavior.IgnoreCache);
                    if (bans != null) {
                        foreach (Ban ban in bans)
                        {
                            handledUsers.Add(ban.User.Id);
                        }
                    }
                }
            }
            logger.LogInformation("Cacher | Done - Cached all bans of registered guilds.");
            return handledUsers;
        }

        public async Task<List<string>> CacheAllKnownUsers(List<string> handledUsers)
        {
            logger.LogInformation("Cacher | Cache all known users.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var modCase in await database.SelectLatestModCases(DateTime.UtcNow.AddYears(-3), 750))
                {
                    if (!handledUsers.Contains(modCase.UserId)) {
                        await discord.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.UserId);
                    }
                    if (!handledUsers.Contains(modCase.ModId)) {
                        await discord.FetchUserInfo(modCase.ModId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.ModId);
                    }
                    if (!handledUsers.Contains(modCase.LastEditedByModId)) {
                        await discord.FetchUserInfo(modCase.LastEditedByModId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.LastEditedByModId);
                    }
                }

                foreach (var userNote in await database.SelectLatestUserNotes(DateTime.UtcNow.AddYears(-3), 100))
                {
                    if (!handledUsers.Contains(userNote.UserId)) {
                        await discord.FetchUserInfo(userNote.UserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userNote.UserId);
                    }
                    if (!handledUsers.Contains(userNote.CreatorId)) {
                        await discord.FetchUserInfo(userNote.CreatorId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userNote.CreatorId);
                    }
                }

                foreach (var userMapping in await database.SelectLatestUserMappings(DateTime.UtcNow.AddYears(-3), 100))
                {
                    if (!handledUsers.Contains(userMapping.UserA)) {
                        await discord.FetchUserInfo(userMapping.UserA, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.UserA);
                    }
                    if (!handledUsers.Contains(userMapping.UserB)) {
                        await discord.FetchUserInfo(userMapping.UserB, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.UserB);
                    }
                    if (!handledUsers.Contains(userMapping.CreatorUserId)) {
                        await discord.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.CreatorUserId);
                    }
                }
            }
            logger.LogInformation("Cacher | Done - Cache all known users.");
            return handledUsers;
        }

        public DateTime GetNextCacheSchedule()
        {
            return this.nextCacheSchedule;
        }
    }
}
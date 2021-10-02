using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using masz.Enums;
using masz.Events;

namespace masz.Services
{
    public class Scheduler : IScheduler
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly IInternalConfiguration _config;
        private readonly IDiscordAPIInterface _discordAPI;
        private readonly IFilesHandler _filesHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IIdentityManager _identityManager;
        private readonly IEventHandler _eventHandler;
        private DateTime _nextCacheSchedule;

        public Scheduler() { }

        public Scheduler(ILogger<Scheduler> logger, IInternalConfiguration config, IDiscordAPIInterface discord, IServiceScopeFactory serviceScopeFactory, IFilesHandler filesHandler, IIdentityManager identityManager)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discord;
            _serviceScopeFactory = serviceScopeFactory;
            _filesHandler = filesHandler;
            _identityManager = identityManager;
        }

        public void StartTimers()
        {
            _logger.LogWarning("Starting schedule timers.");
            Task task = new Task(() =>
                {
                    while (true)
                    {
                        CheckDeletedCases();
                        CacheAll();
                        _identityManager.ClearOldIdentities();
                        _nextCacheSchedule = DateTime.UtcNow.AddMinutes(15);
                        Thread.Sleep(1000 * 60 * 15);  // 15 minutes
                    }
                });
                task.Start();
            _logger.LogWarning("Started schedule timers.");
        }

        public async void CheckDeletedCases()
        {
            _logger.LogInformation("Casebin | Checking case bin and delete old cases.");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                foreach (ModCase modCase in await database.SelectAllModcasesMarkedAsDeleted())
                {
                    try {
                        _filesHandler.DeleteDirectory(Path.Combine(_config.GetFileUploadPath(), modCase.GuildId.ToString(), modCase.CaseId.ToString()));
                    } catch (Exception e) {
                        _logger.LogError(e, "Failed to delete files directory for modcase.");
                    }
                    database.DeleteSpecificModCase(modCase);
                }
                await database.SaveChangesAsync();
            }
            _logger.LogInformation("Casebin | Done.");
        }

        public async void CacheAll()
        {
            await CacheAllKnownGuilds();
            List<ulong> handledUsers = new List<ulong>();
            handledUsers = await CacheAllGuildBans(handledUsers);
            handledUsers = await CacheAllGuildMembers(handledUsers);
            handledUsers = await CacheAllKnownUsers(handledUsers);
            _logger.LogInformation($"Cacher | Done with {handledUsers.Count} entries.");
            await _eventHandler.InvokeInternalCachingDone(new InternalCachingDoneEventArgs(handledUsers.Count));
        }

        public async Task CacheAllKnownGuilds()
        {
            _logger.LogInformation("Cacher | Cache all registered guilds.");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    await _discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.IgnoreCache);
                    await _discordAPI.FetchGuildChannels(guild.GuildId, CacheBehavior.IgnoreCache);
                }
            }
        }
        public async Task<List<ulong>> CacheAllGuildMembers(List<ulong> handledUsers)
        {
            _logger.LogInformation("Cacher | Cache all members of registered guilds.");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    var members = await _discordAPI.FetchGuildMembers(guild.GuildId, CacheBehavior.IgnoreCache);
                    if (members != null) {
                        foreach (var item in members)
                        {
                            if (!handledUsers.Contains(item.Id)) {
                                handledUsers.Add(item.Id);
                            }
                        }
                    }
                }
            }
            return handledUsers;
        }

        public async Task<List<ulong>> CacheAllGuildBans(List<ulong> handledUsers)
        {
            _logger.LogInformation("Cacher | Cache all bans of registered guilds.");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    List<DiscordBan> bans = await _discordAPI.GetGuildBans(guild.GuildId, CacheBehavior.IgnoreCache);
                    if (bans != null) {
                        foreach (DiscordBan ban in bans)
                        {
                            handledUsers.Add(ban.User.Id);
                        }
                    }
                }
            }
            return handledUsers;
        }

        public async Task<List<ulong>> CacheAllKnownUsers(List<ulong> handledUsers)
        {
            _logger.LogInformation("Cacher | Cache all known users.");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                foreach (var modCase in await database.SelectLatestModCases(DateTime.UtcNow.AddYears(-3), 750))
                {
                    if (!handledUsers.Contains(modCase.UserId)) {
                        await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.UserId);
                    }
                    if (!handledUsers.Contains(modCase.ModId)) {
                        await _discordAPI.FetchUserInfo(modCase.ModId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.ModId);
                    }
                    if (!handledUsers.Contains(modCase.LastEditedByModId)) {
                        await _discordAPI.FetchUserInfo(modCase.LastEditedByModId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.LastEditedByModId);
                    }
                }

                foreach (var userNote in await database.SelectLatestUserNotes(DateTime.UtcNow.AddYears(-3), 100))
                {
                    if (!handledUsers.Contains(userNote.UserId)) {
                        await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userNote.UserId);
                    }
                    if (!handledUsers.Contains(userNote.CreatorId)) {
                        await _discordAPI.FetchUserInfo(userNote.CreatorId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userNote.CreatorId);
                    }
                }

                foreach (var userMapping in await database.SelectLatestUserMappings(DateTime.UtcNow.AddYears(-3), 100))
                {
                    if (!handledUsers.Contains(userMapping.UserA)) {
                        await _discordAPI.FetchUserInfo(userMapping.UserA, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.UserA);
                    }
                    if (!handledUsers.Contains(userMapping.UserB)) {
                        await _discordAPI.FetchUserInfo(userMapping.UserB, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.UserB);
                    }
                    if (!handledUsers.Contains(userMapping.CreatorUserId)) {
                        await _discordAPI.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.CreatorUserId);
                    }
                }
            }
            return handledUsers;
        }

        public DateTime GetNextCacheSchedule()
        {
            return _nextCacheSchedule;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public Scheduler() { }

        public Scheduler(ILogger<Scheduler> logger, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IServiceScopeFactory serviceScopeFactory, IFilesHandler filesHandler)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.serviceScopeFactory = serviceScopeFactory;
            this.filesHandler = filesHandler;
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
            List<string> handledUsers = new List<string>();
            handledUsers.AddRange(await CacheAllGuildMembers());
            await CacheAllKnownUsers(handledUsers);
        }

        public async Task<List<string>> CacheAllGuildMembers()
        {
            logger.LogInformation("Cacher | Cache all members of registered guilds.");
            List<string> handledUsers = new List<string>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    var members = await discord.FetchGuildMembers(guild.GuildId, true);
                    foreach (var item in members)
                    {
                        if (!handledUsers.Contains(item.User.Id)) {
                            handledUsers.Add(item.User.Id);
                        }
                    }
                }
            }
            logger.LogInformation("Cacher | Done.");
            return handledUsers;
        }

        public async Task<List<string>> CacheAllKnownUsers(List<string> handledUsers)
        {
            logger.LogInformation("Cacher | Cache all known users.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var modCase in await database.SelectAllModCases())
                {
                    if (!handledUsers.Contains(modCase.UserId)) {
                        await discord.FetchUserInfo(modCase.UserId, true);
                        handledUsers.Add(modCase.UserId);
                    }
                    if (!handledUsers.Contains(modCase.ModId)) {
                        await discord.FetchUserInfo(modCase.ModId, true);
                        handledUsers.Add(modCase.ModId);
                    }
                    if (!handledUsers.Contains(modCase.LastEditedByModId)) {
                        await discord.FetchUserInfo(modCase.LastEditedByModId, true);
                        handledUsers.Add(modCase.LastEditedByModId);
                    }
                }
            }
            logger.LogInformation("Cacher | Done.");
            return handledUsers;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using masz.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class Cacher : ICacher
    {
        private readonly ILogger<Cacher> logger;
        private readonly IOptions<Cacher> config;
        private readonly IDiscordAPIInterface discord;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public Cacher() { }

        public Cacher(ILogger<Cacher> logger, IOptions<Cacher> config, IDiscordAPIInterface discord, IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void StartTimer()
        {
            logger.LogWarning("Starting action loop.");
            Task task = new Task(() =>
                {
                    while (true)
                    {
                        CacheAll();
                        Thread.Sleep(1000 * 60 * 15);  // 15 minutes
                    }
                });
                task.Start();
            logger.LogWarning("Finished action loop.");
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

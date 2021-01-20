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
        private List<string> handledUsers = new List<string>();

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
            this.handledUsers.Clear();
            await CacheAllGuildMembers();
            await CacheAllKnownUsers();
        }

        public async Task CacheAllGuildMembers()
        {
            logger.LogInformation("Cacher | Cache all members of registered guilds.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    var members = await discord.FetchGuildMembersAsync(guild.GuildId, true);
                    foreach (var item in members)
                    {
                        if (!this.handledUsers.Contains(item.User.Id)) {
                            this.handledUsers.Add(item.User.Id);
                        }
                    }
                }
            }
            logger.LogInformation("Cacher | Done.");
        }

        public async Task CacheAllKnownUsers()
        {
            logger.LogInformation("Cacher | Cache all known users.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                
                foreach (var modCase in await database.SelectAllModCases())
                {
                    if (!this.handledUsers.Contains(modCase.UserId)) {
                        await discord.FetchUserInfoAsync(modCase.UserId, true);
                        this.handledUsers.Add(modCase.UserId);
                    }
                    if (!this.handledUsers.Contains(modCase.ModId)) {
                        await discord.FetchUserInfoAsync(modCase.ModId, true);
                        this.handledUsers.Add(modCase.ModId);
                    }
                    if (!this.handledUsers.Contains(modCase.LastEditedByModId)) {
                        await discord.FetchUserInfoAsync(modCase.LastEditedByModId, true);
                        this.handledUsers.Add(modCase.LastEditedByModId);
                    }
                }
            }
            logger.LogInformation("Cacher | Done.");
        }
    }
}

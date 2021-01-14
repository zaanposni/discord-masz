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
                        CacheAllKnownUsers();
                        Thread.Sleep(1000 * 60 * 15);  // 15 minutes
                    }
                });
                task.Start();
            logger.LogWarning("Finished action loop.");
        }

        public async void CacheAllKnownUsers()
        {
            logger.LogInformation("Cacher | Cache all known users.");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                List<string> handledUsers = new List<string>();
                foreach (var modCase in await database.SelectAllModCases())
                {
                    if (!handledUsers.Contains(modCase.UserId)) {
                        await discord.FetchUserInfoAsync(modCase.UserId, true);
                        handledUsers.Add(modCase.UserId);
                    }
                    if (!handledUsers.Contains(modCase.ModId)) {
                        await discord.FetchUserInfoAsync(modCase.ModId, true);
                        handledUsers.Add(modCase.ModId);
                    }
                    if (!handledUsers.Contains(modCase.LastEditedByModId)) {
                        await discord.FetchUserInfoAsync(modCase.LastEditedByModId, true);
                        handledUsers.Add(modCase.LastEditedByModId);
                    }
                }
            }
            logger.LogInformation("Cacher | Done.");
        }
    }
}

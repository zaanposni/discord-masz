using System;
using System.Threading;
using System.Threading.Tasks;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class PunishmentHandler : IPunishmentHandler
    {
        private readonly ILogger<PunishmentHandler> logger;
        private readonly IDatabase database;
        private readonly IOptions<PunishmentHandler> config;
        private readonly IDiscordAPIInterface discord;

        public PunishmentHandler() { }

        public PunishmentHandler(ILogger<PunishmentHandler> logger, IOptions<PunishmentHandler> config, IDiscordAPIInterface discord, IDatabase database)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.database = database;
        }

        public void StartTimer()
        {
            logger.LogWarning("Starting action loop.");
            Task task = new Task(() =>
                {
                    while (true)
                    {
                        CheckAllCurrentPunishments();
                        Thread.Sleep(1000 * 60);
                    }
                });
                task.Start();
            logger.LogWarning("Finished action loop.");
        }

        public async void CheckAllCurrentPunishments()
        {
            var cases = await database.SelectAllModCasesWithActiveAndDuePunishment();
            logger.LogWarning("test");
            logger.LogWarning(DateTime.Now.ToString());
        }

        public async Task ExecutePunishment(ModCase modCase)
        {
            throw new System.NotImplementedException();
        }

        public async Task UndoPunishment(ModCase modCase)
        {
            throw new System.NotImplementedException();
        }
    }
}
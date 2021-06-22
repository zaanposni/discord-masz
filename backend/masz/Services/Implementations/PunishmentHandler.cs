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
    public class PunishmentHandler : IPunishmentHandler
    {
        private readonly ILogger<PunishmentHandler> logger;
        private readonly IOptions<PunishmentHandler> config;
        private readonly IDiscordAPIInterface discord;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public PunishmentHandler() { }

        public PunishmentHandler(ILogger<PunishmentHandler> logger, IOptions<PunishmentHandler> config, IDiscordAPIInterface discord, IServiceScopeFactory serviceScopeFactory)
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
                        CheckAllCurrentPunishments();
                        Thread.Sleep(1000 * 60);
                    }
                });
            task.Start();
            logger.LogWarning("Finished action loop.");
        }

        public async void CheckAllCurrentPunishments()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();
                List<ModCase> cases = await database.SelectAllModCasesWithActivePunishments();
                
                foreach (var element in cases)
                {
                    if (element.PunishedUntil != null)
                    {
                        if (element.PunishedUntil <= DateTime.UtcNow)
                        {
                            await UndoPunishment(element);
                            element.PunishmentActive = false;
                            database.UpdateModCase(element);
                        }
                    }
                }
                await database.SaveChangesAsync();
            }
        }

        public async Task ExecutePunishment(ModCase modCase)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                GuildConfig guildConfig = await database.SelectSpecificGuildConfig(modCase.GuildId);
                if (guildConfig == null) {
                    logger.LogError($"Punisher: Cannot execute punishment in guild {modCase.GuildId} - guildconfig not found.");
                    return;
                }
                switch (modCase.PunishmentType) {
                    case PunishmentType.Mute:
                        if (guildConfig.MutedRoles.Length != 0) {
                            logger.LogInformation($"Punisher: Mute User {modCase.UserId} in guild {modCase.GuildId} with role {guildConfig.MutedRoles}.");
                            foreach (string role in guildConfig.MutedRoles)
                            {
                                await discord.GrantGuildUserRole(modCase.GuildId, modCase.UserId, role);
                            }
                        } else {
                            logger.LogInformation($"Punisher: Cannot Mute User {modCase.UserId} in guild {modCase.GuildId} - mute role undefined.");
                        }
                        break;
                    case PunishmentType.Ban:
                        logger.LogInformation($"Punisher: Ban User {modCase.UserId} in guild {modCase.GuildId}.");
                        await discord.BanUser(modCase.GuildId, modCase.UserId);
                        await discord.GetGuildUserBan(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreCache);  // refresh ban cache
                        break;
                    case PunishmentType.Kick:
                        logger.LogInformation($"Punisher: Kick User {modCase.UserId} in guild {modCase.GuildId}.");
                        await discord.KickGuildUser(modCase.GuildId, modCase.UserId);
                        break;
                }
            }
        }

        public async Task UndoPunishment(ModCase modCase)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

                List<ModCase> parallelCases = await database.SelectAllModCasesThatHaveParallelPunishment(modCase);
                if (parallelCases.Count != 0) {
                    logger.LogInformation("Cannot undo punishment. There exists a parallel punishment for this case");
                    return;
                }
                GuildConfig guildConfig = await database.SelectSpecificGuildConfig(modCase.GuildId);
                if (guildConfig == null) {
                    logger.LogError($"Punisher: Cannot execute punishment in guild {modCase.GuildId} - guildconfig not found.");
                    return;
                }
                switch (modCase.PunishmentType) {
                    case PunishmentType.Mute:
                        if (guildConfig.MutedRoles.Length != 0) {
                            logger.LogInformation($"Punisher: Unmute User {modCase.UserId} in guild {modCase.GuildId} with role {guildConfig.MutedRoles}.");                            
                            foreach (string role in guildConfig.MutedRoles)
                            {
                                await discord.RemoveGuildUserRole(modCase.GuildId, modCase.UserId, role);
                            }
                        } else {
                            logger.LogInformation($"Punisher: Cannot Unmute User {modCase.UserId} in guild {modCase.GuildId} - mute role undefined.");
                        }
                        break;
                    case PunishmentType.Ban:
                        logger.LogInformation($"Punisher: Unban User {modCase.UserId} in guild {modCase.GuildId}.");
                        await discord.UnBanUser(modCase.GuildId, modCase.UserId);
                        await discord.GetGuildUserBan(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreCache);  // refresh ban cache
                        break;
                }
            }
        }
    }
}
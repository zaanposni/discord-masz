using Discord.WebSocket;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;

namespace MASZ.Services
{
    public class PunishmentHandler : IPunishmentHandler
    {
        private readonly ILogger<PunishmentHandler> _logger;
        private readonly IDiscordAPIInterface _discord;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PunishmentHandler() { }

        public PunishmentHandler(ILogger<PunishmentHandler> logger, IDiscordAPIInterface discord, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _discord = discord;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public void StartTimer()
        {
            _logger.LogWarning("Starting action loop.");
            Task task = new(() =>
                {
                    while (true)
                    {
                        CheckAllCurrentPunishments();
                        Thread.Sleep(1000 * 60);
                    }
                });
            task.Start();
            _logger.LogWarning("Finished action loop.");
        }

        public async void CheckAllCurrentPunishments()
        {
            using var scope = _serviceScopeFactory.CreateScope();
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

        public async Task ExecutePunishment(ModCase modCase)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(modCase.GuildId);
            }
            catch (ResourceNotFoundException)
            {
                _logger.LogError($"Cannot execute punishment in guild {modCase.GuildId} - guildconfig not found.");
                return;
            }

            switch (modCase.PunishmentType)
            {
                case PunishmentType.Mute:
                    if (guildConfig.MutedRoles.Length != 0)
                    {
                        _logger.LogInformation($"Mute User {modCase.UserId} in guild {modCase.GuildId} with roles " + string.Join(',', guildConfig.MutedRoles.Select(x => x.ToString())));
                        foreach (ulong role in guildConfig.MutedRoles)
                        {
                            await _discord.GrantGuildUserRole(modCase.GuildId, modCase.UserId, role);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Cannot Mute User {modCase.UserId} in guild {modCase.GuildId} - mute role undefined.");
                    }
                    break;
                case PunishmentType.Ban:
                    _logger.LogInformation($"Ban User {modCase.UserId} in guild {modCase.GuildId}.");
                    await _discord.BanUser(modCase.GuildId, modCase.UserId);
                    await _discord.GetGuildUserBan(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreCache);  // refresh ban cache
                    break;
                case PunishmentType.Kick:
                    _logger.LogInformation($"Kick User {modCase.UserId} in guild {modCase.GuildId}.");
                    await _discord.KickGuildUser(modCase.GuildId, modCase.UserId);
                    break;
            }
        }

        public async Task UndoPunishment(ModCase modCase)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

            List<ModCase> parallelCases = await database.SelectAllModCasesThatHaveParallelPunishment(modCase);
            if (parallelCases.Count != 0)
            {
                _logger.LogInformation("Cannot undo punishment. There exists a parallel punishment for this case");
                return;
            }

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(modCase.GuildId);
            }
            catch (ResourceNotFoundException)
            {
                _logger.LogError($"Cannot execute punishment in guild {modCase.GuildId} - guildconfig not found.");
                return;
            }

            switch (modCase.PunishmentType)
            {
                case PunishmentType.Mute:
                    if (guildConfig.MutedRoles.Length != 0)
                    {
                        _logger.LogInformation($"Unmute User {modCase.UserId} in guild {modCase.GuildId} with roles " + string.Join(',', guildConfig.MutedRoles.Select(x => x.ToString())));
                        foreach (ulong role in guildConfig.MutedRoles)
                        {
                            await _discord.RemoveGuildUserRole(modCase.GuildId, modCase.UserId, role);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Cannot Unmute User {modCase.UserId} in guild {modCase.GuildId} - mute role undefined.");
                    }
                    break;
                case PunishmentType.Ban:
                    _logger.LogInformation($"Unban User {modCase.UserId} in guild {modCase.GuildId}.");
                    await _discord.UnBanUser(modCase.GuildId, modCase.UserId);
                    await _discord.GetGuildUserBan(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreCache);  // refresh ban cache
                    break;
            }
        }

        public async Task HandleMemberJoin(SocketGuildUser user)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            IDatabase database = scope.ServiceProvider.GetService<IDatabase>();

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(user.Guild.Id);
            }
            catch (ResourceNotFoundException)
            {
                _logger.LogInformation($"Cannot execute punishment in guild {user.Guild.Id} - guildconfig not found.");
                return;
            }

            if (guildConfig.MutedRoles.Length == 0)
            {
                return;
            }

            List<ModCase> modCases = await database.SelectAllModCasesWithActiveMuteForGuildAndUser(user.Guild.Id, user.Id);
            if (modCases.Count == 0)
            {
                return;
            }

            _logger.LogInformation($"Muted member {user.Id} rejoined guild {user.Guild.Id}");

            await ExecutePunishment(modCases[0]);
        }
    }
}
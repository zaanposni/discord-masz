using Discord.WebSocket;
using MASZ.Data;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using Timer = System.Timers.Timer;

namespace MASZ.Services
{
    public class Punishments
    {
        private readonly ILogger<Punishments> _logger;
        private readonly DiscordAPIInterface _discord;
        private readonly IServiceProvider _serviceProvider;

        public Punishments(ILogger<Punishments> logger, DiscordAPIInterface discord, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _discord = discord;
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogWarning("Starting action loop.");

            Timer minuteTimer = new(TimeSpan.FromMinutes(1).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            Timer dailyTimer = new(TimeSpan.FromDays(1).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            minuteTimer.Elapsed += (s, e) => CheckAllCurrentPunishments();
            dailyTimer.Elapsed += (s, e) => CheckAllActiveTimeouts();

            await Task.Run(() => minuteTimer.Start());
            await Task.Run(() => dailyTimer.Start());

            CheckAllCurrentPunishments();
            CheckAllActiveTimeouts();

            _logger.LogWarning("Finished action loop.");
        }

        public async void CheckAllActiveTimeouts()
        {
            using var scope = _serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();
            List<ModCase> cases = await database.SelectAllModCasesWithActivePunishments();

            foreach (var element in cases)
            {
                if (element.PunishmentType == PunishmentType.Mute)
                {
                    if (element.PunishedUntil == null || element.PunishedUntil > DateTime.UtcNow)
                    {
                        await ExecutePunishment(element);
                    }
                }
            }
        }

        public async void CheckAllCurrentPunishments()
        {
            using var scope = _serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();
            List<ModCase> cases = await database.SelectAllModCasesWithActivePunishments();

            foreach (var element in cases)
            {
                if (element.PunishedUntil != null)
                {
                    if (element.PunishedUntil <= DateTime.UtcNow)
                    {
                        try
                        {
                            await UndoPunishment(element);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogCritical(ex, $"Something went wrong while undoing punishment for modcase {element.GuildId}/{element.CaseId} ({element.Id}).");
                        }
                        element.PunishmentActive = false;
                        database.UpdateModCase(element);
                    }
                }
            }
            await database.SaveChangesAsync();
        }

        public async Task ExecutePunishment(ModCase modCase)
        {
            using var scope = _serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();

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

            string reason = null;
            try
            {
                Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();
                reason = translator.T(guildConfig).NotificationDiscordAuditLogPunishmentsExecute(modCase.CaseId, modCase.LastEditedByModId, modCase.Title.Truncate(400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to resolve audit log reason string for case {modCase.GuildId}/{modCase.CaseId}");
            }
            switch (modCase.PunishmentType)
            {
                case PunishmentType.Mute:
                    if (guildConfig.MutedRoles.Length != 0)
                    {
                        _logger.LogInformation($"Mute User {modCase.UserId} in guild {modCase.GuildId} with roles " + string.Join(',', guildConfig.MutedRoles.Select(x => x.ToString())));
                        foreach (ulong role in guildConfig.MutedRoles)
                        {
                            await _discord.GrantGuildUserRole(modCase.GuildId, modCase.UserId, role, reason);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Timeout User {modCase.UserId} in guild {modCase.GuildId}");
                        DateTime until = DateTime.UtcNow.AddDays(27);
                        if (modCase.PunishedUntil.HasValue && modCase.PunishedUntil.Value < until)
                        {
                            until = modCase.PunishedUntil.Value;
                        }
                        await _discord.TimeoutGuildUser(modCase.GuildId, modCase.UserId, until, reason);
                    }
                    break;
                case PunishmentType.Ban:
                    _logger.LogInformation($"Ban User {modCase.UserId} in guild {modCase.GuildId}.");
                    await _discord.BanUser(modCase.GuildId, modCase.UserId, reason);
                    await _discord.GetGuildUserBan(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreCache);  // refresh ban cache
                    break;
                case PunishmentType.Kick:
                    _logger.LogInformation($"Kick User {modCase.UserId} in guild {modCase.GuildId}.");
                    await _discord.KickGuildUser(modCase.GuildId, modCase.UserId, reason);
                    break;
            }
        }

        public async Task UndoPunishment(ModCase modCase)
        {
            using var scope = _serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();

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

            string reason = null;
            try
            {
                Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();
                reason = translator.T(guildConfig).NotificationDiscordAuditLogPunishmentsUndone(modCase.CaseId, modCase.Title.Truncate(400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to resolve audit log reason string for case {modCase.GuildId}/{modCase.CaseId}");
            }
            switch (modCase.PunishmentType)
            {
                case PunishmentType.Mute:
                    if (guildConfig.MutedRoles.Length != 0)
                    {
                        _logger.LogInformation($"Unmute User {modCase.UserId} in guild {modCase.GuildId} with roles " + string.Join(',', guildConfig.MutedRoles.Select(x => x.ToString())));
                        foreach (ulong role in guildConfig.MutedRoles)
                        {
                            await _discord.RemoveGuildUserRole(modCase.GuildId, modCase.UserId, role, reason);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Remove timeout from user {modCase.UserId} in guild {modCase.GuildId}");
                        await _discord.RemoveTimeoutGuildUser(modCase.GuildId, modCase.UserId, reason);
                    }
                    break;
                case PunishmentType.Ban:
                    _logger.LogInformation($"Unban User {modCase.UserId} in guild {modCase.GuildId}.");
                    await _discord.UnBanUser(modCase.GuildId, modCase.UserId, reason);
                    _discord.RemoveFromCache(CacheKey.GuildBan(modCase.GuildId, modCase.UserId));  // refresh ban cache
                    break;
            }
        }

        public async Task HandleMemberJoin(SocketGuildUser user)
        {
            using var scope = _serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();

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
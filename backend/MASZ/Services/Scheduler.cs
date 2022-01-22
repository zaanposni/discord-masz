using Discord;
using Discord.Net;
using MASZ.Data;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Utils;
using System.Net;
using Timer = System.Timers.Timer;

namespace MASZ.Services
{
    public class Scheduler : IEvent
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordAPIInterface _discordAPI;
        private readonly FilesHandler _filesHandler;
        private readonly IServiceProvider _serviceProvider;
        private readonly IdentityManager _identityManager;
        private readonly InternalEventHandler _eventHandler;
        private DateTime _nextCacheSchedule;
        private readonly int _cacheIntervalMinutes = 15;

        public Scheduler(ILogger<Scheduler> logger, InternalConfiguration config, DiscordAPIInterface discord, IServiceProvider serviceProvider, FilesHandler filesHandler, IdentityManager identityManager, InternalEventHandler eventHandler)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discord;
            _serviceProvider = serviceProvider;
            _filesHandler = filesHandler;
            _identityManager = identityManager;
            _eventHandler = eventHandler;
        }

        public void RegisterEvents()
        {
            _eventHandler.OnGuildRegistered += HandleGuildRegister;
        }

        private async Task HandleGuildRegister(GuildConfig guildConfig, bool importExistingBans)
        {
            using var scope = _serviceProvider.CreateScope();

            var discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            var scheduler = scope.ServiceProvider.GetRequiredService<Scheduler>();
            var translator = scope.ServiceProvider.GetRequiredService<Translator>();

            await scheduler.CacheAllKnownGuilds();
            await scheduler.CacheAllGuildMembers(new List<ulong>());
            await scheduler.CacheAllGuildBans(new List<ulong>());

            if (importExistingBans)
            {
                translator.SetContext(guildConfig);
                ModCaseRepository modCaseRepository = ModCaseRepository.CreateWithBotIdentity(scope.ServiceProvider);
                foreach (IBan ban in await discordAPI.GetGuildBans(guildConfig.GuildId, CacheBehavior.OnlyCache))
                {
                    ModCase modCase = new()
                    {
                        Title = string.IsNullOrEmpty(ban.Reason) ? translator.T().ImportedFromExistingBans() : ban.Reason,
                        Description = string.IsNullOrEmpty(ban.Reason) ? translator.T().ImportedFromExistingBans() : ban.Reason,
                        GuildId = guildConfig.GuildId,
                        UserId = ban.User.Id,
                        Username = ban.User.Username,
                        Labels = new[] { translator.T().Imported() },
                        Discriminator = ban.User.Discriminator,
                        CreationType = CaseCreationType.Imported,
                        PunishmentType = PunishmentType.Ban,
                        PunishedUntil = null
                    };
                    await modCaseRepository.ImportModCase(modCase);
                }
            }
        }

        public async Task ExecuteAsync()
        {
            _logger.LogWarning("Starting schedule timers.");

            Timer EventTimer = new(TimeSpan.FromMinutes(_cacheIntervalMinutes).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            Timer MinuteEventTimer = new(TimeSpan.FromMinutes(1).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };

            EventTimer.Elapsed += (s, e) => LoopThroughCaches();
            MinuteEventTimer.Elapsed += async (s, e) => await CheckDueScheduledMessages();

            await Task.Run(() => EventTimer.Start());
            await Task.Run(() => MinuteEventTimer.Start());

            _logger.LogWarning("Started schedule timers.");

            LoopThroughCaches();
        }

        public async Task CheckDueScheduledMessages()
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = ScheduledMessageRepository.CreateWithBotIdentity(scope.ServiceProvider);
            var messages = await repo.GetDueMessages();

            foreach (ScheduledMessage message in messages)
            {
                _logger.LogInformation($"Handling scheduled message {message.Id} for {message.GuildId}/{message.ChannelId} by {message.CreatorId}/{message.LastEditedById}.");
                IGuild guild;
                ITextChannel channel;
                try
                {
                    guild = _discordAPI.FetchGuildInfo(message.GuildId, CacheBehavior.OnlyCache);
                    if (guild == null)
                        throw new Exception();
                } catch (Exception)
                {
                    _logger.LogInformation($"Failed scheduled message {message.Id}. Reason unknown.");
                    await repo.SetMessageAsFailed(message.Id, ScheduledMessageFailureReason.Unknown);
                    continue;
                }

                try
                {
                    channel = await guild.GetTextChannelAsync(message.ChannelId);
                    if (channel == null)
                        throw new Exception();
                }
                catch (Exception)
                {
                    _logger.LogInformation($"Failed scheduled message {message.Id}. Reason channel not found.");
                    await repo.SetMessageAsFailed(message.Id, ScheduledMessageFailureReason.ChannelNotFound);
                    continue;
                }

                try
                {
                    await channel.SendMessageAsync(message.Content);
                    await repo.SetMessageAsSent(message.Id);
                    _logger.LogInformation($"Sent scheduled message {message.Id} for {message.GuildId}/{message.ChannelId} by {message.CreatorId}/{message.LastEditedById}.");
                }
                catch (HttpException e)
                {
                    if (e.HttpCode == HttpStatusCode.Unauthorized || e.HttpCode == HttpStatusCode.Forbidden)
                    {
                        _logger.LogInformation($"Failed scheduled message {message.Id}. Reason insufficient permission.");
                        await repo.SetMessageAsFailed(message.Id, ScheduledMessageFailureReason.InsufficientPermission);
                    }
                    else
                    {
                        _logger.LogInformation($"Failed scheduled message {message.Id}. Reason unknown");
                        await repo.SetMessageAsFailed(message.Id, ScheduledMessageFailureReason.Unknown);
                    }
                }
                catch (Exception)
                {
                    _logger.LogInformation($"Failed scheduled message {message.Id}. Reason unknown");
                    await repo.SetMessageAsFailed(message.Id, ScheduledMessageFailureReason.Unknown);
                }
            }
        }

        public void LoopThroughCaches()
        {
            try
            {
                _nextCacheSchedule = DateTime.UtcNow.AddMinutes(_cacheIntervalMinutes);
                CheckDeletedCases();
                CacheAll();
                _identityManager.ClearOldIdentities();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in caching.");
            }
        }

        public async void CheckDeletedCases()
        {
            _logger.LogInformation("Casebin | Checking case bin and delete old cases.");
            using (var scope = _serviceProvider.CreateScope())
            {
                Database database = scope.ServiceProvider.GetRequiredService<Database>();

                foreach (ModCase modCase in await database.SelectAllModcasesMarkedAsDeleted())
                {
                    try
                    {
                        _filesHandler.DeleteDirectory(Path.Combine(_config.GetFileUploadPath(), modCase.GuildId.ToString(), modCase.CaseId.ToString()));
                    }
                    catch (Exception e)
                    {
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
            List<ulong> handledUsers = new();
            handledUsers = await CacheAllGuildBans(handledUsers);
            handledUsers = await CacheAllGuildMembers(handledUsers);
            handledUsers = await CacheAllKnownUsers(handledUsers);
            _logger.LogInformation($"Cacher | Done with {handledUsers.Count} entries.");
            _eventHandler.OnInternalCachingDoneEvent.InvokeAsync(handledUsers.Count, GetNextCacheSchedule());
        }

        public async Task CacheAllKnownGuilds()
        {
            _logger.LogInformation("Cacher | Cache all registered guilds.");
            using var scope = _serviceProvider.CreateScope();
            Database database = scope.ServiceProvider.GetRequiredService<Database>();

            foreach (var guild in await database.SelectAllGuildConfigs())
            {
                _discordAPI.FetchGuildInfo(guild.GuildId, CacheBehavior.IgnoreCache);
                _discordAPI.FetchGuildChannels(guild.GuildId, CacheBehavior.IgnoreCache);
            }
        }

        public async Task<List<ulong>> CacheAllGuildMembers(List<ulong> handledUsers)
        {
            _logger.LogInformation("Cacher | Cache all members of registered guilds.");
            using (var scope = _serviceProvider.CreateScope())
            {
                Database database = scope.ServiceProvider.GetRequiredService<Database>();

                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    var members = await _discordAPI.FetchGuildMembers(guild.GuildId, CacheBehavior.IgnoreCache);
                    if (members != null)
                    {
                        foreach (var item in members)
                        {
                            if (!handledUsers.Contains(item.Id))
                            {
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
            using (var scope = _serviceProvider.CreateScope())
            {
                Database database = scope.ServiceProvider.GetRequiredService<Database>();

                foreach (var guild in await database.SelectAllGuildConfigs())
                {
                    List<IBan> bans = await _discordAPI.GetGuildBans(guild.GuildId, CacheBehavior.IgnoreCache);
                    if (bans != null)
                    {
                        foreach (IBan ban in bans)
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
            using (var scope = _serviceProvider.CreateScope())
            {
                Database database = scope.ServiceProvider.GetRequiredService<Database>();

                foreach (var modCase in await database.SelectLatestModCases(DateTime.UtcNow.AddYears(-3), 750))
                {
                    if (!handledUsers.Contains(modCase.UserId))
                    {
                        await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.UserId);
                    }
                    if (!handledUsers.Contains(modCase.ModId))
                    {
                        await _discordAPI.FetchUserInfo(modCase.ModId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.ModId);
                    }
                    if (!handledUsers.Contains(modCase.LastEditedByModId))
                    {
                        await _discordAPI.FetchUserInfo(modCase.LastEditedByModId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(modCase.LastEditedByModId);
                    }
                }

                foreach (var userNote in await database.SelectLatestUserNotes(DateTime.UtcNow.AddYears(-3), 100))
                {
                    if (!handledUsers.Contains(userNote.UserId))
                    {
                        await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userNote.UserId);
                    }
                    if (!handledUsers.Contains(userNote.CreatorId))
                    {
                        await _discordAPI.FetchUserInfo(userNote.CreatorId, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userNote.CreatorId);
                    }
                }

                foreach (var userMapping in await database.SelectLatestUserMappings(DateTime.UtcNow.AddYears(-3), 100))
                {
                    if (!handledUsers.Contains(userMapping.UserA))
                    {
                        await _discordAPI.FetchUserInfo(userMapping.UserA, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.UserA);
                    }
                    if (!handledUsers.Contains(userMapping.UserB))
                    {
                        await _discordAPI.FetchUserInfo(userMapping.UserB, CacheBehavior.IgnoreCache);
                        handledUsers.Add(userMapping.UserB);
                    }
                    if (!handledUsers.Contains(userMapping.CreatorUserId))
                    {
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
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using MASZ.AutoModeration;
using MASZ.Commands;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.InviteTracking;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Utils;
using RestSharp;
using System.Reflection;

namespace MASZ.Services
{
    public class DiscordBot : IEvent
    {
        private readonly ILogger<DiscordBot> _logger;
        private readonly DiscordSocketClient _client;
        private readonly InternalConfiguration _internalConfiguration;
        private readonly InteractionService _interactions;
        private readonly Scheduler _scheduler;
        private readonly Punishments _punishments;
        private readonly IServiceProvider _serviceProvider;
        private readonly InternalEventHandler _eventHandler;
        private bool _firstReady = true;
        private bool _isRunning = false;
        private DateTime? _lastDisconnect = null;

        public DiscordBot(ILogger<DiscordBot> logger, DiscordSocketClient client, InternalConfiguration internalConfiguration, InteractionService interactions, IServiceProvider serviceProvider, Scheduler scheduler, Punishments punishments, InternalEventHandler eventHandler)
        {
            _logger = logger;
            _client = client;
            _internalConfiguration = internalConfiguration;
            _interactions = interactions;
            _scheduler = scheduler;
            _punishments = punishments;
            _serviceProvider = serviceProvider;
            _eventHandler = eventHandler;
        }

        public async Task ExecuteAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            try
            {
                await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modules could not initialize!");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, _internalConfiguration.GetBotToken());
            await _client.StartAsync();

            string botStatus = _internalConfiguration.GetDiscordBotStatus();
            if (string.IsNullOrWhiteSpace(botStatus))
            {
                await _client.SetGameAsync(_internalConfiguration.GetBaseUrl(), type: ActivityType.Watching);
            }
            else if (botStatus == "none")
            {
                await _client.SetGameAsync(null);
            }
            else
            {
                await _client.SetGameAsync(botStatus);
            }
        }

        public void RegisterEvents()
        {
            _client.MessageReceived += MessageCreatedHandler;
            _client.MessageUpdated += MessageUpdatedHandler;
            _client.JoinedGuild += GuildCreatedHandler;
            _client.UserJoined += GuildMemberAddedHandler;
            _client.GuildMemberUpdated += GuildMemberUpdatedHandler;
            _client.UserLeft += GuildUserRemoved;
            _client.InviteCreated += InviteCreatedHandler;
            _client.UserBanned += GuildBanAdded;
            _client.UserUnbanned += GuildBanRemoved;
            _client.GuildUpdated += GuildUpdatedHandler;
            _client.ChannelCreated += ChannelCreatedHandler;
            _client.ChannelUpdated += ChannelUpdatedHandler;
            _client.ChannelDestroyed += ChannelDestroyedHandler;
            _client.GuildAvailable += GuildAvailableHandler;
            _client.ThreadCreated += ThreadCreatedHandler;

            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Ready += ReadyHandler;

            _client.InteractionCreated += HandleInteraction;

            _interactions.SlashCommandExecuted += CmdErroredHandler;
            _interactions.ContextCommandExecuted += CmdErroredHandler;

            var client_logger = _serviceProvider.GetRequiredService<ILogger<DiscordSocketClient>>();

            _client.Log += (logLevel) => Log(logLevel, client_logger);

            var interactions_logger = _serviceProvider.GetRequiredService<ILogger<InteractionService>>();

            _interactions.Log += (logLevel) => Log(logLevel, interactions_logger);

            _client.ModalSubmitted += async (modal) =>
            {
                try
                {
                    await HandleModalSubmitted(modal);
                }
                catch (BaseAPIException ex)
                {
                    _logger.LogError($"Modal '{modal.Data.CustomId}' invoked by '{modal.User.Username}' failed: {ex.Error}");

                    using var scope = _serviceProvider.CreateScope();
                    Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();
                    if (modal.GuildId.HasValue && ex is not UnregisteredGuildException)
                    {
                        await translator.SetContext(modal.GuildId.Value);
                    }

                    string errorCode = "#" + ((int)ex.Error).ToString("D4");

                    EmbedBuilder builder = new EmbedBuilder()
                        .WithTitle(translator.T().SomethingWentWrong())
                        .WithColor(Color.Red)
                        .WithDescription(translator.T().Enum(ex.Error))
                        .WithCurrentTimestamp()
                        .WithFooter($"{translator.T().Code()} {errorCode}");

                    try
                    {
                        await modal.RespondAsync(embed: builder.Build(), ephemeral: true);
                    }
                    catch (TimeoutException)
                    {
                        await modal.Channel.SendMessageAsync(embed: builder.Build());
                    }
                    catch (InvalidOperationException)
                    {
                        await modal.ModifyOriginalResponseAsync(m =>
                        {
                            m.Content = "";
                            m.Embed = builder.Build();
                        });
                    }
                }
            };

            _eventHandler.OnGuildRegistered += GuildRegisteredHandler;
            _eventHandler.OnGuildDeleted += GuildDeletedHandler;
        }

        private async Task HandleModalSubmitted(SocketModal modal)
        {
            string[] modalIdParts = modal.Data.CustomId.Split(":");
            switch (string.Join(":", modalIdParts.Take(2)))
            {
                case "punish:warn":
                case "punish:mute":
                case "punish:kick":
                case "punish:ban":
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        Commands.PunishModal punishModal = new(scope.ServiceProvider, modal.User);
                        await punishModal.CheckRequirementsAsync(modal, scope.ServiceProvider);
                        await punishModal.BeforeExecute();
                        await punishModal.HandleModal(modal);
                    }
                    break;
            }
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);

                await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to execute {arg.Type} in channel {arg.Channel}");

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public DateTime? GetLastDisconnectTime()
        {
            return _lastDisconnect;
        }

        private Task Connected()
        {
            _logger.LogInformation("Client connected.");
            _isRunning = true;

            return Task.CompletedTask;
        }

        private Task Disconnected(Exception _)
        {
            _logger.LogCritical("Client disconnected.");
            _isRunning = false;
            _lastDisconnect = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        private async Task ReadyHandler()
        {
            _logger.LogInformation("Client connected.");
            _isRunning = true;

            using var scope = _serviceProvider.CreateScope();
            try
            {
                var guilds = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetAllGuildConfigs();
                foreach (var guild in guilds)
                {
                    await _interactions.RegisterCommandsToGuildAsync(
                        guild.GuildId,
                        true
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Something went wrong while registering guild commands.");
                scope.Dispose();
            }

            try
            {
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while overwriting global application commands.");
            }

            if (_firstReady)
            {
                _firstReady = false;
                try
                {
                    await _scheduler.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Something went wrong while starting the scheduler timer.");
                }
                try
                {
                    await _punishments.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Something went wrong while starting the punishmenthandling timer.");
                }
            }
        }

        private static Task Log(LogMessage logMessage, ILogger logger)
        {
            LogLevel level = logMessage.Severity switch
            {
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Warning => LogLevel.Warning,
                _ => throw new NotImplementedException()
            };

            if (logMessage.Exception == null)
                logger.Log(level, logMessage.Message);
            else
                logger.LogError(logMessage.Exception, logMessage.Message);

            return Task.CompletedTask;
        }

        private async Task GuildRegisteredHandler(GuildConfig guild, IUser user, bool importExistingStuff)
        {
            await _interactions.RegisterCommandsToGuildAsync(
                guild.GuildId,
                true
            );

            _logger.LogInformation($"Initialized guild commands for guild {guild.GuildId}.");

        }

        private async Task GuildDeletedHandler(GuildConfig guild, IUser user)
        {
            var restClient = new RestClient($"https://discord.com");

            var request = new RestRequest($"api/v8/applications/{_client.CurrentUser.Id}/guilds/{guild.GuildId}/commands", Method.Put);

            request.AddJsonBody(Array.Empty<object>());
            request.AddHeader("Authorization", $"Bot {_internalConfiguration.GetBotToken()}");

            var response = await restClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                _logger.LogInformation($"Deleted guild commands for guild {guild.GuildId}.");
            }
            else
            {
                _logger.LogError($"Unable to delete guild commands for guild {guild.GuildId}.");
                _logger.LogError(response.StatusCode.ToString());
                _logger.LogError(response.Content);
            }
        }

        private Task GuildBanRemoved(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceProvider.CreateScope();

            // Refresh ban cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.RemoveFromCache(CacheKey.GuildBan(guild.Id, user.Id));

            return Task.CompletedTask;
        }

        private async Task GuildBanAdded(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceProvider.CreateScope();

            // Refresh ban cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            await discordAPI.GetGuildUserBan(guild.Id, user.Id, CacheBehavior.IgnoreCache);
            discordAPI.RemoveFromCache(CacheKey.GuildMember(guild.Id, user.Id));

            // Refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == user.Id)
                {
                    identity.RemoveGuildMembership(guild.Id);
                }
            }

            // Create dummy modcase
            try
            {
                GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guild.Id);

                if (!guildConfig.SyncPunishments)
                {
                    return;
                }
            }
            catch (UnregisteredGuildException) { return; }

            var auditLogs = guild.GetAuditLogsAsync(10, actionType: ActionType.Ban);
            ulong? moderator = null;
            string reason = null;
            foreach (var log in await auditLogs.FlattenAsync())
            {
                if (log.Data is BanAuditLogData banData)
                {
                    if (banData.Target?.Id == user.Id)
                    {
                        moderator = log.User.Id;
                        reason = log.Reason;
                        break;
                    }
                }
            }

            if (!moderator.HasValue || moderator.Value == _client.CurrentUser.Id)
            {
                return;
            }

            _logger.LogInformation($"Syncing ban for user {user.Id} in guild {guild.Id}.");

            ModCase modCase = new()
            {
                Title = string.IsNullOrEmpty(reason) ? "Imported Ban" : reason,
                GuildId = guild.Id,
                UserId = user.Id,
                ModId = moderator.Value,
                Description = string.IsNullOrEmpty(reason) ? "None" : reason,
                PunishmentType = PunishmentType.Ban,
                PunishmentActive = true,
                PunishedUntil = null,
                CreationType = CaseCreationType.Imported
            };

            await ModCaseRepository.CreateWithBotIdentity(scope.ServiceProvider).CreateModCase(modCase, false, false, false);
        }

        private Task GuildUserRemoved(SocketGuild guild, SocketUser usr)
        {
            using var scope = _serviceProvider.CreateScope();

            // Refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == usr.Id)
                {
                    identity.RemoveGuildMembership(guild.Id);
                }
            }

            return Task.CompletedTask;
        }

        private async Task GuildMemberUpdatedHandler(Cacheable<SocketGuildUser, ulong> oldUsrCached, SocketGuildUser newUsr)
        {
            using var scope = _serviceProvider.CreateScope();

            // Refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == newUsr.Id)
                {
                    identity.UpdateGuildMembership(newUsr);
                }
            }

            // Check zalgo
            ZalgoRepository repo = ZalgoRepository.CreateWithBotIdentity(scope.ServiceProvider);
            try
            {
                ZalgoConfig zalgoConfig = await repo.GetZalgo(newUsr.Guild.Id);
                await repo.CheckZalgoForMember(newUsr.Guild.Id, zalgoConfig, newUsr, true);
            }
            catch (ResourceNotFoundException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while checking zalgo for member.");
            }

            // Refresh member cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.AddOrUpdateCache(CacheKey.GuildMember(newUsr.Id, newUsr.Id), new CacheApiResponse(newUsr));

            return;
        }

        private Task ChannelDestroyedHandler(SocketChannel arg)
        {
            if (arg is IGuildChannel channel)
            {
                using var scope = _serviceProvider.CreateScope();

                // Refresh channel cache
                DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();

                List<IGuildChannel> channels = discordAPI.GetFromCache<List<IGuildChannel>>(CacheKey.GuildChannels(channel.GuildId));
                if (channels == null)
                {
                    channels = new List<IGuildChannel>();
                }

                channels.Remove(channel);

                discordAPI.AddOrUpdateCache(CacheKey.GuildChannels(channel.GuildId), new CacheApiResponse(channels));
            }

            return Task.CompletedTask;
        }

        private Task ChannelUpdatedHandler(SocketChannel arg1, SocketChannel arg2)
        {
            if (arg2 is IGuildChannel channel)
            {
                using var scope = _serviceProvider.CreateScope();

                // Refresh channel cache
                DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();

                List<IGuildChannel> channels = discordAPI.GetFromCache<List<IGuildChannel>>(CacheKey.GuildChannels(channel.GuildId));
                if (channels == null)
                {
                    channels = new List<IGuildChannel>();
                }

                int index = channels.FindIndex(x => x.Id == channel.Id);
                if (index != -1)
                {
                    channels[index] = channel;
                }
                else
                {
                    channels.Add(channel);
                }

                discordAPI.AddOrUpdateCache(CacheKey.GuildChannels(channel.GuildId), new CacheApiResponse(channels));
            }

            return Task.CompletedTask;
        }

        private Task ChannelCreatedHandler(SocketChannel arg)
        {
            if (arg is IGuildChannel channel)
            {
                using var scope = _serviceProvider.CreateScope();

                // Refresh channel cache
                DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();

                List<IGuildChannel> channels = discordAPI.GetFromCache<List<IGuildChannel>>(CacheKey.GuildChannels(channel.GuildId));
                if (channels == null)
                {
                    channels = new List<IGuildChannel>();
                }

                channels.Add(channel);

                discordAPI.AddOrUpdateCache(CacheKey.GuildChannels(channel.GuildId), new CacheApiResponse(channels));
            }

            return Task.CompletedTask;
        }

        private async Task ThreadCreatedHandler(SocketThreadChannel channel)
        {
            await channel.JoinAsync();
        }

        private async Task MessageCreatedHandler(SocketMessage message)
        {
            if (message.Channel is ITextChannel channel)
            {

                using var scope = _serviceProvider.CreateScope();

                if (message.Type != MessageType.Default && message.Type != MessageType.Reply)
                {
                    return;
                }
                if (message.Author.IsBot)
                {
                    return;
                }
                if (channel.Guild == null)
                {
                    return;
                }

                AutoModerator autoModerator = null;
                try
                {
                    autoModerator = await AutoModerator.CreateDefault(_client, channel.Guild.Id, scope.ServiceProvider);
                }
                catch (ResourceNotFoundException)
                {
                    return;
                }
                await autoModerator.HandleAutomoderation(message);
            }
        }

        private async Task MessageUpdatedHandler(Cacheable<IMessage, ulong> oldMsg, SocketMessage newMsg, ISocketMessageChannel channel)
        {
            if (channel is ITextChannel txtChannel)
            {
                using var scope = _serviceProvider.CreateScope();

                if (newMsg.Type != MessageType.Default && newMsg.Type != MessageType.Reply)
                {
                    return;
                }
                if (newMsg.Author.IsBot)
                {
                    return;
                }
                if (txtChannel.Guild == null)
                {
                    return;
                }

                AutoModerator autoModerator = null;
                try
                {
                    autoModerator = await AutoModerator.CreateDefault(_client, txtChannel.Guild.Id, scope.ServiceProvider);
                }
                catch (ResourceNotFoundException)
                {
                    return;
                }
                await autoModerator.HandleAutomoderation(newMsg, true);
            }

            return;
        }

        private async Task<List<TrackedInvite>> FetchInvites(IGuild guild)
        {
            List<TrackedInvite> invites = new();
            try
            {
                var i = await guild.GetInvitesAsync();
                invites.AddRange(i.Select(x => new TrackedInvite(x, guild.Id)));
            }
            catch (HttpException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get invites from guild {guild.Id}.");
            }
            try
            {
                var vanityInvite = await guild.GetVanityInviteAsync();
                invites.Add(new TrackedInvite(guild.Id, vanityInvite.Code, vanityInvite.Uses.GetValueOrDefault()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch vanity url for guild {guild.Id}.");
            }
            return invites;
        }

        private async Task GuildAvailableHandler(SocketGuild guild)
        {
            InviteTracker.AddInvites(guild.Id, await FetchInvites(guild));
        }

        private async Task GuildCreatedHandler(SocketGuild guild)
        {
            _logger.LogInformation($"I joined guild '{guild.Name}' with ID: '{guild.Id}'");

            using var scope = _serviceProvider.CreateScope();

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(guild.Id);
                await _interactions.RegisterCommandsToGuildAsync(
                    guild.Id,
                    true
                );
            }
            catch (ResourceNotFoundException)
            {
                return;
            }
        }

        private async Task GuildMemberAddedHandler(SocketGuildUser member)
        {
            using var scope = _serviceProvider.CreateScope();

            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();

            await translator.SetContext(member.Guild.Id);

            // =========================================================================================================================================
            // Refresh identity memberships
            try
            {
                IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
                foreach (Identity identity in identityManager.GetCurrentIdentities())
                {
                    if (identity.GetCurrentUser().Id == member.Id)
                    {
                        identity.AddGuildMembership(member);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh identity memberships.");
            }

            // =========================================================================================================================================
            // Refresh member cache
            try
            {
                discordAPI.AddOrUpdateCache(CacheKey.GuildMember(member.Guild.Id, member.Id), new CacheApiResponse(member));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh member cache.");
            }

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(member.Guild.Id);
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

            // =========================================================================================================================================
            // Punishment handling
            try
            {
                Punishments handler = scope.ServiceProvider.GetRequiredService<Punishments>();
                await handler.HandleMemberJoin(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle punishment on member join.");
            }

            if (member.IsBot)
            {
                return;
            }

            // =========================================================================================================================================
            // Appeal handling
            try
            {
                await AppealRepository.CreateDefault(scope.ServiceProvider).SetAllAppealsAsInvalid(member.Guild.Id, member.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle appeal on member join.");
            }

            // =========================================================================================================================================
            // Check zalgo
            ZalgoRepository repo = ZalgoRepository.CreateWithBotIdentity(scope.ServiceProvider);
            try
            {
                ZalgoConfig zalgoConfig = await repo.GetZalgo(member.Guild.Id);
                await repo.CheckZalgoForMember(member.Guild.Id, zalgoConfig, member, true);
            }
            catch (ResourceNotFoundException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while checking zalgo for member.");
            }

            // =========================================================================================================================================
            // Invitetracking
            try
            {
                List<TrackedInvite> newInvites = await FetchInvites(member.Guild);

                TrackedInvite usedInvite = InviteTracker.GetUsedInvite(member.Guild.Id, newInvites);

                InviteTracker.AddInvites(member.Guild.Id, newInvites);

                if (usedInvite != null)
                {
                    UserInvite invite = new()
                    {
                        GuildId = member.Guild.Id,
                        JoinedUserId = member.Id,
                        JoinedAt = DateTime.UtcNow,
                        InviteIssuerId = usedInvite.CreatorId,
                        InviteCreatedAt = usedInvite.CreatedAt,
                        TargetChannelId = usedInvite.TargetChannelId,
                        UsedInvite = $"https://discord.gg/{usedInvite.Code}"
                    };

                    _logger.LogInformation($"User {member.Username} joined guild {member.Guild.Name} with ID: {member.Guild.Id} using invite {usedInvite.Code}");

                    if (guildConfig.ExecuteWhoisOnJoin && !string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
                    {
                        string message;
                        if (invite.InviteIssuerId != 0 && invite.InviteCreatedAt != null)
                        {
                            message = translator.T().NotificationAutoWhoisJoinWithAndFrom(member, invite.InviteIssuerId, invite.InviteCreatedAt.Value, member.CreatedAt.DateTime, invite.UsedInvite);
                        }
                        else
                        {
                            message = translator.T().NotificationAutoWhoisJoinWith(member, member.CreatedAt.DateTime, invite.UsedInvite);
                        }

                        await discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, message, AllowedMentions.None);
                    }

                    await InviteRepository.CreateDefault(scope.ServiceProvider).CreateInvite(invite);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get used invite.");
            }
        }

        private async Task GuildUpdatedHandler(SocketGuild oldG, SocketGuild newG)
        {
            using var scope = _serviceProvider.CreateScope();

            IInviteMetadata invite = null;

            try
            {
                invite = await newG.GetVanityInviteAsync();
            }
            catch (HttpException) { }

            if (invite != null)
            {
                InviteTracker.AddInvite(invite.Guild.Id, new TrackedInvite(invite.Guild.Id, invite.Code, invite.Uses.GetValueOrDefault()));
            }

            // Refresh role cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.AddOrUpdateCache(CacheKey.Guild(newG.Id), new CacheApiResponse(newG));
        }

        private Task InviteCreatedHandler(SocketInvite invite)
        {
            InviteTracker.AddInvite(invite.Guild.Id, new TrackedInvite(invite, invite.Guild.Id));

            return Task.CompletedTask;
        }

        private async Task CmdErroredHandler(IApplicationCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        {
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult eResult)
                {
                    if (eResult.Exception is BaseAPIException apiException)
                    {
                        _logger.LogError($"Command '{info.Name}' invoked by '{context.User.Username}' failed: {(eResult.Exception as BaseAPIException).Error}");

                        using var scope = _serviceProvider.CreateScope();
                        Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();
                        if (context.Guild != null && eResult.Exception is not UnregisteredGuildException)
                        {
                            await translator.SetContext(context.Guild.Id);
                        }

                        string errorCode = "#" + ((int)apiException.Error).ToString("D4");

                        EmbedBuilder builder = new EmbedBuilder()
                            .WithTitle(translator.T().SomethingWentWrong())
                            .WithColor(Color.Red)
                            .WithDescription(translator.T().Enum(apiException.Error))
                            .WithCurrentTimestamp()
                            .WithFooter($"{translator.T().Code()} {errorCode}");

                        try
                        {
                            await context.Interaction.RespondAsync(embed: builder.Build(), ephemeral: true);
                        }
                        catch (TimeoutException)
                        {
                            await context.Channel.SendMessageAsync(embed: builder.Build());
                        }
                        catch (InvalidOperationException)
                        {
                            await context.Interaction.ModifyOriginalResponseAsync(m =>
                            {
                                m.Content = "";
                                m.Embed = builder.Build();
                            });
                        }
                    }
                    else
                    {
                        _logger.LogError($"Command '{info.Name}' invoked by '{context.User.Username}' failed: " + eResult.Exception.Message + "\n" + eResult.Exception.StackTrace);
                    }
                }
                else
                    _logger.LogError($"Command '{info.Name}' invoked by '{context.User.Username}' failed due to {result.Error}.");
            }
        }
    }
}
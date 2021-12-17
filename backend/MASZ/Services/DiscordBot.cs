using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using MASZ.AutoModeration;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.GuildAuditLog;
using MASZ.InviteTracking;
using MASZ.Models;
using MASZ.Repositories;
using System.Net;
using System.Reflection;
using System.Text;

namespace MASZ.Services
{
    public class DiscordBot : BackgroundService
    {
        private readonly ILogger<DiscordBot> _logger;
        private readonly DiscordSocketClient _client;
        private readonly InternalConfiguration _internalConfiguration;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isRunning = false;
        private DateTime? _lastDisconnect = null;

        public DiscordBot(ILogger<DiscordBot> logger, DiscordSocketClient client, InternalConfiguration internalConfiguration, InteractionService interactions, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _client = client;
            _internalConfiguration = internalConfiguration;
            _interactions = interactions;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;

            RegisterEvents();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

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
            await _client.SetGameAsync(_internalConfiguration.GetBaseUrl(), type: ActivityType.Watching);
        }

        public void RegisterEvents()
        {
            _client.MessageReceived += MessageCreatedHandler;
            _client.MessageUpdated += MessageUpdatedHandler;
            _client.MessageDeleted += MessageDeletedHandler;
            _client.JoinedGuild += GuildCreatedHandler;
            _client.UserJoined += GuildMemberAddedHandler;
            _client.GuildMemberUpdated += GuildMemberUpdatedHandler;
            _client.UserLeft += GuildMemberRemoved;
            _client.InviteCreated += InviteCreatedHandler;
            _client.InviteDeleted += InviteDeletedHandler;
            _client.UserBanned += GuildBanAdded;
            _client.UserUnbanned += GuildBanRemoved;
            _client.GuildUpdated += GuildUpdatedHandler;
            _client.GuildAvailable += GuildAvailableHandler;
            _client.ThreadCreated += ThreadCreatedHandler;

            _client.Disconnected += Disconnected;
            _client.Ready += ReadyHandler;

            _client.InteractionCreated += HandleInteraction;

            _interactions.SlashCommandExecuted += CmdErroredHandler;

            var client_logger = _serviceProvider.GetRequiredService<ILogger<DiscordSocketClient>>();

            _client.Log += (logLevel) => Log(logLevel, client_logger);

            _client.JoinedGuild += JoinGuild;

            var interactions_logger = _serviceProvider.GetRequiredService<ILogger<InteractionService>>();

            _interactions.Log += (logLevel) => Log(logLevel, interactions_logger);
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);

                using var scope = _serviceScopeFactory.CreateScope();

                await _interactions.ExecuteCommandAsync(ctx, scope.ServiceProvider);
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to execute {arg.Type} in channel {arg.Channel}");

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

            foreach (var guild in _client.Guilds)
            {
                await JoinGuild(guild);
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

        private async Task JoinGuild(SocketGuild guild)
        {
            await _interactions.RegisterCommandsToGuildAsync(
                guild.Id,
                true
            );

            _logger.LogInformation($"Initialized guild commands for guild {guild.Name}.");
        }

        private async Task GuildBanRemoved(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, guild.Id);

            await audit_logger.HandleEvent(BanRemovedAuditLog.HandleBanRemoved(user, translator), GuildAuditLogEvent.BanRemoved);

            // Refresh ban cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.RemoveFromCache(CacheKey.GuildBan(guild.Id, user.Id));
        }

        private async Task GuildBanAdded(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, guild.Id);
            await audit_logger.HandleEvent(BanAddedAuditLog.HandleBanAdded(user, translator), GuildAuditLogEvent.BanAdded);

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
        }

        private async Task GuildMemberRemoved(SocketGuildUser usr)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(usr.Guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, usr.Guild.Id);
            await audit_logger.HandleEvent(MemberRemovedAuditLog.HandleMemberRemovedUpdated(usr, translator), GuildAuditLogEvent.MemberRemoved);

            // Refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == usr.Id)
                {
                    identity.RemoveGuildMembership(usr.Guild.Id);
                }
            }
        }

        private async Task GuildMemberUpdatedHandler(Cacheable<SocketGuildUser, ulong> oldUsrCached, SocketGuildUser newUsr)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, newUsr.Guild.Id);

            var oldUsr = await oldUsrCached.GetOrDownloadAsync();

            if (oldUsr.Nickname != newUsr.Nickname)
            {
                await audit_logger.HandleEvent(NicknameUpdatedAuditLog.HandleNicknameUpdated(), GuildAuditLogEvent.NicknameUpdated);
            }

            if (oldUsr.AvatarId != newUsr.AvatarId)
            {
                await audit_logger.HandleEvent(AvatarUpdatedAuditLog.HandleAvatarUpdated(), GuildAuditLogEvent.AvatarUpdated);
            }

            if (oldUsr.Roles.Select(r => r.Id).ToArray() != newUsr.Roles.Select(r => r.Id).ToArray())
            {
                await audit_logger.HandleEvent(MemberRolesUpdatedAuditLog.HandleMemberRolesUpdated(), GuildAuditLogEvent.MemberRolesUpdated);
            }

            // Refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == newUsr.Id)
                {
                    identity.UpdateGuildMembership(newUsr);
                }
            }

            // Refresh member cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.AddOrUpdateCache(CacheKey.GuildMember(newUsr.Id, newUsr.Id), new CacheApiResponse(newUsr));
        }

        private async Task MessageDeletedHandler(Cacheable<IMessage, ulong> msgC, Cacheable<IMessageChannel, ulong> channel)
        {
            var chnl = await channel.GetOrDownloadAsync();
            var msg = await msgC.GetOrDownloadAsync();

            if (chnl is ITextChannel txtChannel)
                if (txtChannel.Guild != null)
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                    await translator.SetContext(txtChannel.Guild.Id);

                    GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, txtChannel.Guild.Id);
                    await audit_logger.HandleEvent(MessageDeletedAuditLog.HandleMessageDeleted(msg, translator), GuildAuditLogEvent.MessageDeleted);
                }
        }

        private async Task ThreadCreatedHandler(SocketThreadChannel channel)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(channel.Guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
            await audit_logger.HandleEvent(ThreadCreatedAuditLog.HandleThreadCreated(channel, translator), GuildAuditLogEvent.ThreadCreated);

            await channel.JoinAsync();
        }

        private async Task MessageCreatedHandler(SocketMessage message)
        {
            if (message.Channel is ITextChannel channel)
            {

                using var scope = _serviceScopeFactory.CreateScope();

                if (channel.Guild != null)
                {
                    if (!message.Author.IsBot && !message.Author.IsWebhook)
                    {
                        var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                        await translator.SetContext(channel.Guild.Id);

                        GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
                        await audit_logger.HandleEvent(MessageSentAuditLog.HandleMessageSent(message, translator), GuildAuditLogEvent.MessageSent);
                    }
                }

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
                using var scope = _serviceScopeFactory.CreateScope();

                if (txtChannel.Guild != null)
                {
                    var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                    await translator.SetContext(txtChannel.Guild.Id);

                    GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, txtChannel.Guild.Id);
                    await audit_logger.HandleEvent(await MessageUpdatedAuditLog.HandleMessageUpdated(oldMsg, newMsg, translator), GuildAuditLogEvent.MessageUpdated);
                }

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
            catch (HttpException) { }
            return invites;
        }

        private async Task GuildAvailableHandler(SocketGuild guild)
        {
            InviteTracker.AddInvites(guild.Id, await FetchInvites(guild));
        }

        private Task GuildCreatedHandler(SocketGuild guild)
        {
            _logger.LogInformation($"I joined guild '{guild.Name}' with ID: '{guild.Id}'");
            return Task.CompletedTask;
        }

        private async Task GuildMemberAddedHandler(SocketGuildUser member)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(member.Guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, member.Guild.Id);
            await audit_logger.HandleEvent(MemberJoinedAuditLog.HandleMemberJoined(member, translator), GuildAuditLogEvent.MemberJoined);

            // Refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == member.Id)
                {
                    identity.AddGuildMembership(member);
                }
            }

            // Refresh member cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.AddOrUpdateCache(CacheKey.GuildMember(member.Guild.Id, member.Id), new CacheApiResponse(member));

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(member.Guild.Id);
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

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

            List<TrackedInvite> newInvites = await FetchInvites(member.Guild);
            TrackedInvite usedInvite = null;

            try
            {
                usedInvite = InviteTracker.GetUsedInvite(member.Guild.Id, newInvites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get used invite.");
            }

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

                _logger.LogInformation($"User {member.Username}#{member.Discriminator} joined guild {member.Guild.Name} with ID: {member.Guild.Id} using invite {usedInvite.Code}");

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

                    await discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, message);
                }

                await InviteRepository.CreateDefault(scope.ServiceProvider).CreateInvite(invite);
            }
        }

        private async Task GuildUpdatedHandler(SocketGuild oldG, SocketGuild newG)
        {
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
        }

        private async Task InviteCreatedHandler(SocketInvite invite)
        {
            InviteTracker.AddInvite(invite.Guild.Id, new TrackedInvite(invite, invite.Guild.Id));

            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(invite.Guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, invite.Guild.Id);
            await audit_logger.HandleEvent(InviteCreatedAuditLog.HandleInviteCreated(invite, translator), GuildAuditLogEvent.InviteCreated);
        }

        private async Task InviteDeletedHandler(SocketGuildChannel channel, string invite)
        {
            var invites = InviteTracker.RemoveInvite(channel.Guild.Id, invite);

            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(channel.Guild.Id);

            GuildAuditLogger audit_logger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
            await audit_logger.HandleEvent(await InviteDeletedAuditLog.HandleInviteDeleted(invites.First(), channel, translator), GuildAuditLogEvent.InviteDeleted);
        }

        private async Task CmdErroredHandler(SlashCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        {
            Console.WriteLine("ASDKJHKFASJ");
            if (!result.IsSuccess)
                if (result is ExecuteResult eResult)
                    if (eResult.Exception is BaseAPIException)
                    {
                        _logger.LogError($"Command '{info.Name}' invoked by '{context.User.Username}#{context.User.Discriminator}' failed: {(eResult.Exception as BaseAPIException).Error}");

                        using var scope = _serviceScopeFactory.CreateScope();
                        Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();
                        if (context.Guild != null)
                        {
                            await translator.SetContext(context.Guild.Id);
                        }

                        string errorCode = "0#" + ((int)(eResult.Exception as BaseAPIException).Error).ToString("D7");
                        StringBuilder sb = new();
                        sb.AppendLine(translator.T().SomethingWentWrong());
                        sb.AppendLine($"`{translator.T().Enum((eResult.Exception as BaseAPIException).Error)}`");
                        sb.Append($"**{translator.T().Code()}** ");
                        sb.Append($"`{errorCode}`");

                        try
                        {
                            await context.Interaction.RespondAsync(sb.ToString());
                        }
                        catch (HttpException ex)
                        {
                            if (ex.HttpCode == HttpStatusCode.NotFound)
                                await context.Channel.SendMessageAsync(sb.ToString());
                        }
                    }
                    else
                    {
                        _logger.LogError($"Command '{info.Name}' invoked by '{context.User.Username}#{context.User.Discriminator}' failed: " + eResult.Exception.Message + "\n" + eResult.Exception.StackTrace);
                    }
        }
    }
}
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using MASZ.AutoModerations;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.GuildAuditLog;
using MASZ.InviteTracking;
using MASZ.Logger;
using MASZ.Models;
using MASZ.Repositories;
using System.Net;
using System.Text;

namespace MASZ.Services
{
    public class DiscordBot : IDiscordBot
    {
        private readonly ILogger<DiscordBot> _logger;
        private readonly IInternalConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isRunning = false;
        private DateTime? _lastDisconnect = null;

        public DiscordBot(ILogger<DiscordBot> logger, DiscordSocketClient client, InteractionService interactions, IInternalConfiguration config, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _client = client;
            _config = config;
            _interactions = interactions;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomLoggerProvider());

            _client.MessageReceived += MessageCreatedHandler;
            _client.MessageUpdated += MessageUpdatedHandler;
            _client.MessageDeleted += MessageDeletedHandler;
            _client.JoinedGuild += GuildCreatedHandler;
            _client.UserJoined += GuildMemberAddedHandler;
            _client.GuildMemberUpdated += GuildMemberUpdatedHandler;
            _client.UserLeft += GuildMemberRemovedHandler;
            _client.InviteCreated += InviteCreatedHandler;
            _client.InviteDeleted += InviteDeletedHandler;
            _client.UserBanned += GuildBanAddedHandler;
            _client.UserUnbanned += GuildBanRemovedHandler;
            _client.GuildUpdated += GuildUpdatedHandler;
            _client.GuildAvailable += GuildAvailableHandler;
            _client.ThreadCreated += ThreadCreatedHandler;

            _client.Disconnected += Disconnected;
            _client.Ready += ReadyHandler;

            _interactions.SlashCommandExecuted += CmdErroredHandler;

            _client.Log += Log;

            _client.JoinedGuild += JoinGuild;

            _interactions.Log += Log;
        }

        private async Task Log(LogMessage logMessage)
        {
            ConsoleColor color = logMessage.Severity switch
            {
                LogSeverity.Info => ConsoleColor.Blue,
                LogSeverity.Debug => ConsoleColor.Gray,
                LogSeverity.Critical => ConsoleColor.DarkRed,
                LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Verbose => ConsoleColor.Magenta,
                LogSeverity.Warning => ConsoleColor.Yellow,
                _ => throw new NotImplementedException()
            };

            Console.ForegroundColor = color;

            if (logMessage.Exception == null)
                await Console.Out.WriteLineAsync(logMessage.Message);
            else
                await Console.Out.WriteLineAsync(logMessage.Exception.ToString());
        }

        private async Task JoinGuild(SocketGuild guild)
        {
            await _interactions.RegisterCommandsToGuildAsync(
                guild.Id,
                true
            );

            Console.WriteLine($"Initializing guild commands for guild {guild.Name}.");
        }

        private Task GuildBanRemovedHandler(SocketUser user, SocketGuild guild)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, guild.Id);

                await auditLogger.HandleEvent(BanRemovedAuditLog.HandleBanRemoved(user, translator), GuildAuditLogEvent.BanRemoved);

                // refresh ban cache
                IDiscordAPIInterface discordAPI = scope.ServiceProvider.GetService<IDiscordAPIInterface>();
                discordAPI.RemoveFromCache(CacheKey.GuildBan(guild.Id, user.Id));
            });
            return Task.CompletedTask;
        }

        private Task GuildBanAddedHandler(SocketUser user, SocketGuild guild)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, guild.Id);
                await auditLogger.HandleEvent(BanAddedAuditLog.HandleBanAdded(user, translator), GuildAuditLogEvent.BanAdded);

                // refresh ban cache
                IDiscordAPIInterface discordAPI = scope.ServiceProvider.GetService<IDiscordAPIInterface>();
                await discordAPI.GetGuildUserBan(guild.Id, user.Id, CacheBehavior.IgnoreCache);
                discordAPI.RemoveFromCache(CacheKey.GuildMember(guild.Id, user.Id));

                // refresh identity memberships
                IIdentityManager identityManager = scope.ServiceProvider.GetService<IIdentityManager>();
                foreach (Identity identity in identityManager.GetCurrentIdentities())
                {
                    if (identity.GetCurrentUser().Id == user.Id)
                    {
                        identity.RemoveGuildMembership(guild.Id);
                    }
                }
            });
            return Task.CompletedTask;
        }

        private Task GuildMemberRemovedHandler(SocketGuildUser usr)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(usr.Guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, usr.Guild.Id);
                await auditLogger.HandleEvent(MemberRemovedAuditLog.HandleMemberRemovedUpdated(usr, translator), GuildAuditLogEvent.MemberRemoved);

                // refresh identity memberships
                IIdentityManager identityManager = scope.ServiceProvider.GetService<IIdentityManager>();
                foreach (Identity identity in identityManager.GetCurrentIdentities())
                {
                    if (identity.GetCurrentUser().Id == usr.Id)
                    {
                        identity.RemoveGuildMembership(usr.Guild.Id);
                    }
                }
            });
            return Task.CompletedTask;
        }

        private Task GuildMemberUpdatedHandler(Cacheable<SocketGuildUser, ulong> oldUsrCached, SocketGuildUser newUsr)
        {
            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, newUsr.Guild.Id);

                var oldUsr = await oldUsrCached.GetOrDownloadAsync();

                if (oldUsr.Nickname != newUsr.Nickname)
                {
                    await auditLogger.HandleEvent(NicknameUpdatedAuditLog.HandleNicknameUpdated(), GuildAuditLogEvent.NicknameUpdated);
                }

                if (oldUsr.AvatarId != newUsr.AvatarId)
                {
                    await auditLogger.HandleEvent(AvatarUpdatedAuditLog.HandleAvatarUpdated(), GuildAuditLogEvent.AvatarUpdated);
                }

                // i dont know if we need to do this, but best be certain >w>
                if (oldUsr.Roles.Select(r => r.Id).ToArray() != newUsr.Roles.Select(r => r.Id).ToArray())
                {
                    await auditLogger.HandleEvent(MemberRolesUpdatedAuditLog.HandleMemberRolesUpdated(), GuildAuditLogEvent.MemberRolesUpdated);
                }

                // refresh identity memberships
                IIdentityManager identityManager = scope.ServiceProvider.GetService<IIdentityManager>();
                foreach (Identity identity in identityManager.GetCurrentIdentities())
                {
                    if (identity.GetCurrentUser().Id == newUsr.Id)
                    {
                        identity.UpdateGuildMembership(newUsr);
                    }
                }

                // refresh member cache
                IDiscordAPIInterface discordAPI = scope.ServiceProvider.GetService<IDiscordAPIInterface>();
                discordAPI.AddOrUpdateCache(CacheKey.GuildMember(newUsr.Id, newUsr.Id), new CacheApiResponse(newUsr));
            });
            return Task.CompletedTask;
        }

        private Task MessageDeletedHandler(Cacheable<IMessage, ulong> msgC, Cacheable<IMessageChannel, ulong> channel)
        {
            Task.Run(async () =>
            {
                var chnl = await channel.GetOrDownloadAsync();
                var msg = await msgC.GetOrDownloadAsync();

                if (chnl is ITextChannel txtChannel)
                    if (txtChannel.Guild != null)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();

                        var translator = scope.ServiceProvider.GetService<ITranslator>();
                        await translator.SetContext(txtChannel.Guild.Id);

                        GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, txtChannel.Guild.Id);
                        await auditLogger.HandleEvent(MessageDeletedAuditLog.HandleMessageDeleted(msg, translator), GuildAuditLogEvent.MessageDeleted);
                    }
            });

            return Task.CompletedTask;
        }

        // https://discord.com/developers/docs/topics/gateway#thread-create
        // Sent when a thread is created, relevant to the current user, or when the current user is added to a thread.
        private async Task ThreadCreatedHandler(SocketThreadChannel channel)
        {
            Task auditLogTask = new(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(channel.Guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
                await auditLogger.HandleEvent(ThreadCreatedAuditLog.HandleThreadCreated(channel, translator), GuildAuditLogEvent.ThreadCreated);
            });
            auditLogTask.Start();
            await channel.JoinAsync();
        }

        public async Task Start()
        {
            await _client.LoginAsync(TokenType.Bot, _config.GetBotToken());
            await _client.SetGameAsync(_config.GetBaseUrl(), type: ActivityType.Watching);
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

        private Task ReadyHandler()
        {
            _logger.LogInformation("Client connected.");
            _isRunning = true;

            Task.Run(async () =>
            {
                foreach (var guild in _client.Guilds)
                {
                    await JoinGuild(guild);
                }
            });

            return Task.CompletedTask;
        }

        private Task MessageCreatedHandler(SocketMessage message)
        {
            if (message.Channel is ITextChannel channel)
            {

                if (channel.Guild != null)
                {
                    if (!message.Author.IsBot && !message.Author.IsWebhook)
                    {
                        Task.Run(async () =>
                        {
                            using var scope = _serviceScopeFactory.CreateScope();

                            var translator = scope.ServiceProvider.GetService<ITranslator>();
                            await translator.SetContext(channel.Guild.Id);

                            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
                            await auditLogger.HandleEvent(MessageSentAuditLog.HandleMessageSent(message, translator), GuildAuditLogEvent.MessageSent);
                        });
                    }
                }

                if (message.Type != MessageType.Default && message.Type != MessageType.Reply)
                {
                    return Task.CompletedTask;
                }
                if (message.Author.IsBot)
                {
                    return Task.CompletedTask;
                }
                if (channel.Guild == null)
                {
                    return Task.CompletedTask;
                }

                Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();

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
                });
            }

            return Task.CompletedTask;
        }

        private Task MessageUpdatedHandler(Cacheable<IMessage, ulong> oldMsg, SocketMessage newMsg, ISocketMessageChannel channel)
        {
            if (channel is ITextChannel txtChannel)
            {
                if (txtChannel.Guild != null)
                {
                    Task.Run(async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();

                        var translator = scope.ServiceProvider.GetService<ITranslator>();
                        await translator.SetContext(txtChannel.Guild.Id);

                        GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, txtChannel.Guild.Id);
                        await auditLogger.HandleEvent(await MessageUpdatedAuditLog.HandleMessageUpdated(oldMsg, newMsg, translator), GuildAuditLogEvent.MessageUpdated);
                    });
                }

                if (newMsg.Type != MessageType.Default && newMsg.Type != MessageType.Reply)
                {
                    return Task.CompletedTask;
                }
                if (newMsg.Author.IsBot)
                {
                    return Task.CompletedTask;
                }
                if (txtChannel.Guild == null)
                {
                    return Task.CompletedTask;
                }

                Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
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
                });
            }
            return Task.CompletedTask;
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
            Task auditLogTask = new(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(member.Guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, member.Guild.Id);
                await auditLogger.HandleEvent(MemberJoinedAuditLog.HandleMemberJoined(member, translator), GuildAuditLogEvent.MemberJoined);

                // refresh identity memberships
                IIdentityManager identityManager = scope.ServiceProvider.GetService<IIdentityManager>();
                foreach (Identity identity in identityManager.GetCurrentIdentities())
                {
                    if (identity.GetCurrentUser().Id == member.Id)
                    {
                        identity.AddGuildMembership(member);
                    }
                }

                // refresh member cache
                IDiscordAPIInterface discordAPI = scope.ServiceProvider.GetService<IDiscordAPIInterface>();
                discordAPI.AddOrUpdateCache(CacheKey.GuildMember(member.Guild.Id, member.Id), new CacheApiResponse(member));
            });
            auditLogTask.Start();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
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
                    IPunishmentHandler handler = scope.ServiceProvider.GetService<IPunishmentHandler>();
                    await handler.HandleMemberJoin(member);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle punishment on member join.");
                }

                if (member.IsBot)  // bots dont join via invite link so invitetracking is useless
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
                        ITranslator translator = scope.ServiceProvider.GetService<ITranslator>();
                        translator.SetContext(guildConfig);
                        string message;
                        if (invite.InviteIssuerId != 0 && invite.InviteCreatedAt != null)
                        {
                            message = translator.T().NotificationAutoWhoisJoinWithAndFrom(member, invite.InviteIssuerId, invite.InviteCreatedAt.Value, member.CreatedAt.DateTime, invite.UsedInvite);
                        }
                        else
                        {
                            message = translator.T().NotificationAutoWhoisJoinWith(member, member.CreatedAt.DateTime, invite.UsedInvite);
                        }

                        IDiscordAPIInterface discordAPI = scope.ServiceProvider.GetService<IDiscordAPIInterface>();
                        await discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, null, message);
                    }

                    await InviteRepository.CreateDefault(scope.ServiceProvider).CreateInvite(invite);
                }
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

        private Task InviteCreatedHandler(SocketInvite invite)
        {
            InviteTracker.AddInvite(invite.Guild.Id, new TrackedInvite(invite, invite.Guild.Id));

            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(invite.Guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, invite.Guild.Id);
                await auditLogger.HandleEvent(InviteCreatedAuditLog.HandleInviteCreated(invite, translator), GuildAuditLogEvent.InviteCreated);
            });

            return Task.CompletedTask;
        }

        private Task InviteDeletedHandler(SocketGuildChannel channel, string invite)
        {
            var invites = InviteTracker.RemoveInvite(channel.Guild.Id, invite);

            Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var translator = scope.ServiceProvider.GetService<ITranslator>();
                await translator.SetContext(channel.Guild.Id);

                GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
                await auditLogger.HandleEvent(await InviteDeletedAuditLog.HandleInviteDeleted(invites.First(), channel, translator), GuildAuditLogEvent.InviteDeleted);
            });

            return Task.CompletedTask;
        }

        private async Task CmdErroredHandler(SlashCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        {
            if (!result.IsSuccess)
                if (result is ExecuteResult eResult)
                    if (eResult.Exception is BaseAPIException)
                    {
                        _logger.LogError($"Command '{info.Name}' invoked by '{context.User.Username}#{context.User.Discriminator}' failed: {(eResult.Exception as BaseAPIException).Error}");

                        using var scope = _serviceScopeFactory.CreateScope();
                        ITranslator translator = scope.ServiceProvider.GetService<ITranslator>();
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

        public DiscordSocketClient GetClient()
        {
            return _client;
        }

        public int GetPing()
        {
            return _client.Latency;
        }
    }
}
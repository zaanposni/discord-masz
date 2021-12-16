using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using MASZ.AutoModeration;
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
    public class DiscordBot
    {
        private readonly ILogger<DiscordBot> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isRunning = false;
        private DateTime? _lastDisconnect = null;

        public DiscordBot(ILogger<DiscordBot> logger, DiscordSocketClient client, InteractionService interactions, InternalConfiguration config, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
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

            _interactions.SlashCommandExecuted += CmdErroredHandler;

            _client.Log += Log;

            _client.JoinedGuild += JoinGuild;

            _interactions.Log += Log;
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

        private async Task ReadyHandler()
        {
            _logger.LogInformation("Client connected.");
            _isRunning = true;

            foreach (var guild in _client.Guilds)
            {
                await JoinGuild(guild);
            }
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

        private async Task GuildBanRemoved(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, guild.Id);

            await auditLogger.HandleEvent(BanRemovedAuditLog.HandleBanRemoved(user, translator), GuildAuditLogEvent.BanRemoved);

            // Refresh ban cache
            DiscordAPIInterface discordAPI = scope.ServiceProvider.GetRequiredService<DiscordAPIInterface>();
            discordAPI.RemoveFromCache(CacheKey.GuildBan(guild.Id, user.Id));
        }

        private async Task GuildBanAdded(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, guild.Id);
            await auditLogger.HandleEvent(BanAddedAuditLog.HandleBanAdded(user, translator), GuildAuditLogEvent.BanAdded);

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

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, usr.Guild.Id);
            await auditLogger.HandleEvent(MemberRemovedAuditLog.HandleMemberRemovedUpdated(usr, translator), GuildAuditLogEvent.MemberRemoved);

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

            if (oldUsr.Roles.Select(r => r.Id).ToArray() != newUsr.Roles.Select(r => r.Id).ToArray())
            {
                await auditLogger.HandleEvent(MemberRolesUpdatedAuditLog.HandleMemberRolesUpdated(), GuildAuditLogEvent.MemberRolesUpdated);
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

                    GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, txtChannel.Guild.Id);
                    await auditLogger.HandleEvent(MessageDeletedAuditLog.HandleMessageDeleted(msg, translator), GuildAuditLogEvent.MessageDeleted);
                }
        }

        private async Task ThreadCreatedHandler(SocketThreadChannel channel)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(channel.Guild.Id);

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
            await auditLogger.HandleEvent(ThreadCreatedAuditLog.HandleThreadCreated(channel, translator), GuildAuditLogEvent.ThreadCreated);

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

                        GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
                        await auditLogger.HandleEvent(MessageSentAuditLog.HandleMessageSent(message, translator), GuildAuditLogEvent.MessageSent);
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

                    GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, txtChannel.Guild.Id);
                    await auditLogger.HandleEvent(await MessageUpdatedAuditLog.HandleMessageUpdated(oldMsg, newMsg, translator), GuildAuditLogEvent.MessageUpdated);
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

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, member.Guild.Id);
            await auditLogger.HandleEvent(MemberJoinedAuditLog.HandleMemberJoined(member, translator), GuildAuditLogEvent.MemberJoined);

            // refresh identity memberships
            IdentityManager identityManager = scope.ServiceProvider.GetRequiredService<IdentityManager>();
            foreach (Identity identity in identityManager.GetCurrentIdentities())
            {
                if (identity.GetCurrentUser().Id == member.Id)
                {
                    identity.AddGuildMembership(member);
                }
            }

            // refresh member cache
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
                PunishmentHandler handler = scope.ServiceProvider.GetRequiredService<PunishmentHandler>();
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

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, invite.Guild.Id);
            await auditLogger.HandleEvent(InviteCreatedAuditLog.HandleInviteCreated(invite, translator), GuildAuditLogEvent.InviteCreated);
        }

        private async Task InviteDeletedHandler(SocketGuildChannel channel, string invite)
        {
            var invites = InviteTracker.RemoveInvite(channel.Guild.Id, invite);

            using var scope = _serviceScopeFactory.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(channel.Guild.Id);

            GuildAuditLogger auditLogger = GuildAuditLogger.CreateDefault(_client, scope.ServiceProvider, channel.Guild.Id);
            await auditLogger.HandleEvent(await InviteDeletedAuditLog.HandleInviteDeleted(invites.First(), channel, translator), GuildAuditLogEvent.InviteDeleted);
        }

        private async Task CmdErroredHandler(SlashCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        {
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
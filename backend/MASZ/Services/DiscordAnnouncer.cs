using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Utils;

namespace MASZ.Services
{
	public class DiscordAnnouncer : IEvent
    {
        private readonly ILogger<DiscordAnnouncer> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordAPIInterface _discordAPI;
        private readonly InternalEventHandler _eventHandler;
        private readonly IServiceProvider _serviceProvider;

        public DiscordAnnouncer(ILogger<DiscordAnnouncer> logger, InternalConfiguration config, DiscordAPIInterface discordAPI, InternalEventHandler eventHandler, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discordAPI;
            _eventHandler = eventHandler;
            _serviceProvider = serviceProvider;
        }

        public void RegisterEvents()
        {
            _eventHandler.OnGuildRegistered += async (a, b) => await AnnounceTipsInNewGuild(a, b);

            _eventHandler.OnAutoModerationEventRegistered += async (a, b, c, d, e) => await AnnounceAutomoderation(a, b, c, d, e);

            _eventHandler.OnModCaseCreated += async (a, b, c, d) => await AnnounceModCase(a, b, c, d, RestAction.Created);
            _eventHandler.OnModCaseUpdated += async (a, b, c, d) => await AnnounceModCase(a, b, c, d, RestAction.Updated);
            _eventHandler.OnModCaseDeleted += async (a, b, c, d) => await AnnounceModCase(a, b, c, d, RestAction.Deleted);
            _eventHandler.OnModCaseMarkedToBeDeleted += async (a, b, c, d) => await AnnounceModCase(a, b, c, d, RestAction.Deleted);

            _eventHandler.OnModCaseCommentCreated += async (a, b) => await AnnounceComment(a, b, RestAction.Created);
            _eventHandler.OnModCaseCommentUpdated += async (a, b) => await AnnounceComment(a, b, RestAction.Updated);
            _eventHandler.OnModCaseCommentDeleted += async (a, b) => await AnnounceComment(a, b, RestAction.Deleted);

            _eventHandler.OnFileUploaded += async (a, b, c) => await AnnounceFile(a, b, c, RestAction.Created);
            _eventHandler.OnFileDeleted += async (a, b, c) => await AnnounceFile(a, b, c, RestAction.Deleted);

            _eventHandler.OnUserNoteCreated += async (a, b) => await AnnounceUserNote(a, b, RestAction.Created);
            _eventHandler.OnUserNoteUpdated += async (a, b) => await AnnounceUserNote(a, b, RestAction.Updated);
            _eventHandler.OnUserNoteDeleted += async (a, b) => await AnnounceUserNote(a, b, RestAction.Deleted);

            _eventHandler.OnUserMapCreated += async (a, b) => await AnnounceUserMapping(a, b, RestAction.Created);
            _eventHandler.OnUserMapUpdated += async (a, b) => await AnnounceUserMapping(a, b, RestAction.Updated);
            _eventHandler.OnUserMapDeleted += async (a, b) => await AnnounceUserMapping(a, b, RestAction.Deleted);

            _eventHandler.OnAutoModerationConfigCreated += async (a, b) => await AnnounceAutomoderationConfig(a, b, RestAction.Created);
            _eventHandler.OnAutoModerationConfigUpdated += async (a, b) => await AnnounceAutomoderationConfig(a, b, RestAction.Updated);
            _eventHandler.OnAutoModerationConfigDeleted += async (a, b) => await AnnounceAutomoderationConfig(a, b, RestAction.Deleted);

            _eventHandler.OnGuildMotdCreated += async (a, b) => await AnnounceMotd(a, b, RestAction.Created);
            _eventHandler.OnGuildMotdUpdated += async (a, b) => await AnnounceMotd(a, b, RestAction.Updated);

            _eventHandler.OnGuildLevelAuditLogConfigCreated += async (a, b) => await AnnounceGuildAuditLog(a, b, RestAction.Created);
            _eventHandler.OnGuildLevelAuditLogConfigUpdated += async (a, b) => await AnnounceGuildAuditLog(a, b, RestAction.Updated);
            _eventHandler.OnGuildLevelAuditLogConfigDeleted += async (a, b) => await AnnounceGuildAuditLog(a, b, RestAction.Deleted);

            _eventHandler.OnAppealCreated += async (a, b) => await AnnounceNewAppeal(a, b);
            _eventHandler.OnAppealUpdated += async (a, b, c) => await AnnounceUpdatedAppeal(a, b, c);

            _eventHandler.OnZalgoNicknameRename += async (a, b, c, d) => await AnnounceZalgoRename(a, b, c, d);
        }

        private async Task AnnounceTipsInNewGuild(GuildConfig guildConfig, bool importExistingBans)
        {
            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal tips webhook to {guildConfig.ModInternalNotificationWebhook} for guild {guildConfig.GuildId}.");

                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    EmbedBuilder embed = await guildConfig.CreateTipsEmbedForNewGuilds(scope.ServiceProvider);

                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), null);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing tips to {guildConfig.ModInternalNotificationWebhook} for guild {guildConfig.GuildId}.");
                }
            }
        }

        private async Task AnnounceModCase(ModCase modCase, IUser actor, bool announcePublic, bool announceDm, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();

            IUser caseUser = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.Default);
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(modCase.GuildId);
            translator.SetContext(guildConfig);

            if (announceDm && action != RestAction.Deleted)
            {
                _logger.LogInformation($"Sending dm notification to {modCase.UserId} for case {modCase.GuildId}/{modCase.CaseId}");

                try
                {
                    IGuild guild = _discordAPI.FetchGuildInfo(modCase.GuildId, CacheBehavior.Default);
                    string message = string.Empty;
                    switch (modCase.PunishmentType)
                    {
                        case PunishmentType.Mute:
                            if (modCase.PunishedUntil.HasValue)
                            {
                                message = translator.T().NotificationModcaseDMMuteTemp(modCase, guild, _config.GetBaseUrl());
                            }
                            else
                            {
                                message = translator.T().NotificationModcaseDMMutePerm(guild, _config.GetBaseUrl());
                            }
                            break;
                        case PunishmentType.Kick:
                            message = translator.T().NotificationModcaseDMKick(guild, _config.GetBaseUrl());
                            break;
                        case PunishmentType.Ban:
                            if (modCase.PunishedUntil.HasValue)
                            {
                                message = translator.T().NotificationModcaseDMBanTemp(modCase, guild, _config.GetBaseUrl());
                            }
                            else
                            {
                                message = translator.T().NotificationModcaseDMBanPerm(guild, _config.GetBaseUrl());
                            }
                            break;
                        default:
                            message = translator.T().NotificationModcaseDMWarn(guild, _config.GetBaseUrl());
                            break;
                    }
                    await _discordAPI.SendDmMessage(modCase.UserId, message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing modcase {modCase.GuildId}/{modCase.CaseId} in DMs to {modCase.UserId}.");
                }

            }

            if (!string.IsNullOrEmpty(guildConfig.ModPublicNotificationWebhook) && announcePublic)
            {
                _logger.LogInformation($"Sending public webhook for modcase {modCase.GuildId}/{modCase.CaseId} to {guildConfig.ModPublicNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await modCase.CreateModcaseEmbed(action, actor, scope.ServiceProvider, caseUser, false);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModPublicNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing modcase {modCase.GuildId}/{modCase.CaseId} public to {guildConfig.ModPublicNotificationWebhook}.");
                }
            }

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for modcase {modCase.GuildId}/{modCase.CaseId} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await modCase.CreateModcaseEmbed(action, actor, scope.ServiceProvider, caseUser, true);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing modcase {modCase.GuildId}/{modCase.CaseId} internal to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceComment(ModCaseComment comment, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing comment {comment.Id} in case {comment.ModCase.GuildId}/{comment.ModCase.CaseId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(comment.ModCase.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for comment {comment.ModCase.GuildId}/{comment.ModCase.CaseId}/{comment.Id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    IUser user = await _discordAPI.FetchUserInfo(comment.UserId, CacheBehavior.Default);
                    EmbedBuilder embed = await comment.CreateCommentEmbed(action, actor, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing comment {comment.ModCase.GuildId}/{comment.ModCase.CaseId}/{comment.Id} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceNewAppeal(Appeal appeal, IUser actor)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing new appeal {appeal.Id} for user {appeal.UserId} in guild {appeal.GuildId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(appeal.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for new appeal {appeal.GuildId}/{appeal.UserId}/{appeal.Id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await appeal.CreateEmbedForNewAppeal(actor, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing new appeal {appeal.GuildId}/{appeal.UserId}/{appeal.Id} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceUpdatedAppeal(Appeal appeal, IUser actor, IUser user)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing new appeal {appeal.Id} for user {appeal.UserId} in guild {appeal.GuildId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(appeal.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for new appeal {appeal.GuildId}/{appeal.UserId}/{appeal.Id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await appeal.CreateEmbedForUpdatedAppeal(actor, user, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing new appeal {appeal.GuildId}/{appeal.UserId}/{appeal.Id} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceFile(UploadedFile file, ModCase modCase, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing file {modCase.GuildId}/{modCase.CaseId}/{file.Name}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(modCase.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for file {modCase.GuildId}/{modCase.CaseId}/{file.Name} to {guildConfig.ModInternalNotificationWebhook}.");
                try
                {
                    EmbedBuilder embed = await file.CreateFileEmbed(modCase, action, actor, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing file {modCase.GuildId}/{modCase.CaseId}/{file.Name} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceUserNote(UserNote userNote, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing usernote {userNote.GuildId}/{userNote.UserId} ({userNote.Id}).");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(userNote.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for usernote {userNote.GuildId}/{userNote.UserId} ({userNote.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    IUser user = await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.Default);
                    EmbedBuilder embed = await userNote.CreateUserNoteEmbed(action, actor, user, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing usernote {userNote.GuildId}/{userNote.UserId} ({userNote.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceUserMapping(UserMapping userMapping, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing usermap {userMapping.GuildId}/{userMapping.UserA}-{userMapping.UserB} ({userMapping.Id}).");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(userMapping.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for usermap {userMapping.GuildId}/{userMapping.UserA}-{userMapping.UserB} ({userMapping.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await userMapping.CreateUserMapEmbed(action, actor, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing usermap {userMapping.GuildId}/{userMapping.UserA}-{userMapping.UserB} ({userMapping.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceAutomoderation(AutoModerationEvent modEvent, AutoModerationConfig moderationConfig, GuildConfig guildConfig, ITextChannel channel, IUser author)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();

            translator.SetContext(guildConfig);
            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for automod event {modEvent.GuildId}/{modEvent.Id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await modEvent.CreateInternalAutomodEmbed(guildConfig, author, channel, scope.ServiceProvider, moderationConfig.PunishmentType);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing automod event {modEvent.GuildId}/{modEvent.Id} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }

            if (moderationConfig.SendDmNotification)
            {
                _logger.LogInformation($"Sending dm notification for autmod event {modEvent.GuildId}/{modEvent.Id} to {author.Id}.");

                try
                {
                    string reason = translator.T().Enum(modEvent.AutoModerationType);
                    string action = translator.T().Enum(modEvent.AutoModerationAction);
                    await _discordAPI.SendDmMessage(author.Id, translator.T().NotificationAutomoderationDM(author, channel, reason, action));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing automod event {modEvent.GuildId}/{modEvent.Id} in dm to {author.Id}.");
                }
            }

            if ((modEvent.AutoModerationAction == AutoModerationAction.ContentDeleted || modEvent.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated) && moderationConfig.ChannelNotificationBehavior != AutoModerationChannelNotificationBehavior.NoNotification)
            {
                _logger.LogInformation($"Sending channel notification to {modEvent.GuildId}/{modEvent.Id} {channel.GuildId}/{channel.Id}.");

                try
                {
                    string reason = translator.T().Enum(modEvent.AutoModerationType);
                    IMessage msg = await channel.SendMessageAsync(translator.T().NotificationAutomoderationChannel(author, reason));
                    if (moderationConfig.ChannelNotificationBehavior == AutoModerationChannelNotificationBehavior.SendNotificationAndDelete)
                    {
                        Task task = new(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            try
                            {
                                _logger.LogInformation($"Deleting channel automod event notification {channel.GuildId}/{channel.Id}/{msg.Id}.");
                                await msg.DeleteAsync();
                            }
                            catch (UnauthorizedException) { }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"Error while deleting message {channel.GuildId}/{channel.Id}/{msg.Id} for automod event {modEvent.GuildId}/{modEvent.Id}.");
                            }
                        });
                        task.Start();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing automod event {modEvent.GuildId}/{modEvent.Id} in channel {channel.Id}.");
                }
            }
        }

        private async Task AnnounceAutomoderationConfig(AutoModerationConfig config, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing automod config {config.GuildId}/{config.AutoModerationType} ({config.Id}).");


            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(config.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for config {config.GuildId}/{config.AutoModerationType} ({config.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await config.CreateAutomodConfigEmbed(actor, action, _serviceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing config  {config.GuildId}/{config.AutoModerationType} ({config.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceMotd(GuildMotd motd, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing motd {motd.GuildId} ({motd.Id}).");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(motd.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for motd {motd.GuildId} ({motd.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await motd.CreateMotdEmbed(actor, action, _serviceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing motd {motd.GuildId} ({motd.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceGuildAuditLog(GuildLevelAuditLogConfig config, IUser actor, RestAction action)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing guild auditlog {config.GuildId}/{config.GuildAuditLogEvent} ({config.Id}).");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(config.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for guild auditlog {config.GuildId}/{config.GuildAuditLogEvent} ({config.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await config.CreateGuildAuditLogEmbed(actor, action, scope.ServiceProvider);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing guild auditlog {config.GuildId}/{config.GuildAuditLogEvent} ({config.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceZalgoRename(ZalgoConfig config, ulong id, string oldName, string newName)
        {
            using var scope = _serviceProvider.CreateScope();

            _logger.LogInformation($"Announcing zalgo rename {config.GuildId}/{id}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(config.GuildId);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for zalgo rename {config.GuildId}/{id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    Translator translator = scope.ServiceProvider.GetRequiredService<Translator>();
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, content: translator.T(guildConfig).NotificationZalgo(id, oldName, newName));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Errow while announcing internal webhook for zalgo rename {config.GuildId}/{id} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }
    }
}
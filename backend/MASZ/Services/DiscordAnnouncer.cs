using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;

namespace MASZ.Services
{
    public class DiscordAnnouncer
    {
        private readonly ILogger<DiscordAnnouncer> _logger;
        private readonly InternalConfiguration _config;
        private readonly DiscordAPIInterface _discordAPI;
        private readonly InternalEventHandler _eventHandler;
        private readonly BackgroundRunner _backgroundRunner;

        public DiscordAnnouncer(ILogger<DiscordAnnouncer> logger, InternalConfiguration config, DiscordAPIInterface discordAPI, InternalEventHandler eventHandler, BackgroundRunner backgroundRunner)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discordAPI;
            _eventHandler = eventHandler;
            _backgroundRunner = backgroundRunner;
        }

        public void Init()
        {
            _eventHandler.OnGuildRegistered += (GuildConfig guildConfig)
                => { _backgroundRunner.QueueAction(AnnounceTipsInNewGuild, guildConfig); return Task.CompletedTask; };

            _eventHandler.OnModCaseCreated += (ModCase modCase, IUser actor, bool announcePublic, bool announceDm)
                => { _backgroundRunner.QueueAction(AnnounceModCase, modCase, RestAction.Created, actor, announcePublic, announceDm); return Task.CompletedTask; };
            _eventHandler.OnModCaseUpdated += (ModCase modCase, IUser actor, bool announcePublic, bool announceDm)
                => { _backgroundRunner.QueueAction(AnnounceModCase, modCase, RestAction.Edited, actor, announcePublic, announceDm); return Task.CompletedTask; };
            _eventHandler.OnModCaseDeleted += (ModCase modCase, IUser actor, bool announcePublic, bool announceDm)
                => { _backgroundRunner.QueueAction(AnnounceModCase, modCase, RestAction.Deleted, actor, announcePublic, announceDm); return Task.CompletedTask; };
            _eventHandler.OnModCaseMarkedToBeDeleted += (ModCase modCase, IUser actor, bool announcePublic, bool announceDm)
                => { _backgroundRunner.QueueAction(AnnounceModCase, modCase, RestAction.Deleted, actor, announcePublic, announceDm); return Task.CompletedTask; };

            _eventHandler.OnModCaseCommentCreated += (ModCaseComment comment, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceComment, comment, actor, RestAction.Created); return Task.CompletedTask; };
            _eventHandler.OnModCaseCommentUpdated += (ModCaseComment comment, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceComment, comment, actor, RestAction.Edited); return Task.CompletedTask; };
            _eventHandler.OnModCaseCommentDeleted += (ModCaseComment comment, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceComment, comment, actor, RestAction.Deleted); return Task.CompletedTask; };

            _eventHandler.OnFileUploaded += (UploadedFile file, ModCase modCase, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceFile, file.Name, modCase, actor, RestAction.Created); return Task.CompletedTask; };
            _eventHandler.OnFileDeleted += (UploadedFile file, ModCase modCase, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceFile, file.Name, modCase, actor, RestAction.Deleted); return Task.CompletedTask; };

            _eventHandler.OnUserNoteCreated += (UserNote note, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceUserNote, note, actor, RestAction.Created); return Task.CompletedTask; };
            _eventHandler.OnUserNoteUpdated += (UserNote note, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceUserNote, note, actor, RestAction.Edited); return Task.CompletedTask; };
            _eventHandler.OnUserNoteDeleted += (UserNote note, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceUserNote, note, actor, RestAction.Deleted); return Task.CompletedTask; };

            _eventHandler.OnUserMapCreated += (UserMapping map, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceUserMapping, map, actor, RestAction.Created); return Task.CompletedTask; };
            _eventHandler.OnUserMapUpdated += (UserMapping map, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceUserMapping, map, actor, RestAction.Edited); return Task.CompletedTask; };
            _eventHandler.OnUserMapDeleted += (UserMapping map, IUser actor)
                => { _backgroundRunner.QueueAction(AnnounceUserMapping, map, actor, RestAction.Deleted); return Task.CompletedTask; };

            _eventHandler.OnAutoModerationEventRegistered += (AutoModerationEvent moderationEvent, AutoModerationConfig moderationConfig, GuildConfig guildConfig, ITextChannel channel, IUser author)
                => { _backgroundRunner.QueueAction(AnnounceAutomoderation, moderationEvent, moderationConfig, guildConfig, channel, author); return Task.CompletedTask; };
        }

        private async Task AnnounceTipsInNewGuild(IServiceScope scope, GuildConfig guildConfig)
        {
            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal tips webhook to {guildConfig.ModInternalNotificationWebhook} for guild {guildConfig.GuildId}.");

                try
                {
                    var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();
                    EmbedBuilder embed = embedCreator.CreateTipsEmbedForNewGuilds(guildConfig);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), null);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing tips to {guildConfig.ModInternalNotificationWebhook} for guild {guildConfig.GuildId}.");
                }
            }
        }

        private async Task AnnounceModCase(IServiceScope scope, ModCase modCase, RestAction action, IUser actor, bool announcePublic, bool announceDm)
        {
            _logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();

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
                    EmbedBuilder embed = await embedCreator.CreateModcaseEmbed(modCase, action, actor, caseUser, false);
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
                    EmbedBuilder embed = await embedCreator.CreateModcaseEmbed(modCase, action, actor, caseUser, true);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing modcase {modCase.GuildId}/{modCase.CaseId} internal to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceComment(IServiceScope scope, ModCaseComment comment, IUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing comment {comment.Id} in case {comment.ModCase.GuildId}/{comment.ModCase.CaseId}.");

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(comment.ModCase.GuildId);
            translator.SetContext(guildConfig);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for comment {comment.ModCase.GuildId}/{comment.ModCase.CaseId}/{comment.Id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    IUser user = await _discordAPI.FetchUserInfo(comment.UserId, CacheBehavior.Default);
                    EmbedBuilder embed = await embedCreator.CreateCommentEmbed(comment, action, actor);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing comment {comment.ModCase.GuildId}/{comment.ModCase.CaseId}/{comment.Id} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceFile(IServiceScope scope, string filename, ModCase modCase, IUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing file {modCase.GuildId}/{modCase.CaseId}/{filename}.");

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(modCase.GuildId);
            translator.SetContext(guildConfig);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for file {modCase.GuildId}/{modCase.CaseId}/{filename} to {guildConfig.ModInternalNotificationWebhook}.");
                try
                {
                    EmbedBuilder embed = await embedCreator.CreateFileEmbed(filename, modCase, action, actor);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing file {modCase.GuildId}/{modCase.CaseId}/{filename} to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceUserNote(IServiceScope scope, UserNote userNote, IUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing usernote {userNote.GuildId}/{userNote.UserId} ({userNote.Id}).");

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(userNote.GuildId);
            translator.SetContext(guildConfig);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for usernote {userNote.GuildId}/{userNote.UserId} ({userNote.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    IUser user = await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.Default);
                    EmbedBuilder embed = await embedCreator.CreateUserNoteEmbed(userNote, action, actor, user);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing usernote{userNote.GuildId}/{userNote.UserId} ({userNote.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceUserMapping(IServiceScope scope, UserMapping userMapping, IUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing usermap {userMapping.GuildId}/{userMapping.UserA}-{userMapping.UserB} ({userMapping.Id}).");

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(scope.ServiceProvider).GetGuildConfig(userMapping.GuildId);
            translator.SetContext(guildConfig);

            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for usermap {userMapping.GuildId}/{userMapping.UserA}-{userMapping.UserB} ({userMapping.Id}) to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = await embedCreator.CreateUserMapEmbed(userMapping, action, actor);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while announcing usermap {userMapping.GuildId}/{userMapping.UserA}-{userMapping.UserB} ({userMapping.Id}) to {guildConfig.ModInternalNotificationWebhook}.");
                }
            }
        }

        private async Task AnnounceAutomoderation(IServiceScope scope, AutoModerationEvent modEvent, AutoModerationConfig moderationConfig, GuildConfig guildConfig, ITextChannel channel, IUser author)
        {
            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            var embedCreator = scope.ServiceProvider.GetRequiredService<NotificationEmbedCreator>();

            translator.SetContext(guildConfig);
            if (!string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook for automod event {modEvent.GuildId}/{modEvent.Id} to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    EmbedBuilder embed = embedCreator.CreateInternalAutomodEmbed(modEvent, guildConfig, author, channel, moderationConfig.PunishmentType);
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
    }
}
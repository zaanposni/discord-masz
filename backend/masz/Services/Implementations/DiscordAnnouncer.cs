using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Models;
using masz.Repositories;
using Microsoft.Extensions.Logging;
using masz.Enums;

namespace masz.Services
{
    public class DiscordAnnouncer : IDiscordAnnouncer
    {
        private readonly ILogger<DiscordAnnouncer> _logger;
        private readonly IInternalConfiguration _config;
        private readonly IDiscordAPIInterface _discordAPI;
        private readonly INotificationEmbedCreator _notificationEmbedCreator;
        private readonly ITranslator _translator;
        private readonly IServiceProvider _serviceProvider;

        public DiscordAnnouncer() { }

        public DiscordAnnouncer(ILogger<DiscordAnnouncer> logger, IInternalConfiguration config, IDiscordAPIInterface discordAPI, INotificationEmbedCreator notificationContentCreator, ITranslator translator, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = config;
            _discordAPI = discordAPI;
            _notificationEmbedCreator = notificationContentCreator;
            _translator = translator;
            _serviceProvider = serviceProvider;
        }

        public async Task AnnounceTipsInNewGuild(GuildConfig guildConfig)
        {
            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal tips webhook to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    DiscordEmbedBuilder embed = _notificationEmbedCreator.CreateTipsEmbedForNewGuilds(guildConfig);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), null);
                } catch (Exception e)
                {
                    _logger.LogError(e, "Error while announcing tips.");
                }

                _logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceModCase(ModCase modCase, RestAction action, DiscordUser actor, bool announcePublic, bool announceDm)
        {
            _logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            DiscordUser caseUser = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.Default);
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);

            if (announceDm && action != RestAction.Deleted)
            {
                _logger.LogInformation($"Sending dm notification");

                try
                {
                    DiscordGuild guild = await _discordAPI.FetchGuildInfo(modCase.GuildId, CacheBehavior.Default);
                    string message = string.Empty;
                    switch (modCase.PunishmentType) {
                        case (PunishmentType.Mute):
                            if (modCase.PunishedUntil.HasValue) {
                                message = _translator.T().NotificationModcaseDMMuteTemp(modCase, guild, _config.GetBaseUrl());
                            } else {
                                message = _translator.T().NotificationModcaseDMMutePerm(modCase, guild, _config.GetBaseUrl());
                            }
                            break;
                        case (PunishmentType.Kick):
                            message = _translator.T().NotificationModcaseDMKick(modCase, guild, _config.GetBaseUrl());
                            break;
                        case (PunishmentType.Ban):
                            if (modCase.PunishedUntil.HasValue) {
                                message = _translator.T().NotificationModcaseDMBanTemp(modCase, guild, _config.GetBaseUrl());
                            } else {
                                message = _translator.T().NotificationModcaseDMBanPerm(modCase, guild, _config.GetBaseUrl());
                            }
                            break;
                        default:
                            message = _translator.T().NotificationModcaseDMWarn(modCase, guild, _config.GetBaseUrl());
                            break;
                    }
                    await _discordAPI.SendDmMessage(modCase.UserId, message);
                }  catch (Exception e)
                {
                    _logger.LogError(e, "Error while announcing modcase.");
                }

                _logger.LogInformation($"Sent dm notification");
            }

            if (! string.IsNullOrEmpty(guildConfig.ModPublicNotificationWebhook) && announcePublic)
            {
                _logger.LogInformation($"Sending public webhook to {guildConfig.ModPublicNotificationWebhook}.");

                DiscordEmbedBuilder embed = await _notificationEmbedCreator.CreateModcaseEmbed(modCase, action, actor, caseUser, false);

                await _discordAPI.ExecuteWebhook(guildConfig.ModPublicNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                _logger.LogInformation("Sent public webhook.");
            }

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                DiscordEmbedBuilder embed = await _notificationEmbedCreator.CreateModcaseEmbed(modCase, action, actor, caseUser, true);

                await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                _logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceComment(ModCaseComment comment, DiscordUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing comment {comment.Id} in case {comment.ModCase.CaseId} in guild {comment.ModCase.GuildId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(comment.ModCase.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    DiscordUser discordUser = await _discordAPI.FetchUserInfo(comment.UserId, CacheBehavior.Default);
                    DiscordEmbedBuilder embed = await _notificationEmbedCreator.CreateCommentEmbed(comment, action, actor);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                } catch (Exception e)
                {
                    _logger.LogError(e, "Error while announcing comment.");
                }

                _logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceFile(string filename, ModCase modCase, DiscordUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing file {filename} in case {modCase.CaseId} in guild {modCase.GuildId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    DiscordEmbedBuilder embed = await _notificationEmbedCreator.CreateFileEmbed(filename, modCase, action, actor);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                } catch (Exception e)
                {
                    _logger.LogError(e, "Error while announcing file.");
                }

                _logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceUserNote(UserNote userNote, DiscordUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing usernote {userNote.Id} in guild {userNote.GuildId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(userNote.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    DiscordUser DiscordUser = await _discordAPI.FetchUserInfo(userNote.UserId, CacheBehavior.Default);
                    DiscordEmbedBuilder embed = await _notificationEmbedCreator.CreateUserNoteEmbed(userNote, action, actor, DiscordUser);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                } catch (Exception e)
                {
                    _logger.LogError(e, "Error while announcing usernote.");
                }

                _logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceUserMapping(UserMapping userMapping, DiscordUser actor, RestAction action)
        {
            _logger.LogInformation($"Announcing usermap {userMapping.Id} in guild {userMapping.GuildId}.");

            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(userMapping.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                _logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                try
                {
                    DiscordEmbedBuilder embed = await _notificationEmbedCreator.CreateUserMapEmbed(userMapping, action, actor);
                    await _discordAPI.ExecuteWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                } catch (Exception e)
                {
                    _logger.LogError(e, "Error while announcing usermap.");
                }

                _logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceAutomoderation(AutoModerationEvent modEvent, AutoModerationConfig moderationConfig, GuildConfig guildConfig, DiscordChannel channel)
        {
            // if webhook, announce internal

            // announce dm

            // announce channel if deleted
        }
    }
}
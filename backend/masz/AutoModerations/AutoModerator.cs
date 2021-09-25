using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.Logging;

namespace masz.AutoModerations
{
    public class AutoModerator
    {
        private readonly ILogger<AutoModerator> _logger;
        private readonly DiscordClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly IInternalConfiguration _config;
        private readonly IDiscordAnnouncer _announcer;

        private AutoModerator(DiscordClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _logger = (ILogger<AutoModerator>) _serviceProvider.GetService(typeof(ILogger<AutoModerator>));
            _config = (IInternalConfiguration) _serviceProvider.GetService(typeof(IInternalConfiguration));
            _announcer = (IDiscordAnnouncer) _serviceProvider.GetService(typeof(IDiscordAnnouncer));
        }

        public static AutoModerator CreateDefault(DiscordClient client, IServiceProvider serviceProvider) => new AutoModerator(client, serviceProvider);

        public async Task HandleAutomoderation(DiscordMessage message, bool onEdit = false)
        {
            if (message.MessageType != MessageType.Default && message.MessageType != MessageType.Reply)
            {
                return;
            }
            if (message.Author.IsBot)
            {
                return;
            }
            if (message.Channel.Guild == null)
            {
                return;
            }

            GuildConfig guildConfig = null;
            List<AutoModerationConfig> autoModerationConfigs = null;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(message.Channel.Guild.Id);
                autoModerationConfigs = await AutoModerationConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuild(message.Channel.Guild.Id);
            } catch (ResourceNotFoundException)
            {
                return;
            }

            // invites
            if (await CheckAutoMod(
                    autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.InvitePosted),
                    message,
                    guildConfig,
                    InviteChecker.Check
                )) return;

            // emotes
            if (await CheckAutoMod(
                    autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.TooManyEmotes),
                    message,
                    guildConfig,
                    EmoteCheck.Check
                )) return;

            // mentions
            if (await CheckAutoMod(
                    autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.TooManyMentions),
                    message,
                    guildConfig,
                    MentionCheck.Check
                )) return;

            // attachments
            if (await CheckAutoMod(
                    autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.TooManyAttachments),
                    message,
                    guildConfig,
                    AttachmentCheck.Check
                )) return;

            // attachments
            if (await CheckAutoMod(
                    autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.TooManyEmbeds),
                    message,
                    guildConfig,
                    EmbedCheck.Check
                )) return;

            // too many automods

            // custom
            if (await CheckAutoMod(
                    autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.CustomWordFilter),
                    message,
                    guildConfig,
                    CustomWordCheck.Check
                )) return;

            // spam check
            if (! onEdit)
            {
                if (await CheckAutoMod(
                        autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.TooManyMessages),
                        message,
                        guildConfig,
                        SpamCheck.Check
                    )) return;
            }
        }

        private async Task<bool> CheckAutoMod(AutoModerationConfig? autoModerationConfig, DiscordMessage message, GuildConfig guildConfig, Func<DiscordMessage, AutoModerationConfig, bool> predicate)
        {
            if (autoModerationConfig != null)
            {
                if (predicate(message, autoModerationConfig))
                {
                    if (! await IsProtectedByFilter(message, guildConfig, autoModerationConfig))
                    {
                        _logger.LogInformation($"U: {message.Author.Id} | C: {message.Channel.Id} | G: {message.Channel.Guild.Id} triggered {autoModerationConfig.AutoModerationType.ToString()}.");
                        await ExecutePunishment(message, autoModerationConfig, guildConfig);
                        await CheckMultipleEvents(message, autoModerationConfig, guildConfig);
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> IsProtectedByFilter(DiscordMessage message, GuildConfig guildConfig, AutoModerationConfig autoModerationConfig)
        {
            if (_config.GetSiteAdmins().Contains(message.Author.Id))
            {
                return true;
            }

            DiscordGuild guild = await _client.GetGuildAsync(message.Channel.Guild.Id);
            DiscordMember member = await guild.GetMemberAsync(message.Author.Id);

            if (member == null)
            {
                return false;
            }

            if (member.Roles.Any(x => guildConfig.ModRoles.Contains(x.Id) ||
                                      guildConfig.AdminRoles.Contains(x.Id) ||
                                      autoModerationConfig.IgnoreRoles.Contains(x.Id)))
            {
                return true;
            }

            return autoModerationConfig.IgnoreChannels.Contains(message.Channel.Id);
        }

        private async Task CheckMultipleEvents(DiscordMessage message, AutoModerationConfig autoModerationConfig, GuildConfig guildConfig)
        {

        }

        private async Task ExecutePunishment(DiscordMessage message, AutoModerationConfig autoModerationConfig, GuildConfig guildConfig)
        {
            // internal notification
            // dm notification
            // register event
            // delete content if needed
            // notification in current channel if content deleted
        }

    }
}
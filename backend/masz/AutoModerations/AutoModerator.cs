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

            AutoModerationConfig inviteConfig = autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == AutoModerationType.InvitePosted);
            if (inviteConfig != null)
            {
                if (InviteChecker.Check(message))
                {
                    _logger.LogWarning("invite found!");
                    if (! await IsProtectedByFilter(message, guildConfig, inviteConfig))
                    {
                        _logger.LogWarning("invite not protected by filter!");
                    }
                }
            }
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

        private async Task CheckMultipleEvents(DiscordMessage message, GuildConfig guildConfig, AutoModerationConfig autoModerationConfig)
        {

        }

        private async Task ExecutePunishment(DiscordMessage message, AutoModerationConfig autoModerationConfig, GuildConfig guildConfig, AutoModerationType autoModerationType)
        {
            // internal notification
            // dm notification
            // register event
            // delete content if needed
            // notification in current channel if content deleted
        }

    }
}
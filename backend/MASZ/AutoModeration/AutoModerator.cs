using Discord;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Services;

namespace MASZ.AutoModeration
{
    public class AutoModerator
    {
        private readonly ILogger<AutoModerator> _logger;
        private readonly IDiscordClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly InternalConfiguration _config;
        private readonly GuildConfig _guildConfig;
        private readonly List<AutoModerationConfig> _autoModerationConfigs;

        private AutoModerator(IDiscordClient client, IServiceProvider serviceProvider, GuildConfig guildConfig, List<AutoModerationConfig> autoModerationConfigs)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<AutoModerator>>();
            _config = _serviceProvider.GetRequiredService<InternalConfiguration>();

            _guildConfig = guildConfig;
            _autoModerationConfigs = autoModerationConfigs;
        }

        public static async Task<AutoModerator> CreateDefault(IDiscordClient client, ulong guildId, IServiceProvider serviceProvider)
        {
            var guildConfig = await GuildConfigRepository.CreateDefault(serviceProvider).GetGuildConfig(guildId);
            var autoModerationConfigs = await AutoModerationConfigRepository.CreateWithBotIdentity(serviceProvider).GetConfigsByGuild(guildId);
            return new AutoModerator(client, serviceProvider, guildConfig, autoModerationConfigs);
        }

        public async Task HandleAutomoderation(IMessage message, bool onEdit = false)
        {
            if (message.Type != MessageType.Default && message.Type != MessageType.Reply)
            {
                return;
            }
            if (message.Author.IsBot)
            {
                return;
            }
            if ((message.Channel as ITextChannel).Guild == null)
            {
                return;
            }

            // spam check
            if (!onEdit)
            {
                if (await CheckAutoMod(
                       AutoModerationType.TooManyMessages,
                        message,
                        SpamCheck.Check
                    )) return;
            }

            // invites
            if (await CheckAutoMod(
                    AutoModerationType.InvitePosted,
                    message,
                    InviteChecker.Check
                )) return;

            // emotes
            if (await CheckAutoMod(
                    AutoModerationType.TooManyEmotes,
                    message,
                    EmoteCheck.Check
                )) return;

            // mentions
            if (await CheckAutoMod(
                    AutoModerationType.TooManyMentions,
                    message,
                    MentionCheck.Check
                )) return;

            // attachments
            if (await CheckAutoMod(
                    AutoModerationType.TooManyAttachments,
                    message,
                    AttachmentCheck.Check
                )) return;

            // attachments
            if (await CheckAutoMod(
                    AutoModerationType.TooManyEmbeds,
                    message,
                    EmbedCheck.Check
                )) return;

            // too many automods

            // custom
            if (await CheckAutoMod(
                    AutoModerationType.CustomWordFilter,
                    message,
                    CustomWordCheck.Check
                )) return;

            // duplicated chars
            if (await CheckAutoMod(
                    AutoModerationType.TooManyDuplicatedCharacters,
                    message,
                    DuplicatedCharacterCheck.Check
                )) return;

            // links
            if (await CheckAutoMod(
                    AutoModerationType.TooManyLinks,
                    message,
                    LinkCheck.Check
                )) return;
        }

        private async Task<bool> CheckAutoMod(AutoModerationType autoModerationType, IMessage message, Func<IMessage, AutoModerationConfig, IDiscordClient, Task<bool>> predicate)
        {
            AutoModerationConfig autoModerationConfig = _autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == autoModerationType);
            if (autoModerationConfig != null)
            {
                if (await predicate(message, autoModerationConfig, _client))
                {
                    if (!await IsProtectedByFilter(message, autoModerationConfig))
                    {
                        _logger.LogInformation($"U: {message.Author.Id} | C: {(message.Channel as ITextChannel).Id} | G: {(message.Channel as ITextChannel).Guild.Id} triggered {autoModerationConfig.AutoModerationType}.");
                        await ExecutePunishment(message, autoModerationConfig);
                        if (autoModerationConfig.AutoModerationType != AutoModerationType.TooManyAutoModerations)
                        {
                            await CheckAutoMod(AutoModerationType.TooManyAutoModerations, message, CheckMultipleEvents);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> CheckAutoMod(AutoModerationType autoModerationType, IMessage message, Func<IMessage, AutoModerationConfig, IDiscordClient, bool> predicate)
        {
            AutoModerationConfig autoModerationConfig = _autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == autoModerationType);
            if (autoModerationConfig != null)
            {
                if (predicate(message, autoModerationConfig, _client))
                {
                    if (!await IsProtectedByFilter(message, autoModerationConfig))
                    {
                        _logger.LogInformation($"U: {message.Author.Id} | C: {(message.Channel as ITextChannel).Id} | G: {(message.Channel as ITextChannel).Guild.Id} triggered {autoModerationConfig.AutoModerationType}.");
                        await ExecutePunishment(message, autoModerationConfig);
                        if (autoModerationConfig.AutoModerationType != AutoModerationType.TooManyAutoModerations)
                        {
                            await CheckAutoMod(AutoModerationType.TooManyAutoModerations, message, CheckMultipleEvents);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> CheckAutoMod(AutoModerationType autoModerationType, IMessage message, Func<IMessage, AutoModerationConfig, Task<bool>> predicate)
        {
            AutoModerationConfig autoModerationConfig = _autoModerationConfigs.FirstOrDefault(x => x.AutoModerationType == autoModerationType);
            if (autoModerationConfig != null)
            {
                if (await predicate(message, autoModerationConfig))
                {
                    if (!await IsProtectedByFilter(message, autoModerationConfig))
                    {
                        _logger.LogInformation($"U: {message.Author.Id} | C: {(message.Channel as ITextChannel).Id} | G: {(message.Channel as ITextChannel).Guild.Id} triggered {autoModerationConfig.AutoModerationType}.");
                        await ExecutePunishment(message, autoModerationConfig);
                        if (autoModerationConfig.AutoModerationType != AutoModerationType.TooManyAutoModerations)
                        {
                            await CheckAutoMod(AutoModerationType.TooManyAutoModerations, message, CheckMultipleEvents);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<bool> IsProtectedByFilter(IMessage message, AutoModerationConfig autoModerationConfig)
        {
            if (_config.GetSiteAdmins().Contains(message.Author.Id))
            {
                return true;
            }

            IGuild guild = await _client.GetGuildAsync((message.Channel as ITextChannel).Guild.Id);
            IGuildUser member = await guild.GetUserAsync(message.Author.Id);

            if (member == null)
            {
                return false;
            }

            if (member.Guild.OwnerId == member.Id)
            {
                return true;
            }

            if (member.RoleIds.Any(x => _guildConfig.ModRoles.Contains(x) ||
                                      _guildConfig.AdminRoles.Contains(x) ||
                                      autoModerationConfig.IgnoreRoles.Contains(x)))
            {
                return true;
            }

            return autoModerationConfig.IgnoreChannels.Contains((message.Channel as ITextChannel).Id);
        }

        private async Task<bool> CheckMultipleEvents(IMessage message, AutoModerationConfig config)
        {
            if (config.Limit == null)
            {
                return false;
            }
            if (config.TimeLimitMinutes == null)
            {
                return false;
            }
            var existing = await AutoModerationEventRepository.CreateDefault(_serviceProvider).GetAllEventsForUserSinceMinutes(message.Author.Id, config.TimeLimitMinutes.Value);
            return existing.Count > config.Limit.Value;
        }

        private async Task ExecutePunishment(IMessage message, AutoModerationConfig autoModerationConfig)
        {
            AutoModerationEvent modEvent = new()
            {
                GuildId = (message.Channel as ITextChannel).Guild.Id,
                AutoModerationType = autoModerationConfig.AutoModerationType,
                AutoModerationAction = autoModerationConfig.AutoModerationAction,
                UserId = message.Author.Id,
                MessageId = message.Id,
                MessageContent = message.Content
            };

            await AutoModerationEventRepository.CreateDefault(_serviceProvider).RegisterEvent(modEvent, message.Channel as ITextChannel, message.Author);

            if (modEvent.AutoModerationAction == AutoModerationAction.ContentDeleted || modEvent.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated)
            {
                try
                {
                    RequestOptions requestOptions = new();
                    requestOptions.RetryMode = RetryMode.RetryRatelimit;
                    await message.DeleteAsync(requestOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting message {message.Id}.");
                }
            }
        }
    }
}
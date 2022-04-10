using Discord;
using Discord.Net;
using Discord.WebSocket;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.InviteTracking;
using MASZ.Models;
using MASZ.Repositories;
using MASZ.Utils;
using System.Text;

namespace MASZ.Services
{
    public class GuildAuditLogger : IEvent
    {
        private readonly static string CHECK = "\u2705";
        private readonly static string X_CHECK = "\u274C";
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;

        public GuildAuditLogger(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public void RegisterEvents()
        {
            _client.UserBanned += HandleBanAdded;
            _client.UserUnbanned += HandleBanRemoved;
            _client.InviteCreated += HandleInviteCreated;
            _client.UserJoined += HandleUserJoined;
            _client.UserLeft += HandleUserRemoved;
            _client.MessageReceived += HandleMessageSent;
            _client.MessageDeleted += HandleMessageDeleted;
            _client.MessageUpdated += HandleMessageUpdated;
            _client.ThreadCreated += HandleThreadCreated;
            _client.InviteDeleted += HandleInviteDeleted;
            _client.UserUpdated += HandleUsernameUpdated;
            _client.GuildMemberUpdated += HandleGUserUpdated;
            _client.UserVoiceStateUpdated += HandleVoiceStateUpdated;
            _client.ReactionAdded += HandleReactionAdded;
            _client.ReactionRemoved += HandleReactionRemoved;
        }

        public async Task SendEmbed(EmbedBuilder embed, ulong guildId, GuildAuditLogEvent eventType)
        {
            using var scope = _serviceProvider.CreateScope();

            var guildConfigRepository = GuildConfigRepository.CreateDefault(scope.ServiceProvider);

            embed
                .WithColor(eventType switch
                {
                    GuildAuditLogEvent.MessageSent => Color.Green,
                    GuildAuditLogEvent.MessageUpdated => Color.Orange,
                    GuildAuditLogEvent.MessageDeleted => Color.Red,
                    GuildAuditLogEvent.UsernameUpdated => Color.Orange,
                    GuildAuditLogEvent.AvatarUpdated => Color.Orange,
                    GuildAuditLogEvent.NicknameUpdated => Color.Orange,
                    GuildAuditLogEvent.MemberRolesUpdated => Color.Orange,
                    GuildAuditLogEvent.MemberJoined => Color.Green,
                    GuildAuditLogEvent.MemberRemoved => Color.Red,
                    GuildAuditLogEvent.BanAdded => Color.Red,
                    GuildAuditLogEvent.BanRemoved => Color.Green,
                    GuildAuditLogEvent.InviteCreated => Color.Green,
                    GuildAuditLogEvent.InviteDeleted => Color.Red,
                    GuildAuditLogEvent.ThreadCreated => Color.Green,
                    GuildAuditLogEvent.VoiceJoined => Color.Green,
                    GuildAuditLogEvent.VoiceLeft => Color.Red,
                    GuildAuditLogEvent.VoiceMoved => Color.Orange,
                    GuildAuditLogEvent.ReactionAdded => Color.Green,
                    GuildAuditLogEvent.ReactionRemoved => Color.Red,
                    _ => throw new NotImplementedException(),
                })

                .WithCurrentTimestamp();

            try
            {
                GuildConfig guildConfig = await guildConfigRepository.GetGuildConfig(guildId);
                if (guildConfig == null)
                {
                    return;
                }
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

            var auditLogRepository = GuildLevelAuditLogConfigRepository.CreateWithBotIdentity(scope.ServiceProvider);
            GuildLevelAuditLogConfig auditLogConfig = null;
            try
            {
                auditLogConfig = await auditLogRepository.GetConfigsByGuildAndType(guildId, eventType);
                if (auditLogConfig == null)
                {
                    return;
                }
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

            if (embed.Footer == null)
            {
                embed.WithFooter(auditLogConfig.GuildAuditLogEvent.ToString());
            }
            else
            {
                embed.WithFooter(embed.Footer.Text + $" | {auditLogConfig.GuildAuditLogEvent}");
            }

            StringBuilder rolePings = new();
            foreach (ulong role in auditLogConfig.PingRoles)
            {
                rolePings.Append($"<@&{role}> ");
            }

            ITextChannel channel;

            try
            {
                channel = await _client.GetChannelAsync(auditLogConfig.ChannelId) as ITextChannel;
            }
            catch (Exception)
            {
                return;
            }

            await channel.SendMessageAsync(rolePings.ToString(), embed: embed.Build());
        }

        public async Task<bool> CheckForIgnoredRoles(ulong guildId, GuildAuditLogEvent eventType, List<ulong> roles)
        {
            using var scope = _serviceProvider.CreateScope();

            var auditLogRepository = GuildLevelAuditLogConfigRepository.CreateWithBotIdentity(scope.ServiceProvider);
            GuildLevelAuditLogConfig auditLogConfig = null;
            try
            {
                auditLogConfig = await auditLogRepository.GetConfigsByGuildAndType(guildId, eventType);
                if (auditLogConfig == null)
                {
                    return false;
                }
                return auditLogConfig.IgnoreRoles?.Any(role => roles.Contains(role)) ?? false;
            }
            catch (ResourceNotFoundException) { }
            return false;
        }

        public async Task<bool> CheckForIgnoredRoles(ulong guildId, GuildAuditLogEvent eventType, IReadOnlyCollection<SocketRole> roles)
        {
            return await CheckForIgnoredRoles(guildId, eventType, roles.Select(role => role.Id).ToList());
        }

        public async Task<bool> CheckForIgnoredChannel(ulong guildId, GuildAuditLogEvent eventType, ulong channelId)
        {
            using var scope = _serviceProvider.CreateScope();

            var auditLogRepository = GuildLevelAuditLogConfigRepository.CreateWithBotIdentity(scope.ServiceProvider);
            GuildLevelAuditLogConfig auditLogConfig = null;
            try
            {
                auditLogConfig = await auditLogRepository.GetConfigsByGuildAndType(guildId, eventType);
                if (auditLogConfig == null)
                {
                    return false;
                }
                return auditLogConfig.IgnoreChannels?.Contains(channelId) ?? false;
            }
            catch (ResourceNotFoundException) { }
            return false;
        }

        public async Task HandleGUserUpdated(Cacheable<SocketGuildUser, ulong> oldU, SocketGuildUser newU)
        {
            if (await CheckForIgnoredRoles(newU.Guild.Id, GuildAuditLogEvent.MemberRolesUpdated, newU.Roles))
            {
                return;
            }
            if (oldU.HasValue)
            {
                if (oldU.Value.Nickname != newU.Nickname)
                    await HandleNicknameUpdated(oldU.Value, newU, newU.Guild.Id);
                else if (oldU.Value.AvatarId != newU.AvatarId)
                    await HandleAvatarUpdated(oldU.Value, newU, newU.Guild.Id);
                else if (oldU.Value.Roles != newU.Roles)
                    await HandleMemberRolesUpdated(newU, oldU.Value.Roles, newU.Roles, newU.Guild.Id);
            }
        }

        public async Task HandleAvatarUpdated(IGuildUser oldU, IGuildUser newU, ulong guildId)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildId);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {newU.Username}#{newU.Discriminator} - {newU.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{newU.Id}`");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogAvatarUpdatedTitle())
                .WithDescription(description.ToString())
                .WithImageUrl(newU.GetAvatarOrDefaultUrl());

            embed.AddField(
                translator.T().GuildAuditLogOld(),
                oldU.GetAvatarOrDefaultUrl(),
                true
            );
            embed.AddField(
                translator.T().GuildAuditLogNew(),
                newU.GetAvatarOrDefaultUrl(),
                true
            );

            await SendEmbed(embed, guildId, GuildAuditLogEvent.AvatarUpdated);
        }

        public async Task HandleNicknameUpdated(IGuildUser oldU, IGuildUser newU, ulong guildId)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildId);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {newU.Username}#{newU.Discriminator} - {newU.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{newU.Id}`");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogNicknameUpdatedTitle())
                .WithDescription(description.ToString());

            embed.AddField(
                translator.T().GuildAuditLogOld(),
                string.IsNullOrEmpty(oldU.Nickname) ? $"`{translator.T().GuildAuditLogEmpty()}`" : oldU.Nickname,
                true
            );
            embed.AddField(
                translator.T().GuildAuditLogNew(),
                string.IsNullOrEmpty(newU.Nickname) ? $"`{translator.T().GuildAuditLogEmpty()}`" : newU.Nickname,
                true
            );

            await SendEmbed(embed, guildId, GuildAuditLogEvent.NicknameUpdated);
        }

        public async Task HandleMemberRolesUpdated(IGuildUser user, IReadOnlyCollection<SocketRole> roleOld, IReadOnlyCollection<SocketRole> roleNew, ulong guildId)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildId);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{user.Id}`");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogRolesUpdatedTitle())
                .WithDescription(description.ToString());

            List<SocketRole> addedRoles = roleNew.Except(roleOld).ToList();
            List<SocketRole> removedRoles = roleOld.Except(roleNew).ToList();

            if (removedRoles.Count > 0)
            {
                embed.AddField(
                    translator.T().GuildAuditLogRolesUpdatedRemoved(),
                    string.Join(" ", removedRoles.Select(x => x.Mention)),
                    true
                );
            }
            if (addedRoles.Count > 0)
            {
                embed.AddField(
                    translator.T().GuildAuditLogRolesUpdatedAdded(),
                    string.Join(" ", addedRoles.Select(x => x.Mention)),
                    true
                );
            }

            if (addedRoles.Count + removedRoles.Count > 0)
            {
                await SendEmbed(embed, guildId, GuildAuditLogEvent.MemberRolesUpdated);
            }
        }

        public async Task HandleUsernameUpdated(SocketUser oldU, SocketUser newU)
        {
            if (oldU.Username != newU.Username)
            {
                using var scope = _serviceProvider.CreateScope();
                var translator = scope.ServiceProvider.GetRequiredService<Translator>();

                foreach (var guild in newU.MutualGuilds)
                {
                    await translator.SetContext(guild.Id);

                    StringBuilder description = new();
                    description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {newU.Username}#{newU.Discriminator} - {newU.Mention}");
                    description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{newU.Id}`");

                    var embed = new EmbedBuilder()
                        .WithTitle(translator.T().GuildAuditLogUsernameUpdatedTitle())
                        .WithDescription(description.ToString());

                    embed.AddField(
                        translator.T().GuildAuditLogOld(),
                        oldU.Username,
                        true
                    );
                    embed.AddField(
                        translator.T().GuildAuditLogNew(),
                        newU.Username,
                        true
                    );

                    await SendEmbed(embed, guild.Id, GuildAuditLogEvent.UsernameUpdated);
                }
            }
        }

        private async Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            using var scope = _serviceProvider.CreateScope();

            IGuildChannel guildChannel;
            try
            {
                guildChannel = (IGuildChannel)await _client.GetChannelAsync(channel.Id);
                if (guildChannel == null)
                {
                    return;
                }
            }
            catch (HttpException)
            {
                return;
            }

            if (await CheckForIgnoredChannel(guildChannel.GuildId, GuildAuditLogEvent.ReactionRemoved, guildChannel.Id))
            {
                return;
            }

            if (!reaction.User.IsSpecified)
            {
                return;
            }

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildChannel.GuildId);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {reaction.User.Value.Username}#{reaction.User.Value.Discriminator} - {reaction.User.Value.Mention}");
            if (message.HasValue)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogMessage()}:** [{message.Id}]({message.Value.GetJumpUrl()})");
            }
            else
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogMessage()}:** {message.Id}");
            }
            description.AppendLine($"> **{translator.T().GuildAuditLogEmote()}:** {reaction.Emote}");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogReactionRemovedTitle())
                .WithDescription(description.ToString())
                .WithAuthor(reaction.User.Value)
                .WithFooter($"{translator.T().GuildAuditLogUserID()}: {reaction.User.Value.Id}");

            await SendEmbed(embed, guildChannel.GuildId, GuildAuditLogEvent.ReactionRemoved);
        }

        private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            using var scope = _serviceProvider.CreateScope();

            IGuildChannel guildChannel;
            try
            {
                guildChannel = (IGuildChannel)await _client.GetChannelAsync(channel.Id);
                if (guildChannel == null)
                {
                    return;
                }
            }
            catch (HttpException)
            {
                return;
            }

            if (await CheckForIgnoredChannel(guildChannel.GuildId, GuildAuditLogEvent.ReactionRemoved, guildChannel.Id))
            {
                return;
            }

            if (!reaction.User.IsSpecified)
            {
                return;
            }

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildChannel.GuildId);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {reaction.User.Value.Username}#{reaction.User.Value.Discriminator} - {reaction.User.Value.Mention}");
            if (message.HasValue)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogMessage()}:** [{message.Id}]({message.Value.GetJumpUrl()})");
            }
            else
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogMessage()}:** {message.Id}");
            }
            description.AppendLine($"> **{translator.T().GuildAuditLogEmote()}:** {reaction.Emote}");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogReactionAddedTitle())
                .WithDescription(description.ToString())
                .WithAuthor(reaction.User.Value)
                .WithFooter($"{translator.T().GuildAuditLogUserID()}: {reaction.User.Value.Id}");

            await SendEmbed(embed, guildChannel.GuildId, GuildAuditLogEvent.ReactionAdded);
        }

        private async Task HandleVoiceStateUpdated(SocketUser user, SocketVoiceState voiceStateBefore, SocketVoiceState voiceStateAfter)
        {
            using var scope = _serviceProvider.CreateScope();

            if (voiceStateBefore.VoiceChannel == voiceStateAfter.VoiceChannel)
            {
                return;
            }

            GuildAuditLogEvent eventType;
            IVoiceChannel beforeChannel = null;
            IVoiceChannel afterChannel = null;
            ulong guildId;
            if (voiceStateBefore.VoiceChannel == null)
            {
                eventType = GuildAuditLogEvent.VoiceJoined;
                afterChannel = voiceStateAfter.VoiceChannel;
                guildId = afterChannel.Guild.Id;
            }
            else if (voiceStateAfter.VoiceChannel == null)
            {
                eventType = GuildAuditLogEvent.VoiceLeft;
                beforeChannel = voiceStateBefore.VoiceChannel;
                guildId = beforeChannel.Guild.Id;
            }
            else
            {
                eventType = GuildAuditLogEvent.VoiceMoved;
                beforeChannel = voiceStateBefore.VoiceChannel;
                afterChannel = voiceStateAfter.VoiceChannel;
                if (beforeChannel.GuildId != afterChannel.GuildId)
                {
                    return;
                }
                guildId = beforeChannel.GuildId;
            }

            if (guildId == 0)
            {
                return;
            }

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildId);

            string title;
            if (eventType == GuildAuditLogEvent.VoiceJoined)
            {
                title = translator.T().GuildAuditLogVoiceJoinedTitle();
            }
            else if (eventType == GuildAuditLogEvent.VoiceLeft)
            {
                title = translator.T().GuildAuditLogVoiceLeftTitle();
            }
            else
            {
                title = translator.T().GuildAuditLogVoiceMovedTitle();
            }

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");

            if (eventType == GuildAuditLogEvent.VoiceJoined)
            {
                if (await CheckForIgnoredChannel(guildId, GuildAuditLogEvent.VoiceJoined, afterChannel.Id))
                {
                    return;
                }
                description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {afterChannel.Name} - {afterChannel.Mention}");
            }
            else if (eventType == GuildAuditLogEvent.VoiceLeft)
            {
                if (await CheckForIgnoredChannel(guildId, GuildAuditLogEvent.VoiceLeft, beforeChannel.Id))
                {
                    return;
                }
                description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {beforeChannel.Name} - {beforeChannel.Mention}");
            }
            else
            {
                if (await CheckForIgnoredChannel(guildId, GuildAuditLogEvent.VoiceMoved, beforeChannel.Id))
                {
                    return;
                }
                if (await CheckForIgnoredChannel(guildId, GuildAuditLogEvent.VoiceMoved, afterChannel.Id))
                {
                    return;
                }
                description.AppendLine($"> **{translator.T().GuildAuditLogChannelBefore()}:** {beforeChannel.Name} - {beforeChannel.Mention}");
                description.AppendLine($"> **{translator.T().GuildAuditLogChannelAfter()}:** {afterChannel.Name} - {afterChannel.Mention}");
            }

            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description.ToString())
                .WithAuthor(user)
                .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            await SendEmbed(embed, guildId, eventType);
        }

        public async Task HandleBanAdded(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogBanAddedTitle())
                .WithDescription(description.ToString())
                .WithAuthor(user)
                .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            await SendEmbed(embed, guild.Id, GuildAuditLogEvent.BanAdded);
        }

        public async Task HandleBanRemoved(SocketUser user, SocketGuild guild)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");

            var embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogBanRemovedTitle())
                .WithDescription(description.ToString())
                .WithAuthor(user)
                .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            await SendEmbed(embed, guild.Id, GuildAuditLogEvent.BanRemoved);
        }

        public async Task HandleInviteCreated(SocketInvite invite)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(invite.Guild.Id);

            EmbedBuilder embed = new();

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedURL()}:** {invite}");
            if (invite.Inviter != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {invite.Inviter.Username}#{invite.Inviter.Discriminator} - {invite.Inviter.Mention}");
                embed.WithAuthor(invite.Inviter)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {invite.Inviter.Id}");
            }

            if (invite.Channel is ITextChannel tChannel)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedTargetChannel()}:** {tChannel.Name} - {tChannel.Mention}");
            }

            if (invite.GuildId.HasValue && await CheckForIgnoredChannel(invite.GuildId.Value, GuildAuditLogEvent.InviteCreated, invite.ChannelId))
            {
                return;
            }

            embed.WithTitle(translator.T().GuildAuditLogInviteCreatedTitle())
                 .WithDescription(description.ToString());

            if (invite.MaxUses != 0)
            {
                embed.AddField(translator.T().GuildAuditLogInviteCreatedMaxUses(), $"`{invite.MaxUses}`", true);
            }

            if (invite.CreatedAt != default && invite.MaxAge != default)
            {
                embed.AddField(translator.T().GuildAuditLogInviteCreatedExpiration(), invite.CreatedAt.AddSeconds(invite.MaxAge).DateTime.ToDiscordTS(), true);
            }

            await SendEmbed(embed, invite.Guild.Id, GuildAuditLogEvent.InviteCreated);
        }

        public async Task HandleInviteDeleted(SocketGuildChannel channel, string tracker)
        {
            var invite = InviteTracker.RemoveInvite(channel.Guild.Id, tracker).FirstOrDefault();

            if (invite == null)
                return;

            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(channel.Guild.Id);

            EmbedBuilder embed = new();

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedURL()}:** {invite}");

            var inviter = channel.Guild.GetUser(invite.CreatorId);

            if (inviter != null)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {inviter.Username}#{inviter.Discriminator} - {inviter.Mention}");
                embed.WithAuthor(inviter)
                     .WithFooter($"{translator.T().GuildAuditLogUserID()}: {inviter.Id}");
            }

            if (channel is ITextChannel tChannel)
            {
                description.AppendLine($"> **{translator.T().GuildAuditLogInviteCreatedTargetChannel()}:** {tChannel.Name} - {tChannel.Mention}");
            }

            if (await CheckForIgnoredChannel(invite.GuildId, GuildAuditLogEvent.InviteCreated, channel.Id))
            {
                return;
            }

            embed.WithTitle(translator.T().GuildAuditLogInviteDeletedTitle())
                 .WithDescription(description.ToString());

            await SendEmbed(embed, channel.Guild.Id, GuildAuditLogEvent.InviteDeleted);
        }

        public async Task HandleUserJoined(SocketGuildUser user)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(user.Guild.Id);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{user.Id}`");
            description.AppendLine($"> **{translator.T().GuildAuditLogMemberJoinedRegistered()}:** {user.CreatedAt.DateTime.ToDiscordTS()}");

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(translator.T().GuildAuditLogMemberJoinedTitle())
                .WithDescription(description.ToString())
                .WithAuthor(user)
                .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            await SendEmbed(embed, user.Guild.Id, GuildAuditLogEvent.MemberJoined);
        }

        public async Task HandleUserRemoved(SocketGuild guild, SocketUser user)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guild.Id);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogUser()}:** {user.Username}#{user.Discriminator} - {user.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** `{user.Id}`");
            description.AppendLine($"> **{translator.T().GuildAuditLogMemberJoinedRegistered()}:** {user.CreatedAt.DateTime.ToDiscordTS()}");

            EmbedBuilder embed = new EmbedBuilder()
                 .WithTitle(translator.T().GuildAuditLogMemberRemovedTitle())
                 .WithDescription(description.ToString())
                 .WithAuthor(user)
                 .WithFooter($"{translator.T().GuildAuditLogUserID()}: {user.Id}");

            await SendEmbed(embed, guild.Id, GuildAuditLogEvent.MemberRemoved);
        }

        public async Task HandleMessageDeleted(Cacheable<IMessage, ulong> messageCached, Cacheable<IMessageChannel, ulong> channel)
        {
            var message = await messageCached.GetOrDownloadAsync();

            if (message == null)
            {
                return;
            }

            if (message.Channel is ITextChannel tchannel)
            {
                if (await CheckForIgnoredChannel(tchannel.GuildId, GuildAuditLogEvent.MessageDeleted, tchannel.Id))
                {
                    return;
                }

                if (message.Author is IGuildUser tauthor)
                {
                    if (await CheckForIgnoredRoles(tchannel.GuildId, GuildAuditLogEvent.MessageDeleted, tauthor.RoleIds.ToList()))
                    {
                        return;
                    }
                }

                using var scope = _serviceProvider.CreateScope();

                var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                await translator.SetContext(tchannel.Guild.Id);

                EmbedBuilder embed = new();

                StringBuilder description = new();
                description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {message.Channel.Name} - {tchannel.Mention}");
                description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{message.Id}]({message.GetJumpUrl()})");

                if (message.Author != null)
                {
                    description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {message.Author.Username}#{message.Author.Discriminator} - {message.Author.Mention}");
                    embed.WithAuthor(message.Author)
                         .WithFooter($"{translator.T().GuildAuditLogUserID()}: {message.Author.Id}");
                }

                if (message.CreatedAt != default)
                {
                    description.AppendLine($"> **{translator.T().GuildAuditLogCreated()}:** {message.CreatedAt.DateTime.ToDiscordTS()}");
                }

                embed.WithTitle(translator.T().GuildAuditLogMessageDeletedTitle())
                     .WithDescription(description.ToString());

                if (!string.IsNullOrEmpty(message.Content))
                {
                    if (message.Content.Length > 1024)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (message.Content.Length > i * 1024)
                            {
                                embed.AddField(
                                    $"{translator.T().GuildAuditLogMessageDeletedContent()} [{i + 1}]",
                                    new string(message.Content.Skip(i * 1024).Take(1024).ToArray())
                                );
                            }
                        }
                    }
                    else
                    {
                        embed.AddField(translator.T().GuildAuditLogMessageDeletedContent(), message.Content);
                    }
                }

                if (message.Attachments.Count > 0)
                {
                    StringBuilder attachmentInfo = new();
                    int counter = 1;
                    foreach (IAttachment attachment in message.Attachments.Take(5))
                    {
                        attachmentInfo.AppendLine($"- [{counter}. {translator.T().Attachment()}]({attachment.Url})");
                        counter++;
                    }
                    if (message.Attachments.Count > 5)
                    {
                        attachmentInfo.AppendLine(translator.T().AndXMore(message.Attachments.Count - 5));
                    }
                    embed.AddField(translator.T().Attachments(), attachmentInfo.ToString());
                }

                await SendEmbed(embed, tchannel.Guild.Id, GuildAuditLogEvent.MessageDeleted);
            }
        }

        public async Task HandleMessageSent(IMessage message)
        {
            if (!message.Author.IsBot && !message.Author.IsWebhook)
            {
                if (message.Channel is ITextChannel tchannel)
                {
                    if (await CheckForIgnoredChannel(tchannel.GuildId, GuildAuditLogEvent.MessageSent, tchannel.Id))
                    {
                        return;
                    }

                    if (message.Author is IGuildUser tauthor)
                    {
                        if (await CheckForIgnoredRoles(tchannel.GuildId, GuildAuditLogEvent.MessageSent, tauthor.RoleIds.ToList()))
                        {
                            return;
                        }
                    }

                    using var scope = _serviceProvider.CreateScope();

                    var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                    await translator.SetContext(tchannel.Guild.Id);

                    StringBuilder description = new();
                    description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {tchannel.Name} - {tchannel.Mention}");
                    description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{message.Id}]({message.GetJumpUrl()})");
                    description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {message.Author.Username}#{message.Author.Discriminator} - {message.Author.Mention}");

                    var embed = new EmbedBuilder()
                         .WithTitle(translator.T().GuildAuditLogMessageSentTitle())
                         .WithDescription(description.ToString())
                         .WithAuthor(message.Author)
                         .WithFooter($"{translator.T().GuildAuditLogUserID()}: {message.Author.Id}");

                    if (!string.IsNullOrEmpty(message.Content))
                    {
                        if (message.Content.Length > 1024)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (message.Content.Length > i * 1024)
                                {
                                    embed.AddField(
                                        $"{translator.T().GuildAuditLogMessageSentContent()} [{i + 1}]",
                                        new string(message.Content.Skip(i * 1024).Take(1024).ToArray())
                                    );
                                }
                            }
                        }
                        else
                        {
                            embed.AddField(translator.T().GuildAuditLogMessageSentContent(), message.Content);
                        }
                    }
                    if (message.Attachments.Count > 0)
                    {
                        StringBuilder attachmentInfo = new();
                        int counter = 1;
                        foreach (IAttachment attachment in message.Attachments.Take(5))
                        {
                            attachmentInfo.AppendLine($"- [{counter}. {translator.T().Attachment()}]({attachment.Url})");
                            counter++;
                        }
                        if (message.Attachments.Count > 5)
                        {
                            attachmentInfo.AppendLine(translator.T().AndXMore(message.Attachments.Count - 5));
                        }
                        embed.AddField(translator.T().Attachments(), attachmentInfo.ToString());
                    }

                    await SendEmbed(embed, tchannel.Guild.Id, GuildAuditLogEvent.MessageSent);
                }
            }
        }

        public async Task HandleMessageUpdated(Cacheable<IMessage, ulong> messageBefore, SocketMessage messageAfter, ISocketMessageChannel channel)
        {
            if (!messageAfter.Author.IsBot && !messageAfter.Author.IsWebhook)
            {
                if (channel is ITextChannel tchannel)
                {
                    if (await CheckForIgnoredChannel(tchannel.GuildId, GuildAuditLogEvent.MessageUpdated, tchannel.Id))
                    {
                        return;
                    }

                    if (messageAfter.Author is IGuildUser tauthor)
                    {
                        if (await CheckForIgnoredRoles(tchannel.GuildId, GuildAuditLogEvent.MessageUpdated, tauthor.RoleIds.ToList()))
                        {
                            return;
                        }
                    }

                    using var scope = _serviceProvider.CreateScope();

                    var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                    await translator.SetContext(tchannel.Guild.Id);

                    StringBuilder description = new();
                    description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {tchannel.Name} - {tchannel.Mention}");
                    description.AppendLine($"> **{translator.T().GuildAuditLogID()}:** [{messageAfter.Id}]({messageAfter.GetJumpUrl()})");
                    description.AppendLine($"> **{translator.T().GuildAuditLogAuthor()}:** {messageAfter.Author.Username}#{messageAfter.Author.Discriminator} - {messageAfter.Author.Mention}");
                    description.AppendLine($"> **{translator.T().GuildAuditLogCreated()}:** {messageAfter.CreatedAt.DateTime.ToDiscordTS()}");

                    var embed = new EmbedBuilder()
                         .WithTitle(translator.T().GuildAuditLogMessageUpdatedTitle())
                         .WithDescription(description.ToString())
                         .WithAuthor(messageAfter.Author)
                         .WithFooter($"{translator.T().GuildAuditLogUserID()}: {messageAfter.Author.Id}");

                    embed.AddField(translator.T().GuildAuditLogMessageUpdatedPinned(), messageAfter.IsPinned ? CHECK : X_CHECK, false);

                    var before = await messageBefore.GetOrDownloadAsync();

                    if (before == null)
                    {
                        embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentBefore(), translator.T().GuildAuditLogNotFoundInCache());
                    }
                    else
                    {
                        if (string.Equals(before.Content, messageAfter.Content) && before.Embeds.Count != messageAfter.Embeds.Count)
                        {
                            return;
                        }
                        if (!string.IsNullOrEmpty(before.Content))
                        {
                            embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentBefore(), before.Content.Truncate(1024));
                        }
                    }

                    if (!string.IsNullOrEmpty(messageAfter.Content))
                    {
                        if (messageAfter.Content.Length > 1024)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (messageAfter.Content.Length > i * 1024)
                                {
                                    embed.AddField(
                                        $"{translator.T().GuildAuditLogMessageUpdatedContentNew()} [{i + 1}]",
                                        new string(messageAfter.Content.Skip(i * 1024).Take(1024).ToArray())
                                    );
                                }
                            }
                        }
                        else
                        {
                            embed.AddField(translator.T().GuildAuditLogMessageUpdatedContentNew(), messageAfter.Content);
                        }
                    }

                    if (messageAfter.Attachments.Count > 0)
                    {
                        StringBuilder attachmentInfo = new();
                        int counter = 1;
                        foreach (IAttachment attachment in messageAfter.Attachments.Take(5))
                        {
                            attachmentInfo.AppendLine($"- [{counter}. {translator.T().Attachment()}]({attachment.Url})");
                            counter++;
                        }
                        if (messageAfter.Attachments.Count > 5)
                        {
                            attachmentInfo.AppendLine(translator.T().AndXMore(messageAfter.Attachments.Count - 5));
                        }
                        embed.AddField(translator.T().Attachments(), attachmentInfo.ToString());
                    }

                    await SendEmbed(embed, tchannel.Guild.Id, GuildAuditLogEvent.MessageUpdated);
                }
            }
        }

        public async Task HandleThreadCreated(SocketThreadChannel thread)
        {
            if (await CheckForIgnoredChannel(thread.Guild.Id, GuildAuditLogEvent.ThreadCreated, thread.ParentChannel.Id))
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(thread.Guild.Id);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {thread.Name} - {thread.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedParent()}:** {thread.ParentChannel.Name} - <#{thread.ParentChannel.Id}>");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedCreator()}:** {thread.Owner.Mention}");

            var embed = new EmbedBuilder()
                 .WithTitle(translator.T().GuildAuditLogThreadCreatedTitle())
                 .WithDescription(description.ToString())
                 .WithFooter($"{translator.T().GuildAuditLogChannelId()}: {thread.Id}");

            await SendEmbed(embed, thread.Guild.Id, GuildAuditLogEvent.ThreadCreated);
        }
    }
}

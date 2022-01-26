using Discord;
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
        }

        public async Task SendEmbed(EmbedBuilder embed, ulong guildID, GuildAuditLogEvent eventType) {
            var guildConfigRepository = GuildConfigRepository.CreateDefault(_serviceProvider);

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
                    _ => throw new NotImplementedException(),
                })

                .WithCurrentTimestamp();

            try
            {
                GuildConfig guildConfig = await guildConfigRepository.GetGuildConfig(guildID);
                if (guildConfig == null)
                {
                    return;
                }
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

            var auditLogRepository = GuildLevelAuditLogConfigRepository.CreateWithBotIdentity(_serviceProvider);
            GuildLevelAuditLogConfig auditLogConfig = null;
            try
            {
                auditLogConfig = await auditLogRepository.GetConfigsByGuildAndType(guildID, eventType);
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

        public async Task HandleGUserUpdated(Cacheable<SocketGuildUser, ulong> oldU, SocketGuildUser newU)
		{
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

        public async Task HandleAvatarUpdated(IGuildUser oldU, IGuildUser newU, ulong guildID)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildID);

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
                newU.GetAvatarOrDefaultUrl(),
                true
            );

            await SendEmbed(embed, guildID, GuildAuditLogEvent.AvatarUpdated);
        }

        public async Task HandleNicknameUpdated(IGuildUser oldU, IGuildUser newU, ulong guildID)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildID);

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

            await SendEmbed(embed, guildID, GuildAuditLogEvent.NicknameUpdated);
        }

        public async Task HandleMemberRolesUpdated(IGuildUser user, IReadOnlyCollection<SocketRole> roleOld, IReadOnlyCollection<SocketRole> roleNew, ulong guildID)
        {
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(guildID);

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
                await SendEmbed(embed, guildID, GuildAuditLogEvent.MemberRolesUpdated);
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

            EmbedBuilder embed = new ();

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
                return;

            if (message.Channel is ITextChannel tchannel)
            {
                using var scope = _serviceProvider.CreateScope();

                var translator = scope.ServiceProvider.GetRequiredService<Translator>();
                await translator.SetContext(tchannel.Guild.Id);

                EmbedBuilder embed = new ();

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
            using var scope = _serviceProvider.CreateScope();

            var translator = scope.ServiceProvider.GetRequiredService<Translator>();
            await translator.SetContext(thread.Guild.Id);

            StringBuilder description = new();
            description.AppendLine($"> **{translator.T().GuildAuditLogChannel()}:** {thread.Name} - {thread.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedParent()}:** {thread.ParentChannel.Name} - {thread.ParentChannel.Mention}");
            description.AppendLine($"> **{translator.T().GuildAuditLogThreadCreatedCreator()}:** <@{thread.Owner}>");

            var embed = new EmbedBuilder()
                 .WithTitle(translator.T().GuildAuditLogThreadCreatedTitle())
                 .WithDescription(description.ToString())
                 .WithFooter($"{translator.T().GuildAuditLogChannelId()}: {thread.Id}");

            await SendEmbed(embed, thread.Guild.Id, GuildAuditLogEvent.ThreadCreated);
        }
    }
}
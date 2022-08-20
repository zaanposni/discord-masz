using Discord;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using System.Text;

namespace MASZ.Services
{
	public static class EmbedCreator
    {
        private readonly static string CHECK = "\u2705";
        private readonly static string X_CHECK = "\u274C";
        private readonly static string SCALES_EMOTE = "\u2696";
        private readonly static string SCROLL_EMOTE = "\uD83C\uDFF7";
        private readonly static string ALARM_CLOCK = "\u23F0";
        private readonly static string STAR = "\u2B50";
        private readonly static string GLOBE = "\uD83C\uDF0D";
        private readonly static string CLOCK = "\uD83D\uDD50";
        private readonly static string HAND_SHAKE = "\uD83E\uDD1D";

        private static EmbedBuilder CreateBasicEmbed(RestAction action, IServiceProvider provider, IUser author = null)
        {
            EmbedBuilder embed = new()
            {
                Timestamp = DateTime.Now
            };

            switch (action)
            {
                case RestAction.Updated:
                    embed.Color = Color.Orange;
                    break;
                case RestAction.Deleted:
                    embed.Color = Color.Red;
                    break;
                case RestAction.Created:
                    embed.Color = Color.Green;
                    break;
            }

            if (author != null)
            {
                embed.WithAuthor(author);
            }

            var _config = provider.GetService<InternalConfiguration>();

            // Url
            if (!string.IsNullOrEmpty(_config.GetBaseUrl()))
            {
                embed.Url = _config.GetBaseUrl();
            }

            return embed;
        }

        public static async Task<EmbedBuilder> CreateModcaseEmbed(this ModCase modCase, RestAction action, IUser actor, IServiceProvider provider, IUser suspect = null, bool isInternal = true)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(modCase.GuildId);

            EmbedBuilder embed;
            if (isInternal)
            {
                embed = CreateBasicEmbed(action, provider, actor);
            }
            else
            {
                embed = CreateBasicEmbed(action, provider);
            }

            // Thumbnail
            if (suspect != null)
            {
                embed.WithThumbnailUrl(suspect.GetAvatarOrDefaultUrl());
            }

            // Description
            embed.AddField($"**{translator.T().Description()}**", modCase.Description.Truncate(1000));

            // Title
            embed.Title = $"#{modCase.CaseId} {modCase.Title}";

            // Footer
            embed.WithFooter($"UserId: {modCase.UserId} | CaseId: {modCase.CaseId}");

            // Description
            switch (action)
            {
                case RestAction.Updated:
                    if (isInternal)
                    {
                        embed.Description = translator.T().NotificationModcaseUpdateInternal(modCase, actor);
                    }
                    else
                    {
                        embed.Description = translator.T().NotificationModcaseUpdatePublic(modCase);
                    }
                    break;
                case RestAction.Deleted:
                    if (isInternal)
                    {
                        embed.Description = translator.T().NotificationModcaseDeleteInternal(modCase, actor);
                    }
                    else
                    {
                        embed.Description = translator.T().NotificationModcaseDeletePublic(modCase);
                    }
                    break;
                case RestAction.Created:
                    if (isInternal)
                    {
                        embed.Description = translator.T().NotificationModcaseCreateInternal(modCase, actor);
                    }
                    else
                    {
                        embed.Description = translator.T().NotificationModcaseCreatePublic(modCase);
                    }
                    break;
            }

            // Punishment
            embed.AddField($"{SCALES_EMOTE} - {translator.T().Punishment()}", modCase.GetPunishment(translator), true);
            if (modCase.PunishedUntil != null)
            {
                embed.AddField($"{ALARM_CLOCK} - {translator.T().PunishmentUntil()}", modCase.PunishedUntil.Value.ToDiscordTS(), true);
            }

            // Labels
            if (modCase.Labels.Length != 0)
            {
                StringBuilder sb = new();
                foreach (string label in modCase.Labels)
                {
                    sb.Append($"`{label}` ");
                    if (sb.ToString().Length > 1000)
                    {
                        break;
                    }
                }
                embed.AddField($"{SCROLL_EMOTE} - {translator.T().Labels()}", sb.ToString(), modCase.PunishedUntil == null);
            }

            return embed;
        }

        public static async Task<EmbedBuilder> CreateFileEmbed(this UploadedFile file, ModCase modCase, RestAction action, IUser actor, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(modCase.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            // Thumbnail
            embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());

            // Footer
            embed.WithFooter($"UserId: {actor.Id} | CaseId: {modCase.CaseId}");

            // Filename
            embed.AddField($"**{translator.T().Filename()}**", file.Name.Truncate(1000));

            switch (action)
            {
                case RestAction.Updated:
                    embed.Description = translator.T().NotificationModcaseFileUpdate(actor);
                    embed.Title = $"**{translator.T().NotificationFilesUpdate().ToUpper()}** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Deleted:
                    embed.Description = translator.T().NotificationModcaseFileDelete(actor);
                    embed.Title = $"**{translator.T().NotificationFilesDelete().ToUpper()}** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Created:
                    embed.Description = translator.T().NotificationModcaseFileCreate(actor);
                    embed.Title = $"**{translator.T().NotificationFilesCreate().ToUpper()}** - #{modCase.CaseId} {modCase.Title}";
                    break;
            }

            return embed;
        }

        public static async Task<EmbedBuilder> CreateCommentEmbed(this ModCaseComment comment, RestAction action, IUser actor, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(comment.ModCase.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            if (actor != null)
            {
                embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());
            }

            switch (action)
            {
                case RestAction.Updated:
                    embed.Description = translator.T().NotificationModcaseCommentsUpdate(actor);
                    embed.Title = $"**{translator.T().NotificationModcaseCommentsShortUpdate().ToUpper()}** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
                case RestAction.Deleted:
                    embed.Description = translator.T().NotificationModcaseCommentsDelete(actor);
                    embed.Title = $"**{translator.T().NotificationModcaseCommentsShortDelete().ToUpper()}** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
                case RestAction.Created:
                    embed.Description = translator.T().NotificationModcaseCommentsCreate(actor);
                    embed.Title = $"**{translator.T().NotificationModcaseCommentsShortCreate().ToUpper()}** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
            }

            // Message
            embed.AddField($"**{translator.T().Message()}**", comment.Message.Truncate(1000));

            // Footer
            embed.WithFooter($"UserId: {actor.Id} | CaseId: {comment.ModCase.CaseId}");

            return embed;
        }

        public static async Task<EmbedBuilder> CreateUserNoteEmbed(this UserNote userNote, RestAction action, IUser actor, IUser target, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(userNote.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            if (target != null)
            {
                embed.WithThumbnailUrl(target.GetAvatarOrDefaultUrl());
            }

            embed.AddField($"**{translator.T().Description()}**", userNote.Description.Truncate(1000));

            embed.Title = $"{translator.T().UserNote()} #{userNote.Id}";

            embed.WithFooter($"UserId: {userNote.UserId} | UserNoteId: {userNote.Id}");

            return embed;
        }

        public static async Task<EmbedBuilder> CreateUserMapEmbed(this UserMapping userMapping, RestAction action, IUser actor, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(userMapping.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            embed.AddField($"**{translator.T().Description()}**", userMapping.Reason.Truncate(1000));

            embed.Title = $"{translator.T().UserMap()} #{userMapping.Id}";
            embed.Description = translator.T().UserMapBetween(userMapping);

            embed.WithFooter($"UserA: {userMapping.UserA} | UserB: {userMapping.UserB} | UserMapId: {userMapping.Id}");

            return embed;
        }

        public static async Task<EmbedBuilder> CreateTipsEmbedForNewGuilds(this GuildConfig guildConfig, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(guildConfig.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(RestAction.Created, provider);

            embed.Title = translator.T().NotificationRegisterWelcomeToMASZ();
            embed.Description = translator.T().NotificationRegisterDescriptionThanks();

            embed.AddField(
                $"{STAR} {translator.T().Features()}",
                translator.T().NotificationRegisterUseFeaturesCommand(),
                false
            );

            embed.AddField(
                $"{GLOBE} {translator.T().LanguageWord()}",
                translator.T().NotificationRegisterDefaultLanguageUsed(guildConfig.PreferredLanguage.ToString()),
                false
            );

            embed.AddField(
                $"{CLOCK} {translator.T().Timestamps()}",
                translator.T().NotificationRegisterConfusingTimestamps(),
                false
            );

            embed.AddField(
                $"{HAND_SHAKE} {translator.T().Support()}",
                translator.T().NotificationRegisterSupport(),
                false
            );

            embed.WithFooter($"GuildId: {guildConfig.GuildId}");

            return embed;
        }

        public static async Task<EmbedBuilder> CreateInternalAutomodEmbed(this AutoModerationEvent autoModerationEvent, GuildConfig guildConfig, IUser user, ITextChannel channel, IServiceProvider provider, PunishmentType? punishmentType = null)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(autoModerationEvent.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(RestAction.Created, provider);

            embed.Title = translator.T().Automoderation();
            embed.Description = translator.T().NotificationAutomoderationInternal(user);

            embed.AddField(
                translator.T().Channel(),
                channel.Mention,
                true
            );

            embed.AddField(
                translator.T().Message(),
                $"[{autoModerationEvent.MessageId}](https://discord.com/channels/{autoModerationEvent.GuildId}/{channel.Id}/{autoModerationEvent.MessageId})",
                true
            );

            embed.AddField(
                translator.T().Type(),
                translator.T().Enum(autoModerationEvent.AutoModerationType),
                false
            );

            if (! string.IsNullOrWhiteSpace(autoModerationEvent.MessageContent)) {
                embed.AddField(
                    translator.T().MessageContent(),
                    autoModerationEvent.MessageContent,
                    false
                );
            }

            embed.AddField(
                translator.T().Action(),
                translator.T().Enum(autoModerationEvent.AutoModerationAction),
                false
            );

            if ((autoModerationEvent.AutoModerationAction == AutoModerationAction.CaseCreated ||
                 autoModerationEvent.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated) &&
                punishmentType != null)
            {
                embed.AddField(
                    translator.T().Punishment(),
                    translator.T().Enum(punishmentType.Value),
                    false
                );
            }

            embed.WithFooter($"GuildId: {guildConfig.GuildId}");

            return embed;
        }

        public static async Task<EmbedBuilder> CreateAutomodConfigEmbed(this AutoModerationConfig autoModerationConfig, IUser actor, RestAction action, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(autoModerationConfig.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            if (actor != null)
            {
                embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());
            }

            embed.WithTitle(translator.T().Automoderation() + ": " + translator.T().Enum(autoModerationConfig.AutoModerationType));

            switch (action)
            {
                case RestAction.Created:
                    embed.WithDescription(translator.T().NotificationAutomoderationConfigInternalCreate(translator.T().Enum(autoModerationConfig.AutoModerationType), actor));
                    break;
                case RestAction.Updated:
                    embed.WithDescription(translator.T().NotificationAutomoderationConfigInternalUpdate(translator.T().Enum(autoModerationConfig.AutoModerationType), actor));
                    break;
                case RestAction.Deleted:
                    return embed.WithDescription(translator.T().NotificationAutomoderationConfigInternalDelete(translator.T().Enum(autoModerationConfig.AutoModerationType), actor));
            }

            if (autoModerationConfig.Limit != null)
            {
                embed.AddField(translator.T().NotificationAutomoderationConfigLimit(), $"`{autoModerationConfig.Limit}`", true);
            }
            if (autoModerationConfig.TimeLimitMinutes != null)
            {
                embed.AddField(translator.T().NotificationAutomoderationConfigTimeLimit(), $"`{autoModerationConfig.TimeLimitMinutes}`", true);
            }
            if (autoModerationConfig.Limit != null || autoModerationConfig.TimeLimitMinutes != null)
            {
                embed.AddField("\u200b", "\u200b");
            }

            if (autoModerationConfig.IgnoreRoles.Length > 0)
            {
                embed.AddField(translator.T().NotificationAutomoderationConfigIgnoredRoles(), string.Join(" ", autoModerationConfig.IgnoreRoles.Select(x => $"<@&{x}>")), true);
            }
            if (autoModerationConfig.IgnoreChannels.Length > 0)
            {
                embed.AddField(translator.T().NotificationAutomoderationConfigIgnoredChannels(), string.Join(" ", autoModerationConfig.IgnoreChannels.Select(x => $"<#{x}>")), true);
            }
            if (autoModerationConfig.IgnoreRoles.Length > 0 || autoModerationConfig.IgnoreChannels.Length > 0)
            {
                embed.AddField("\u200b", "\u200b");
            }

            if (autoModerationConfig.PunishmentType != null && (autoModerationConfig.AutoModerationAction == AutoModerationAction.CaseCreated || autoModerationConfig.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated))
            {
                embed.AddField($"{SCALES_EMOTE} {translator.T().Punishment()}", translator.T().Enum(autoModerationConfig.PunishmentType.Value), true);
                if (autoModerationConfig.PunishmentDurationMinutes > 0)
                {
                    embed.AddField($"{ALARM_CLOCK} {translator.T().NotificationAutomoderationConfigDuration()}", $"`{autoModerationConfig.PunishmentDurationMinutes}`", true);
                }
                embed.AddField(
                    translator.T().NotificationAutomoderationConfigSendPublic(),
                    autoModerationConfig.SendPublicNotification ? CHECK : X_CHECK,
                    true);
                embed.AddField(
                    translator.T().NotificationAutomoderationConfigSendDM(),
                    autoModerationConfig.SendDmNotification ? CHECK : X_CHECK,
                    true);
            }
            embed.AddField(
                translator.T().NotificationAutomoderationConfigDeleteMessage(),
                autoModerationConfig.AutoModerationAction == AutoModerationAction.ContentDeleted || autoModerationConfig.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated ? CHECK : X_CHECK,
                true);


            return embed;
        }

        public static async Task<EmbedBuilder> CreateMotdEmbed(this GuildMotd motd, IUser actor, RestAction action, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(motd.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            if (actor != null)
            {
                embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());
            }

            embed.WithTitle(translator.T().MotD());

            switch (action)
            {
                case RestAction.Created:
                    embed.WithDescription(translator.T().NotificationMotdInternalCreate(actor));
                    break;
                case RestAction.Updated:
                    embed.WithDescription(translator.T().NotificationMotdInternalEdited(actor));
                    break;
            }

            embed.AddField(translator.T().NotificationMotdShow(), motd.ShowMotd ? CHECK : X_CHECK, false);
            embed.AddField(translator.T().Message(), motd.Message.Truncate(1000), false);

            return embed;
        }

        public static async Task<EmbedBuilder> CreateGuildAuditLogEmbed(this GuildLevelAuditLogConfig config, IUser actor, RestAction action, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(config.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(action, provider, actor);

            if (actor != null)
            {
                embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());
            }

            embed.WithTitle(translator.T().NotificationGuildAuditLogTitle());

            switch (action)
            {
                case RestAction.Created:
                    embed.WithDescription(translator.T().NotificationGuildAuditLogInternalCreate(translator.T().Enum(config.GuildAuditLogEvent), actor));
                    break;
                case RestAction.Updated:
                    embed.WithDescription(translator.T().NotificationGuildAuditLogInternalUpdate(translator.T().Enum(config.GuildAuditLogEvent), actor));
                    break;
                case RestAction.Deleted:
                    return embed.WithDescription(translator.T().NotificationGuildAuditLogInternalDelete(translator.T().Enum(config.GuildAuditLogEvent), actor));
            }

            embed.AddField(translator.T().Channel(), $"<#{config.ChannelId}>", false);

            if ((config.PingRoles?.Length ?? 0) > 0)
            {
                embed.AddField(translator.T().NotificationGuildAuditLogMentionRoles(), string.Join(" ", config.PingRoles.Select(x => $"<@&{x}>")), false);
            }

            if ((config.IgnoreRoles?.Length ?? 0) > 0)
            {
                embed.AddField(translator.T().NotificationGuildAuditLogExcludeRoles(), string.Join(" ", config.IgnoreRoles.Select(x => $"<@&{x}>")), false);
            }

            if ((config.IgnoreChannels?.Length ?? 0) > 0)
            {
                embed.AddField(translator.T().NotificationGuildAuditLogExcludeChannels(), string.Join(" ", config.IgnoreChannels.Select(x => $"<#{x}>")), false);
            }

            return embed;
        }

        public static async Task<EmbedBuilder> CreateEmbedForNewAppeal(this Appeal appeal, IUser actor, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(appeal.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(RestAction.Created, provider, actor);

            if (actor != null)
            {
                embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());
            }
            embed.Description = translator.T().NotificationAppealsCreate(appeal.UserId);
            embed.Title = $"**{translator.T().NotificationAppealsAppeal().ToUpper()}** - {actor.Username}#{actor.Discriminator}";

            // Footer
            embed.WithFooter($"UserId: {appeal.UserId} | AppealId: {appeal.Id}");

            return embed;
        }

        public static async Task<EmbedBuilder> CreateEmbedForUpdatedAppeal(this Appeal appeal, IUser actor, IUser user, IServiceProvider provider)
        {
            var translator = provider.GetService<Translator>();

            await translator.SetContext(appeal.GuildId);

            EmbedBuilder embed = CreateBasicEmbed(RestAction.Updated, provider, actor);

            if (actor != null)
            {
                embed.WithThumbnailUrl(actor.GetAvatarOrDefaultUrl());
            }
            embed.Description = translator.T().NotificationAppealsUpdate(appeal.UserId, actor.Id);
            embed.Title = $"**{translator.T().NotificationAppealsAppeal().ToUpper()}** - {user?.Username ?? appeal.Username}#{user?.Discriminator ?? appeal.Discriminator}";

            embed.AddField(translator.T().NotificationAppealsStatus(), translator.T().Enum(appeal.Status), false);

            embed.AddField(translator.T().NotificationAppealsReason(), appeal.ModeratorComment.Truncate(1024), false);

            // Footer
            embed.WithFooter($"UserId: {appeal.UserId} | AppealId: {appeal.Id}");

            return embed;
        }
    }
}
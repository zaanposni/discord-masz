using System.Text;
using System.Threading.Tasks;
using Discord;
using masz.Dtos.DiscordAPIResponses;
using masz.Helpers;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class DiscordAnnouncer : IDiscordAnnouncer
    {
        private readonly ILogger<DiscordAnnouncer> logger;
        private readonly IDatabase dbContext;
        private readonly IOptions<InternalConfig> config;
        private readonly IDiscordAPIInterface discord;
        private readonly INotificationEmbedCreator notificationEmbedCreator;
        private readonly ITranslator translator;

        public DiscordAnnouncer() { }

        public DiscordAnnouncer(ILogger<DiscordAnnouncer> logger, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IDatabase context, INotificationEmbedCreator notificationContentCreator, ITranslator translator)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.dbContext = context;
            this.notificationEmbedCreator = notificationContentCreator;
            this.translator = translator;
        }

        // https://codereview.stackexchange.com/a/257121
        private static string GetEnvironmentVariable(string name, string defaultValue)
            => System.Environment.GetEnvironmentVariable(name) is string v && v.Length > 0 ? v : defaultValue;

        public async Task AnnounceModCase(ModCase modCase, RestAction action, User actor, bool announcePublic, bool announceDm)
        {
            logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            User caseUser = await discord.FetchUserInfo(modCase.UserId, CacheBehavior.Default);
            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(modCase.GuildId);

            if (announceDm && action != RestAction.Deleted)
            {
                logger.LogInformation($"Sending dm notification");

                Guild guild = await discord.FetchGuildInfo(modCase.GuildId, CacheBehavior.Default);
                string prefix = GetEnvironmentVariable("DISCORD_PREFIX", "$");
                string message = string.Empty;
                switch (modCase.PunishmentType) {
                    case (PunishmentType.Mute):
                        if (modCase.PunishedUntil.HasValue) {
                            message = translator.T().NotificationModcaseDMMuteTemp(modCase, guild, prefix, config.Value.ServiceBaseUrl, "UTC");
                        } else {
                            message = translator.T().NotificationModcaseDMMutePerm(modCase, guild, prefix, config.Value.ServiceBaseUrl);
                        }
                        break;
                    case (PunishmentType.Kick):
                        message = translator.T().NotificationModcaseDMKick(modCase, guild, prefix, config.Value.ServiceBaseUrl);
                        break;
                    case (PunishmentType.Ban):
                        if (modCase.PunishedUntil.HasValue) {
                            message = translator.T().NotificationModcaseDMBanTemp(modCase, guild, prefix, config.Value.ServiceBaseUrl, "UTC");
                        } else {
                            message = translator.T().NotificationModcaseDMBanPerm(modCase, guild, prefix, config.Value.ServiceBaseUrl);
                        }
                        break;
                    default:
                        message = translator.T().NotificationModcaseDMWarn(modCase, guild, prefix, config.Value.ServiceBaseUrl);
                        break;
                }
                await discord.SendDmMessage(modCase.UserId, message);
                logger.LogInformation($"Sent dm notification");
            }

            if (! string.IsNullOrEmpty(guildConfig.ModPublicNotificationWebhook) && announcePublic)
            {
                logger.LogInformation($"Sending public webhook to {guildConfig.ModPublicNotificationWebhook}.");

                EmbedBuilder embed = await notificationEmbedCreator.CreateModcaseEmbed(modCase, action, actor, caseUser, false);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModPublicNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                logger.LogInformation("Sent public webhook.");
            }

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                EmbedBuilder embed = await notificationEmbedCreator.CreateModcaseEmbed(modCase, action, actor, caseUser, true);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceComment(ModCaseComment comment, User actor, RestAction action)
        {
            logger.LogInformation($"Announcing comment {comment.Id} in case {comment.ModCase.CaseId} in guild {comment.ModCase.GuildId}.");

            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(comment.ModCase.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                User discordUser = await discord.FetchUserInfo(comment.UserId, CacheBehavior.Default);

                EmbedBuilder embed = await notificationEmbedCreator.CreateCommentEmbed(comment, action, actor);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceFile(string filename, ModCase modCase, User actor, RestAction action)
        {
            logger.LogInformation($"Announcing file {filename} in case {modCase.CaseId} in guild {modCase.GuildId}.");

            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(modCase.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                EmbedBuilder embed = await notificationEmbedCreator.CreateFileEmbed(filename, modCase, action, actor);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceUserNote(UserNote userNote, User actor, RestAction action)
        {
            logger.LogInformation($"Announcing usernote {userNote.Id} in guild {userNote.GuildId}.");

            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(userNote.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                User user = await this.discord.FetchUserInfo(userNote.UserId, CacheBehavior.Default);

                EmbedBuilder embed = await notificationEmbedCreator.CreateUserNoteEmbed(userNote, action, actor, user);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceUserMapping(UserMapping userMapping, User actor, RestAction action)
        {
            logger.LogInformation($"Announcing usermap {userMapping.Id} in guild {userMapping.GuildId}.");

            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(userMapping.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                EmbedBuilder embed = await notificationEmbedCreator.CreateUserMapEmbed(userMapping, action, actor);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                logger.LogInformation("Sent internal webhook.");
            }
        }
    }
}
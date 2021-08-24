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

        public DiscordAnnouncer() { }

        public DiscordAnnouncer(ILogger<DiscordAnnouncer> logger, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IDatabase context)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.dbContext = context;
        }

        // https://codereview.stackexchange.com/a/257121
        private static string GetEnvironmentVariable(string name, string defaultValue)
            => System.Environment.GetEnvironmentVariable(name) is string v && v.Length > 0 ? v : defaultValue;

        public async Task AnnounceModCase(ModCase modCase, RestAction action, User actor, bool announcePublic, bool announceDm)
        {
            logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            User caseUser = await discord.FetchUserInfo(modCase.UserId, CacheBehavior.Default);
            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(modCase.GuildId);

            if (announceDm && modCase.PunishmentType != PunishmentType.None && action != RestAction.Deleted)
            {
                logger.LogInformation($"Sending dm notification");

                Guild guild = await discord.FetchGuildInfo(modCase.GuildId, CacheBehavior.Default);
                StringBuilder message = new StringBuilder();
                message.Append($"The moderators of guild `{guild.Name}` have ");
                switch (modCase.PunishmentType) {
                    case (PunishmentType.None):
                        message.Append("warned");
                        break;
                    case (PunishmentType.Mute):
                        message.Append("muted");
                        break;
                    case (PunishmentType.Kick):
                        message.Append("kicked");
                        break;
                    case (PunishmentType.Ban):
                        message.Append("banned");
                        break;
                }
                message.Append(" you");
                if (modCase.PunishedUntil != null) {
                    message.Append(" until `");
                    message.Append(modCase.PunishedUntil.Value.ToString("dd.MM.yyyy HH:mm:ss"));
                    message.Append(" (UTC)`");
                }
                string prefix = GetEnvironmentVariable("BOT_PREFIX", "$");
                message.Append($".\nUse `{prefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.");
                message.Append($"\nFor more information or rehabilitation visit: {config.Value.ServiceBaseUrl}");

                await discord.SendDmMessage(modCase.UserId, message.ToString());
                logger.LogInformation($"Sent dm notification");
            }

            if (! string.IsNullOrEmpty(guildConfig.ModPublicNotificationWebhook) && announcePublic)
            {
                logger.LogInformation($"Sending public webhook to {guildConfig.ModPublicNotificationWebhook}.");

                EmbedBuilder embed = NotificationEmbedCreator.CreatePublicCaseEmbed(modCase, action, actor, caseUser, config.Value.ServiceBaseUrl);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModPublicNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                logger.LogInformation("Sent public webhook.");
            }            

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");

                EmbedBuilder embed = NotificationEmbedCreator.CreateInternalCaseEmbed(modCase, action, actor, caseUser, config.Value.ServiceBaseUrl);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                logger.LogInformation("Sent internal webhook.");
            }
        }

        public async Task AnnounceComment(ModCaseComment comment, User actor, RestAction action)
        {
            logger.LogInformation($"Announcing comment {comment.Id} in case {comment.ModCase.CaseId} in guild {comment.ModCase.GuildId}.");

            User discordUser = await discord.FetchUserInfo(comment.UserId, CacheBehavior.Default);
            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(comment.ModCase.GuildId);         

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");
                
                EmbedBuilder embed = NotificationEmbedCreator.CreateInternalCommentEmbed(comment, action, actor, discordUser, config.Value.ServiceBaseUrl);

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
                
                EmbedBuilder embed = NotificationEmbedCreator.CreateInternalFilesEmbed(filename, modCase, action, actor, config.Value.ServiceBaseUrl);

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
                
                User user = await this.discord.FetchUserInfo(userNote.UserId, CacheBehavior.OnlyCache);
                EmbedBuilder embed = NotificationEmbedCreator.CreateInternalUserNoteEmbed(userNote, user, actor, action, config.Value.ServiceBaseUrl);

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
                
                EmbedBuilder embed = NotificationEmbedCreator.CreateInternalUserMappingEmbed(userMapping, actor, action, config.Value.ServiceBaseUrl);

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build());
                logger.LogInformation("Sent internal webhook.");
            }
        }
    }
}
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
        private static string discordCdnBaseUrl = "https://cdn.discordapp.com";
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
        public async Task AnnounceModCase(ModCase modCase, RestAction action, User actor, bool announcePublic)
        {
            logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            User caseUser = await discord.FetchUserInfo(modCase.UserId);
            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(modCase.GuildId);

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

            User discordUser = await discord.FetchUserInfo(comment.UserId);
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
    }
}
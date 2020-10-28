using System.Threading.Tasks;
using Discord;
using masz.Dtos.DiscordAPIResponses;
using masz.Helpers;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class ModCaseAnnouncer : IModCaseAnnouncer
    {
        private static string discordCdnBaseUrl = "https://cdn.discordapp.com";
        private readonly ILogger<ModCaseAnnouncer> logger;
        private readonly IDatabase dbContext;
        private readonly IOptions<InternalConfig> config;
        private readonly IDiscordAPIInterface discord;

        public ModCaseAnnouncer() { }

        public ModCaseAnnouncer(ILogger<ModCaseAnnouncer> logger, IOptions<InternalConfig> config, IDiscordAPIInterface discord, IDatabase context)
        {
            this.logger = logger;
            this.config = config;
            this.discord = discord;
            this.dbContext = context;
        }
        public async Task AnnounceModCase(ModCase modCase, ModCaseAction action, bool announcePublic)
        {
            logger.LogInformation($"Announcing modcase {modCase.Id} in guild {modCase.GuildId}.");

            User discordUser = await discord.FetchUserInfo(modCase.UserId);

            GuildConfig guildConfig = await dbContext.SelectSpecificGuildConfig(modCase.GuildId);

            if (! string.IsNullOrEmpty(guildConfig.ModPublicNotificationWebhook) && announcePublic)
            {
                logger.LogInformation($"Sending public webhook to {guildConfig.ModPublicNotificationWebhook}.");
                EmbedBuilder embed = ModCaseEmbedCreator.CreatePublicAnnouncementEmbed(modCase, action, discordUser, config.Value.ServiceBaseUrl);
                DiscordMessenger.SendEmbedWebhook(guildConfig.ModPublicNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                logger.LogInformation("Sent public webhook.");
            }            

            if (! string.IsNullOrEmpty(guildConfig.ModInternalNotificationWebhook))
            {
                logger.LogInformation($"Sending internal webhook to {guildConfig.ModInternalNotificationWebhook}.");
                EmbedBuilder embed = ModCaseEmbedCreator.CreatePublicAnnouncementEmbed(modCase, action, discordUser, config.Value.ServiceBaseUrl);

                var mod = await discord.FetchMemberInfo(modCase.GuildId, modCase.ModId);
                var author = new EmbedAuthorBuilder();
                author.IconUrl = $"{discordCdnBaseUrl}/avatars/{mod.User.Id}/{mod.User.Avatar}.png";
                author.Name = string.IsNullOrEmpty(mod.Nick) ? mod.User.Username : mod.Nick;
                embed.Author = author;

                DiscordMessenger.SendEmbedWebhook(guildConfig.ModInternalNotificationWebhook, embed.Build(), $"<@{modCase.UserId}>");
                logger.LogInformation("Sent internal webhook.");
            }
        }
    }
}
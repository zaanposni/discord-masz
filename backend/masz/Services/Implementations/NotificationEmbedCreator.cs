using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Services
{
    public class NotificationEmbedCreator : INotificationEmbedCreator
    {
        private readonly ILogger<Scheduler> logger;
        private readonly IOptions<InternalConfig> config;
        private readonly ITranslator translator;
        private readonly string SCALES_EMOTE = "\u2696";
        private readonly string SCROLL_EMOTE = "\uD83C\uDFF7";
        private readonly string ALARM_CLOCK = "\u23F0";

        public NotificationEmbedCreator(ILogger<Scheduler> logger, IOptions<InternalConfig> config, IDatabase context, ITranslator translator)
        {
            this.logger = logger;
            this.config = config;
            this.translator = translator;
        }

        private DiscordEmbedBuilder CreateBasicEmbed(RestAction action, DiscordUser author = null)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.Timestamp = DateTime.Now;

            switch(action) {
                case RestAction.Edited:
                    embed.Color = DiscordColor.Orange;
                    break;
                case RestAction.Deleted:
                    embed.Color = DiscordColor.Red;
                    break;
                case RestAction.Created:
                    embed.Color = DiscordColor.Green;
                    break;
            }

            if (author != null) {
                embed.WithAuthor(
                    author.Username,
                    author.AvatarUrl,
                    author.AvatarUrl
                );
            }

            // Url
            if (! string.IsNullOrEmpty(config.Value.ServiceBaseUrl))
            {
                embed.Url = config.Value.ServiceBaseUrl;
            }

            return embed;
        }

        public async Task<DiscordEmbedBuilder> CreateModcaseEmbed(ModCase modCase, RestAction action, DiscordUser actor, DiscordUser suspect = null, bool isInternal = true)
        {
            await translator.SetContext(modCase.GuildId);
            DiscordEmbedBuilder embed;
            if (isInternal) {
                embed = CreateBasicEmbed(action);
            } else {
                embed = CreateBasicEmbed(action, actor);
            }

            // Thumbnail
            if (suspect != null)
            {
                embed.WithThumbnail(suspect.AvatarUrl);
            }

            // Description
            embed.AddField($"**{translator.T().Description()}**", modCase.Description.Substring(0, Math.Min(modCase.Description.Length, 1000)));

            // Title
            embed.Title = $"#{modCase.CaseId} {modCase.Title}";

            // Footer
            embed.WithFooter($"UserId: {modCase.UserId} | ModCaseId: {modCase.CaseId}");

            // Description
            switch(action){
                case RestAction.Edited:
                    if (isInternal) {
                        embed.Description = translator.T().NotificationModcaseUpdateInternal(modCase, actor);
                    } else {
                        embed.Description = translator.T().NotificationModcaseUpdatePublic(modCase);
                    }
                    break;
                case RestAction.Deleted:
                    if (isInternal) {
                        embed.Description = translator.T().NotificationModcaseDeleteInternal(modCase, actor);
                    } else {
                        embed.Description = translator.T().NotificationModcaseDeletePublic(modCase);
                    }
                    break;
                case RestAction.Created:
                    if (isInternal) {
                        embed.Description = translator.T().NotificationModcaseCreateInternal(modCase, actor);
                    } else {
                        embed.Description = translator.T().NotificationModcaseCreatePublic(modCase);
                    }
                    break;
            }

            // Punishment
            embed.AddField($"{SCALES_EMOTE} - {translator.T().Punishment()}", modCase.GetPunishment(translator), true);
            if (modCase.PunishedUntil != null)
            {
                embed.AddField($"{ALARM_CLOCK} - {translator.T().PunishmentUntil("UTC")}", modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss"), true);
            }

            // Labels
            if (modCase.Labels.Length != 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach(string label in modCase.Labels)
                {
                    sb.Append($"`{label}` ");
                    if (sb.ToString().Length > 1000) {
                        break;
                    }
                }
                embed.AddField($"{SCROLL_EMOTE} - {translator.T().Labels()}", sb.ToString(), modCase.PunishedUntil == null);
            }

            return embed;
        }

        public async Task<DiscordEmbedBuilder> CreateFileEmbed(string filename, ModCase modCase, RestAction action, DiscordUser actor)
        {
            await translator.SetContext(modCase.GuildId);
            DiscordEmbedBuilder embed = CreateBasicEmbed(action, actor);

            // Thumbnail
            embed.WithThumbnail(actor.AvatarUrl);

            // Footer
            embed.WithFooter($"UserId: {actor.Id} | ModCaseId: {modCase.CaseId}");

            // Filename
            embed.AddField($"**{translator.T().Filename()}**", filename.Substring(0, Math.Min(filename.Length, 1000)));

            switch(action){
                case RestAction.Edited:
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

        public async Task<DiscordEmbedBuilder> CreateCommentEmbed(ModCaseComment comment, RestAction action, DiscordUser actor)
        {
            await translator.SetContext(comment.ModCase.GuildId);
            DiscordEmbedBuilder embed = CreateBasicEmbed(action, actor);

            if (actor != null)
            {
                embed.WithThumbnail(actor.AvatarUrl);
            }

            switch(action){
                case RestAction.Edited:
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
            embed.AddField($"**{translator.T().Message()}**", comment.Message.Substring(0, Math.Min(comment.Message.Length, 1000)));

            // Footer
            embed.WithFooter($"UserId: {actor.Id} | ModCaseId: {comment.ModCase.CaseId}");

            return embed;
        }

        public async Task<DiscordEmbedBuilder> CreateUserNoteEmbed(UserNote userNote, RestAction action, DiscordUser actor, DiscordUser target)
        {
            await translator.SetContext(userNote.GuildId);
            DiscordEmbedBuilder embed = CreateBasicEmbed(action, actor);

            if (actor != null)
            {
                embed.WithThumbnail(target.AvatarUrl);
            }

            embed.AddField($"**{translator.T().Description()}**", userNote.Description.Substring(0, Math.Min(userNote.Description.Length, 1000)));

            embed.Title = $"Usernote #{userNote.Id}";

            embed.WithFooter($"UserId: {userNote.UserId} | UserNoteId: {userNote.Id}");

            return embed;
        }

        public async Task<DiscordEmbedBuilder> CreateUserMapEmbed(UserMapping userMapping, RestAction action, DiscordUser actor)
        {
            await translator.SetContext(userMapping.GuildId);
            DiscordEmbedBuilder embed = CreateBasicEmbed(action, actor);

            embed.AddField($"**{translator.T().Description()}**", userMapping.Reason.Substring(0, Math.Min(userMapping.Reason.Length, 1000)));

            embed.Title = $"Usermap #{userMapping.Id}";
            embed.Description = $"Usermap between <@{userMapping.UserA}> and <@{userMapping.UserB}>.";

            embed.WithFooter($"UserA: {userMapping.UserA} | UserB: {userMapping.UserB} | UserMapId: {userMapping.Id}");

            return embed;
        }
    }
}
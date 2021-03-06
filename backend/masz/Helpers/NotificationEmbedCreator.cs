using System;
using System.Text;
using Discord;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;

namespace masz.Helpers
{
    public class NotificationEmbedCreator
    {
        private static string SCALES_EMOTE = "\u2696";
        private static string SCROLL_EMOTE = "\uD83C\uDFF7";
        private static string ALARM_CLOCK = "\u23F0";
        private static string discordCdnBaseUrl = "https://cdn.discordapp.com";

        private static EmbedBuilder CreateBasicEmbed(RestAction action) 
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.Timestamp = DateTime.Now;

            switch(action){
                case RestAction.Edited:
                    embed.Color = Color.Orange;
                    break;
                case RestAction.Deleted:
                    embed.Color = Color.Red;
                    break;
                case RestAction.Created:
                    embed.Color = Color.Green;
                    break;
            }

            return embed;
        }

        public static EmbedBuilder CreateInternalFilesEmbed(string filename, ModCase modCase, RestAction action, User actor = null, String serviceBaseUrl = null)
        {
            EmbedBuilder embed = CreateBasicEmbed(action);
            
            if (actor != null)
            {
                embed.ThumbnailUrl = actor.ImageUrl;
            }

            if (! string.IsNullOrEmpty(filename))
            {
                embed.AddField("**Filename**", filename.Substring(0, Math.Min(filename.Length, 1000)));
            }

            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {actor.Id} | ModCaseId: {modCase.CaseId}";
            embed.Footer = footer;

            StringBuilder description = new StringBuilder();
            description.Append($"A **File** has been ");

            switch(action){
                case RestAction.Edited:
                    description.Append($"updated");
                    embed.Title = $"**UPDATED FILE** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Deleted:
                    description.Append($"deleted");
                    embed.Title = $"**DELETED FILE** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Created:
                    description.Append($"uploaded");
                    embed.Title = $"**CREATED FILE** - #{modCase.CaseId} {modCase.Title}";
                    break;
            }
            description.Append($" by <@{actor.Id}>");
            if (actor != null) {
                description.Append($" ({actor.Username}#{actor.Discriminator}).");
            } else {
                description.Append(".");
            }

            embed.Description = description.ToString();
            
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = serviceBaseUrl;
            }

            return embed;
        }

        public static EmbedBuilder CreateInternalCommentEmbed(ModCaseComment comment, RestAction action, User actor, User discordUser = null, String serviceBaseUrl = null)
        {
            EmbedBuilder embed = CreateBasicEmbed(action);
            
            if (discordUser != null)
            {
                embed.ThumbnailUrl = discordUser.ImageUrl;
            }

            if (! string.IsNullOrEmpty(comment.Message))
            {
                embed.AddField("**Message**", comment.Message.Substring(0, Math.Min(comment.Message.Length, 1000)));
            }

            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {comment.UserId} | ModCaseId: {comment.ModCase.CaseId}";
            embed.Footer = footer;

            StringBuilder description = new StringBuilder();

            switch(action){
                case RestAction.Edited:
                    description.Append($"A **Comment** by <@{comment.UserId}>");
                    if (discordUser != null) {
                        description.Append($" ({discordUser.Username}#{discordUser.Discriminator})");
                    }
                    description.Append($" has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).");
                    embed.Title = $"**UPDATED COMMENT** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
                case RestAction.Deleted:
                    description.Append($"A **Comment** by <@{comment.UserId}>");
                    if (discordUser != null) {
                        description.Append($" ({discordUser.Username}#{discordUser.Discriminator})");
                    }
                    description.Append($" has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).");
                    embed.Title = $"**DELETED COMMENT** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
                case RestAction.Created:
                    description.Append($"A **Comment** has been created by <@{comment.UserId}> ({actor.Username}#{actor.Discriminator}).");
                    embed.Title = $"**CREATED COMMENT** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
            }

            embed.Description = description.ToString();
            
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = serviceBaseUrl;
            }

            var author = new EmbedAuthorBuilder();
            author.IconUrl = $"{discordCdnBaseUrl}/avatars/{actor.Id}/{actor.Avatar}.png";
            author.Name = actor.Username;
            embed.Author = author;

            return embed;
        }

        public static EmbedBuilder CreateInternalCaseEmbed(ModCase modCase, RestAction action, User actor, User caseUser = null, String serviceBaseUrl = null, bool isInternal = true)
        {
            EmbedBuilder embed = CreateBasicEmbed(action);
            
            if (caseUser != null)
            {
                embed.ThumbnailUrl = caseUser.ImageUrl;
            }

            if (! string.IsNullOrEmpty(modCase.Description)) {
                embed.AddField("**Description**", modCase.Description.Substring(0, Math.Min(modCase.Description.Length, 1000)));
            }
            
            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {modCase.UserId} | ModCaseId: {modCase.CaseId}";
            embed.Footer = footer;

            StringBuilder description = new StringBuilder();
            description.Append($"A **Modcase** for <@{modCase.UserId}> ");
            
            if (caseUser != null) {
                description.Append($"({caseUser.Username}#{caseUser.Discriminator}) ");
            }

            switch(action){
                case RestAction.Edited:
                    description.Append($"has been updated");
                    embed.Title = $"#{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Deleted:
                    description.Append($"has been deleted");
                    embed.Title = $"#{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Created:
                    description.Append($"has been created");
                    embed.Title = $"#{modCase.CaseId} {modCase.Title}";
                    break;
            }

            if (isInternal) 
            {
                description.Append($" by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).");
            } else {
                description.Append(".");
            }

            embed.Description = description.ToString();
            
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = serviceBaseUrl;
            }

            if (!string.IsNullOrEmpty(modCase.Punishment))
            {
                embed.AddField(SCALES_EMOTE + " - Punishment", modCase.Punishment.Substring(0, Math.Min(modCase.Punishment.Length, 1000)), true);
            }

            if (modCase.PunishedUntil != null)
            {
                embed.AddField(ALARM_CLOCK + " - Punished Until (UTC)", modCase.PunishedUntil.Value.ToString("dd.MM.yyyy HH:mm:ss"), true);
            }

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
                embed.AddField(SCROLL_EMOTE + " - Labels", sb.ToString(), modCase.PunishedUntil == null);
            }

            if (isInternal)
            {
                var author = new EmbedAuthorBuilder();
                author.IconUrl = actor.ImageUrl;
                author.Name = actor.Username;
                embed.Author = author;
            }

            return embed;
        }
    
        public static EmbedBuilder CreatePublicCaseEmbed(ModCase modCase, RestAction action, User actor, User caseUser = null, String serviceBaseUrl = null)
        {
            return CreateInternalCaseEmbed(modCase, action, actor, caseUser, serviceBaseUrl, false);
        }

        public static EmbedBuilder CreateInternalUserNoteEmbed(UserNote userNote, User user, User moderator, RestAction restAction, String serviceBaseUrl = null)
        {
            EmbedBuilder embed = CreateBasicEmbed(restAction);
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = serviceBaseUrl;
            }

            embed.Title = $"Usernote #{userNote.Id}";
            embed.AddField("**Description**", userNote.Description.Substring(0, Math.Min(userNote.Description.Length, 1000)));
            if (user != null)
            {
                embed.ThumbnailUrl = user.ImageUrl;
            }            

            var author = new EmbedAuthorBuilder();
            author.IconUrl = moderator.ImageUrl;
            author.Name = moderator.Username;
            embed.Author = author;

            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {userNote.UserId} | UserNoteId: {userNote.Id}";
            embed.Footer = footer;

            return embed;
        }

        public static EmbedBuilder CreateInternalUserMappingEmbed(UserMapping userMapping, User moderator, RestAction restAction, String serviceBaseUrl = null)
        {
            EmbedBuilder embed = CreateBasicEmbed(restAction);
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = serviceBaseUrl;
            }

            embed.Title = $"Usermap #{userMapping.Id}";
            embed.Description = $"Usermap between <@{userMapping.UserA}> and <@{userMapping.UserB}>.";

            embed.AddField("**Description**", userMapping.Reason.Substring(0, Math.Min(userMapping.Reason.Length, 1000)));

            var author = new EmbedAuthorBuilder();
            author.IconUrl = moderator.ImageUrl;
            author.Name = moderator.Username;
            embed.Author = author;

            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserA: {userMapping.UserA} | UserB: {userMapping.UserB} | UserMapId: {userMapping.Id}";
            embed.Footer = footer;

            return embed;
        }
    }
}

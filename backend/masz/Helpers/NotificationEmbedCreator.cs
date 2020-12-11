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
        private static string EXCLAMATION_EMOTE = "\u2757";
        private static string SCROLL_EMOTE = "\uD83C\uDFF7";
        private static string discordBaseUrl = "https://discord.com/api";
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

        public static EmbedBuilder CreateInternalCommentEmbed(ModCaseComment comment, RestAction action,  User discordUser = null, String serviceBaseUrl = null)
        {
            EmbedBuilder embed = CreateBasicEmbed(action);
            
            if (discordUser != null)
            {
                embed.ThumbnailUrl = $"{discordCdnBaseUrl}/avatars/{discordUser.Id}/{discordUser.Avatar}.png";
            }

            if (! string.IsNullOrEmpty(comment.Message))
            {
                embed.AddField("**Message**", comment.Message.Substring(0, Math.Min(comment.Message.Length, 1000)));
            }

            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {comment.UserId} | ModCaseId: {comment.ModCase.CaseId}";
            embed.Footer = footer;

            switch(action){
                case RestAction.Edited:
                    embed.Description = $"A **Comment** by <@{comment.UserId}> has been updated.\n" + 
                                         "This notification has been generated automatically.\n" + 
                                        $"Follow [this link]({serviceBaseUrl}/modcases/{comment.ModCase.GuildId}/{comment.ModCase.CaseId}) to see more details.\n" +
                                         "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**UPDATED COMMENT** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
                case RestAction.Deleted:
                    embed.Description = $"A **Comment** by <@{comment.UserId}> has been deleted.\n" + 
                                        "This notification has been generated automatically.\n" +
                                        $"Follow [this link]({serviceBaseUrl}/modcases/{comment.ModCase.GuildId}/{comment.ModCase.CaseId}) to see more details.\n" +
                                        "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**DELETED COMMENT** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
                case RestAction.Created:
                    embed.Description = $"A new **Comment** has been created by <@{comment.UserId}>.\n" + 
                                         "This notification has been generated automatically.\n" + 
                                        $"Follow [this link]({serviceBaseUrl}/modcases/{comment.ModCase.GuildId}/{comment.ModCase.CaseId}) to see more details.\n" +
                                         "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**CREATED COMMENT** - #{comment.ModCase.CaseId} {comment.ModCase.Title}";
                    break;
            }
            
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = $"{serviceBaseUrl}/modcases/{comment.ModCase.GuildId}/{comment.ModCase.CaseId}";
            }

            return embed;
        }

        public static EmbedBuilder CreateInternalCaseEmbed(ModCase modCase, RestAction action, User discordUser = null, String serviceBaseUrl = null, GuildMember modUser = null)
        {
            EmbedBuilder embed = CreateBasicEmbed(action);
            
            if (discordUser != null)
            {
                embed.ThumbnailUrl = $"{discordCdnBaseUrl}/avatars/{discordUser.Id}/{discordUser.Avatar}.png";
            }

            if (! string.IsNullOrEmpty(modCase.Description))
            {
                embed.AddField("**Description**", modCase.Description.Substring(0, Math.Min(modCase.Description.Length, 1000)));
            }
            
            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {modCase.UserId} | ModCaseId: {modCase.CaseId}";
            embed.Footer = footer;

            StringBuilder description = new StringBuilder();
            description.Append($"A **Modcase** has been ");

            switch(action){
                case RestAction.Edited:
                    description.Append($"updated");
                    embed.Title = $"**UPDATED** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Deleted:
                    description.Append($"deleted");
                    embed.Title = $"**DELETED** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case RestAction.Created:
                    description.Append($"created");
                    embed.Title = $"**CREATED** - #{modCase.CaseId} {modCase.Title}";
                    break;
            }
            
            description.Append($" for <@{modCase.UserId}> ");
            if (discordUser != null) {
                description.Append($" ({discordUser.Username}#{discordUser.Discriminator}).");
            }
            description.Append($"\nThis notification has been generated automatically.\n");
            if (action != RestAction.Deleted) {
                description.Append($"Follow [this link]({serviceBaseUrl}/modcases/{modCase.GuildId}/{modCase.CaseId}) to see more details.\n");
            }
            description.Append($"[Contribute](https://github.com/zaanposni/discord-masz/blob/master/CONTRIBUTING.md) to this moderation tool.");

            embed.Description = description.ToString();
            
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = $"{serviceBaseUrl}/modcases/{modCase.GuildId}/{modCase.CaseId}";
            }

            if (!string.IsNullOrEmpty(modCase.Punishment))
            {
                embed.AddField(SCALES_EMOTE + " - Punishment", modCase.Punishment.Substring(0, Math.Min(modCase.Punishment.Length, 1000)), true);
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
                embed.AddField(SCROLL_EMOTE + " - Labels", sb.ToString(), true);
            }

            if (modUser != null) {
                var author = new EmbedAuthorBuilder();
                author.IconUrl = $"{discordCdnBaseUrl}/avatars/{modUser.User.Id}/{modUser.User.Avatar}.png";
                author.Name = string.IsNullOrEmpty(modUser.Nick) ? modUser.User.Username : modUser.Nick;
                embed.Author = author;
            }

            return embed;
        }
    
        public static EmbedBuilder CreatePublicCaseEmbed(ModCase modCase, RestAction action, User discordUser = null, String serviceBaseUrl = null)
        {
            EmbedBuilder embed = CreateInternalCaseEmbed(modCase, action, discordUser, serviceBaseUrl);
            embed.Fields.RemoveAt(0);  // remove description field
            return embed;
        }
    }
}

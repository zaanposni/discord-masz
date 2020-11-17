using System;
using System.Text;
using Discord;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;

namespace masz.Helpers
{
    public class ModCaseEmbedCreator
    {
        private static string SCALES_EMOTE = "\u2696";
        private static string EXCLAMATION_EMOTE = "\u2757";
        private static string SCROLL_EMOTE = "\uD83C\uDFF7";
        private static string discordBaseUrl = "https://discord.com/api";
        private static string discordCdnBaseUrl = "https://cdn.discordapp.com";

        public static EmbedBuilder CreatePublicAnnouncementEmbed(ModCase modCase, ModCaseAction action, User discordUser = null, String serviceBaseUrl = null)
        {
            var embed = new EmbedBuilder();
            
            if (discordUser != null)
            {
                embed.ThumbnailUrl = $"{discordCdnBaseUrl}/avatars/{discordUser.Id}/{discordUser.Avatar}.png";
            }

            if (! string.IsNullOrEmpty(modCase.Description))
            {
                embed.AddField("**Description**", modCase.Description.Substring(0, Math.Min(modCase.Description.Length, 1000)));
            }
            


            embed.Timestamp = DateTime.Now;
            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {modCase.UserId} | ModCaseId: {modCase.CaseId}";
            embed.Footer = footer;

            switch(action){
                case ModCaseAction.Edited:
                    embed.Color = Color.Orange;
                    embed.Description = $"A **Modcase** has been updated for <@{modCase.UserId}>.\n" + 
                                         "This notification has been generated automatically.\n" + 
                                        $"Follow [this link]({serviceBaseUrl}/modcases/{modCase.GuildId}/{modCase.CaseId}) to see more details.\n" +
                                         "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**UPDATED** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case ModCaseAction.Deleted:
                    embed.Color = Color.Red;
                    embed.Description = $"A **Modcase** has been deleted for <@{modCase.UserId}>.\n" + 
                                        "This notification has been generated automatically.\n" +
                                        "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**DELETED** - #{modCase.CaseId} {modCase.Title}";
                    break;
                case ModCaseAction.Created:
                    embed.Color = Color.Green;
                    embed.Description = $"A new **Modcase** has been created for <@{modCase.UserId}>.\n" + 
                                         "This notification has been generated automatically.\n" + 
                                        $"Follow [this link]({serviceBaseUrl}/modcases/{modCase.GuildId}/{modCase.CaseId}) to see more details.\n" +
                                         "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**CREATED** - #{modCase.CaseId} {modCase.Title}";
                    break;
            }
            
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

            return embed;
        }
    }
}

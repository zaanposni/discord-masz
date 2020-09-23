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

        public static EmbedBuilder CreatePublicAnnouncementEmbed(ModCase modCase, string action, GuildMember member = null, String serviceBaseUrl = null)
        {
            var embed = new EmbedBuilder();
            
            if (member != null)
            {
                embed.ThumbnailUrl = $"{discordCdnBaseUrl}/avatars/{member.User.Id}/{member.User.Avatar}.png";
            }

            if (! string.IsNullOrEmpty(modCase.Description))
            {
                embed.AddField("**Description**", modCase.Description.Substring(0, Math.Min(modCase.Description.Length, 1000)));
            }
            


            embed.Timestamp = DateTime.Now;
            var footer = new EmbedFooterBuilder();
            footer.Text = $"UserId: {modCase.UserId} | ModCaseId: {modCase.Id}";
            embed.Footer = footer;

            switch(action){
                case "edited":
                    embed.Color = Color.Orange;
                    embed.Description = $"A **Modcase** has been updated for <@{modCase.UserId}>.\n" + 
                                         "This notification has been generated automatically.\n" + 
                                        $"Follow [this link]({serviceBaseUrl}/{modCase.GuildId}/modcase/{modCase.Id}) to see more details.\n" +
                                         "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**UPDATED** - {modCase.Title}";
                    break;
                default:  // on create
                    embed.Color = Color.Green;
                    embed.Description = $"A new **Modcase** has been registered for <@{modCase.UserId}>.\n" + 
                                         "This notification has been generated automatically.\n" + 
                                        $"Follow [this link]({serviceBaseUrl}/{modCase.GuildId}/modcase/{modCase.Id}) to see more details.\n" +
                                         "[Contribute](https://github.com/zaanposni/discord-masz/) to this moderation tool.";
                    embed.Title = $"**CREATED** - {modCase.Title}";
                    break;
            }
            
            if (! string.IsNullOrEmpty(serviceBaseUrl))
            {
                embed.Url = $"{serviceBaseUrl}/{modCase.GuildId}/modcases/{modCase.Id}";
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
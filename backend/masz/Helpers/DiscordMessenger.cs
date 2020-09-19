using Discord;
using Discord.Webhook;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Helpers
{
    public class DiscordMessenger
    {
        private static string discordBaseUrl = "https://discord.com/api";

        public static async void SendEmbedWebhook(string webhookUrl, Embed embed, string text = "")
        {
            var DCW = new DiscordWebhookClient(webhookUrl: webhookUrl);

            Embed[] embedArray = new Embed[] { embed };

            using (var client = DCW)
            {
                await client.SendMessageAsync(text: text, embeds: embedArray);
            }
        }
    }
}
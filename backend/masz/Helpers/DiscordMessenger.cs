using Discord;
using Discord.Webhook;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Helpers
{
    public class DiscordMessenger
    {
        public static async void SendEmbedWebhook(string webhookUrl, Embed embed, string text = "")
        {
            DiscordWebhookClient DCW;
            try {
                DCW = new DiscordWebhookClient(webhookUrl: webhookUrl);
            }
            catch(Discord.Net.HttpException ex) {
                System.Console.WriteLine($"Failed to initialize webhook.\n{ex.ToString()}");
                return;
            }

            Embed[] embedArray = new Embed[] { embed };

            using (var client = DCW)
            {
                await client.SendMessageAsync(text: text, embeds: embedArray);
            }
        }
    }
}
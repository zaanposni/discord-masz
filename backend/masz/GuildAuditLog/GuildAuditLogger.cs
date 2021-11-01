using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Models;

namespace masz.GuildAuditLog
{
    public static class GuildAuditLogger
    {
        public static DiscordEmbedBuilder GenerateBaseEmbed(DiscordColor color)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(color);
            embed.WithTimestamp(DateTime.Now);
            return embed;
        }

        public static async Task HandleEvent<T>(DiscordClient client, T args, Func<DiscordClient, T, DiscordEmbedBuilder> predicate, GuildLevelAuditLogConfig config) where T : DiscordEventArgs
        {
            DiscordChannel channel = null;
            try
            {
                channel = await client.GetChannelAsync(config.ChannelId);
            } catch (Exception)
            {
                return;
            }

            DiscordEmbedBuilder embed = predicate(client, args);

            embed.WithFooter(embed.Footer + $" | {config.GuildAuditLogEvent.ToString()}");

            StringBuilder rolePings = new StringBuilder();
            foreach (ulong role in config.PingRoles)
            {
                rolePings.Append($"<@&{role}> ");
            }

            try
            {
                await channel.SendMessageAsync(rolePings.ToString(), embed);
            } catch (Exception)
            {
                return;
            }
        }
    }
}
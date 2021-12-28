using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using System.Text;

namespace MASZ.GuildAuditLog
{
    public class GuildAuditLogger
    {
        private readonly ILogger<GuildAuditLogger> _logger;
        private readonly IDiscordClient _client;
        private readonly ulong _guildId;
        private readonly IServiceProvider _serviceProvider;

        private GuildAuditLogger(IDiscordClient client, IServiceProvider serviceProvider, ulong guildId)
        {
            _client = client;
            _guildId = guildId;
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<GuildAuditLogger>>();
        }

        public static GuildAuditLogger CreateDefault(IDiscordClient client, IServiceProvider serviceProvider, ulong guildId)
        {
            return new GuildAuditLogger(client, serviceProvider, guildId);
        }

        public static EmbedBuilder GenerateBaseEmbed(Color color)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(color);
            embed.WithTimestamp(DateTime.Now);
            return embed;
        }

        public async Task HandleEvent(EmbedBuilder embed, GuildAuditLogEvent eventType)
        {
            var guildConfigRepository = GuildConfigRepository.CreateDefault(_serviceProvider);
            try
            {
                GuildConfig guildConfig = await guildConfigRepository.GetGuildConfig(_guildId);
                if (guildConfig == null)
                {
                    return;
                }
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

            var auditLogRepository = GuildLevelAuditLogConfigRepository.CreateWithBotIdentity(_serviceProvider);
            GuildLevelAuditLogConfig auditLogConfig = null;
            try
            {
                auditLogConfig = await auditLogRepository.GetConfigsByGuildAndType(_guildId, eventType);
                if (auditLogConfig == null)
                {
                    return;
                }
            }
            catch (ResourceNotFoundException)
            {
                return;
            }

            ITextChannel channel = null;
            try
            {
                channel = await _client.GetChannelAsync(auditLogConfig.ChannelId) as ITextChannel;
            }
            catch (Exception)
            {
                return;
            }

            if (embed.Footer == null)
            {
                embed.WithFooter(auditLogConfig.GuildAuditLogEvent.ToString());
            }
            else
            {
                embed.WithFooter(embed.Footer.Text + $" | {auditLogConfig.GuildAuditLogEvent}");
            }

            StringBuilder rolePings = new();
            foreach (ulong role in auditLogConfig.PingRoles)
            {
                rolePings.Append($"<@&{role}> ");
            }

            try
            {
                await channel.SendMessageAsync(rolePings.ToString(), embed: embed.Build());
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
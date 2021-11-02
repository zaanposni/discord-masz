using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using masz.Services;
using Microsoft.Extensions.Logging;

namespace masz.GuildAuditLog
{
    public class GuildAuditLogger
    {
        private readonly ILogger<GuildAuditLogger> _logger;
        private readonly DiscordClient _client;
        private readonly ulong _guildId;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITranslator _translator;
        private readonly IDatabase _database;

        private GuildAuditLogger(DiscordClient client, IServiceProvider serviceProvider, ulong guildId)
        {
            _client = client;
            _guildId = guildId;
            _serviceProvider = serviceProvider;
            _logger = (ILogger<GuildAuditLogger>) _serviceProvider.GetService(typeof(ILogger<GuildAuditLogger>));
            _translator = (ITranslator) _serviceProvider.GetService(typeof(ITranslator));
        }

        public static GuildAuditLogger CreateDefault(DiscordClient client, IServiceProvider serviceProvider, ulong guildId)
        {
            return new GuildAuditLogger(client, serviceProvider, guildId);
        }
        public static DiscordEmbedBuilder GenerateBaseEmbed(DiscordColor color)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(color);
            embed.WithTimestamp(DateTime.Now);
            return embed;
        }

        public async Task HandleEvent<T>(T args, Func<DiscordClient, T, ITranslator, DiscordEmbedBuilder> predicate, GuildAuditLogEvent eventType) where T : DiscordEventArgs
        {
            var guildConfigRepository = GuildConfigRepository.CreateDefault(_serviceProvider);
            try
            {
                GuildConfig guildConfig = await guildConfigRepository.GetGuildConfig(_guildId);
                if (guildConfig == null)
                {
                    return;
                }
            } catch (ResourceNotFoundException)
            {
                return;
            }

            var auditLogRepository = GuildLevelAuditLogConfigRepository.CreateDefault(_serviceProvider);
            GuildLevelAuditLogConfig auditLogConfig = null;
            try
            {
                auditLogConfig = await auditLogRepository.GetConfigsByGuildAndType(_guildId, eventType);
                if (auditLogConfig == null)
                {
                    return;
                }
            } catch (ResourceNotFoundException)
            {
                return;
            }

            DiscordChannel channel = null;
            try
            {
                channel = await _client.GetChannelAsync(auditLogConfig.ChannelId);
            } catch (Exception)
            {
                return;
            }

            await _translator.SetContext(_guildId);

            DiscordEmbedBuilder embed = predicate(_client, args, _translator);

            if (embed.Footer == null)
            {
                embed.WithFooter(auditLogConfig.GuildAuditLogEvent.ToString());
            } else
            {
                embed.WithFooter(embed.Footer.Text + $" | {auditLogConfig.GuildAuditLogEvent.ToString()}");
            }

            StringBuilder rolePings = new StringBuilder();
            foreach (ulong role in auditLogConfig.PingRoles)
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
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using masz.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using masz.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using masz.Commands;
using masz.Exceptions;
using System.Net.WebSockets;

namespace masz.Services
{
    public class DiscordBot : IDiscordBot
    {
        private readonly ILogger<DiscordClient> _logger;
        private readonly IInternalConfiguration _config;
        private readonly ITranslator _translator;
        private readonly DiscordClient _client;
        private DiscordConfiguration _discordConfiguration;
        private IServiceProvider _serviceProvider;
        private bool _isRunning = false;
        private DateTime? _lastDisconnect = null;

        public DiscordBot(ILogger<DiscordClient> logger, IInternalConfiguration config, ITranslator translator, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = config;
            _translator = translator;
            _serviceProvider = serviceProvider;

            _discordConfiguration = new DiscordConfiguration()
            {
                Token = _config.GetBotToken(),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers
            };

            _client = new DiscordClient(_discordConfiguration);
            _client.MessageCreated += this.MessageCreatedHandler;
            _client.MessageUpdated += this.MessageUpdatedHandler;
            _client.GuildCreated += this.GuildCreatedHandler;
            _client.GuildMemberAdded += this.GuildMemberAddedHandler;
            _client.InviteCreated += this.InviteCreatedHandler;
            _client.InviteDeleted += this.InviteDeletedHandler;

            _client.SocketErrored += this.SocketErroredHandler;
            _client.Resumed += this.ResumedHandler;
            _client.Ready += this.ReadyHandler;

            var slash = _client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = serviceProvider
            });

            ulong? debugGuild = 748943581523345639;  // set your guild id here to enable fast syncing debug commands

            slash.RegisterCommands<PingCommand>(debugGuild);
            slash.RegisterCommands<ReportCommand>(debugGuild);

            slash.SlashCommandErrored += CmdErroredHandler;
        }

        public async Task Start()
        {
            DiscordActivity activity = new DiscordActivity(_config.GetBaseUrl(), ActivityType.Watching);
            await _client.ConnectAsync(activity);
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public DateTime? GetLastDisconnectTime()
        {
            return _lastDisconnect;
        }

        private Task ResumedHandler(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.LogWarning("Client reconnected.");
            _isRunning = true;
            return Task.CompletedTask;
        }

        private Task SocketErroredHandler(DiscordClient sender, SocketErrorEventArgs e)
        {
            if (e.Exception is WebSocketException)
            {
                _logger.LogCritical("Client disconnected.");
                _isRunning = false;
                _lastDisconnect = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }

        private Task ReadyHandler(DiscordClient sender, ReadyEventArgs e)
        {
            _logger.LogInformation("Client connected.");
            _isRunning = true;
            return Task.CompletedTask;
        }

        private Task MessageCreatedHandler(DiscordClient client, MessageCreateEventArgs e)
        {
            // TODO: automod
            return Task.CompletedTask;
        }

        private Task MessageUpdatedHandler(DiscordClient client, MessageUpdateEventArgs e)
        {
            // TODO: automod
            return Task.CompletedTask;
        }

        private Task GuildCreatedHandler(DiscordClient client, GuildCreateEventArgs e)
        {
            _logger.LogInformation($"I joined guild '{e.Guild.Name}' with ID: '{e.Guild.Id}'");
            return Task.CompletedTask;
        }

        private Task GuildMemberAddedHandler(DiscordClient client, GuildMemberAddEventArgs e)
        {
            // TODO: punishment check
            // TODO: invite handling
            return Task.CompletedTask;
        }

        private Task InviteCreatedHandler(DiscordClient sender, InviteCreateEventArgs e)
        {
            // TODO: invite handling
            return Task.CompletedTask;
        }

        private Task InviteDeletedHandler(DiscordClient sender, InviteDeleteEventArgs e)
        {
            // TODO: invite handling
            return Task.CompletedTask;
        }

        private async Task CmdErroredHandler(SlashCommandsExtension _, SlashCommandErrorEventArgs e)
        {
            if (e.Exception is BaseAPIException)
            {
                _logger.LogError($"Command '{e.Context.CommandName}' invoked by '{e.Context.User.Username}#{e.Context.User.Discriminator}' failed: {(e.Exception as BaseAPIException).error}");
                var response = new DiscordInteractionResponseBuilder();
                response.WithContent((e.Exception as BaseAPIException).error.ToString());
                response.IsEphemeral = true;
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            } else
            {
                _logger.LogError($"Command '{e.Context.CommandName}' invoked by '{e.Context.User.Username}#{e.Context.User.Discriminator}' failed: " + e.Exception.Message);
            }
        }

        public DiscordClient GetClient()
        {
            return _client;
        }
    }
}
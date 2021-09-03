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

namespace masz.Services
{
    public class DiscordBot : IDiscordBot
    {
        private readonly ILogger<DiscordClient> logger;
        private readonly IOptions<InternalConfig> config;
        private readonly IDatabase context;
        private readonly ITranslator translator;
        private IConfiguration configuration;
        private readonly DiscordClient client;
        private DiscordConfiguration discordConfiguration;
        private IServiceProvider serviceProvider;

        public DiscordBot(ILogger<DiscordClient> logger, IOptions<InternalConfig> config, IDatabase context, ITranslator translator, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.config = config;
            this.context = context;
            this.translator = translator;
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;

            this.discordConfiguration = new DiscordConfiguration()
            {
                Token = this.config.Value.DiscordBotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers
            };

            this.client = new DiscordClient(this.discordConfiguration);
            this.client.MessageCreated += this.MessageCreatedHandler;

            var slash = client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = serviceProvider
            });

            slash.RegisterCommands<PingCommand>(748943581523345639);

            slash.SlashCommandErrored += CmdErroredHandler;
        }

        public async Task Start()
        {
            DiscordActivity activity = new DiscordActivity(this.translator.T().Description());
            await this.client.ConnectAsync(activity);
        }

        public async Task MessageCreatedHandler(DiscordClient client, MessageCreateEventArgs e)
        {
            if (e.Author != client.CurrentUser) {
                await e.Channel.SendMessageAsync("test");
            }
        }

        private async Task CmdErroredHandler(SlashCommandsExtension _, SlashCommandErrorEventArgs e)
        {
            this.logger.LogError(e.Exception.ToString());
        }
    }
}
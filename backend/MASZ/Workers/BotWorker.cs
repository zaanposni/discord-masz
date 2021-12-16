using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MASZ.Services;
using System.Reflection;

namespace MASZ.Workers
{
    public class BotWorker : IHostedService
    {

        private readonly DiscordSocketClient _client;
        private readonly InternalConfiguration _config;
        private readonly InteractionService _interaction;
        private readonly IServiceProvider _services;
        private readonly ILogger<BotWorker> _logger;

        public BotWorker (DiscordSocketClient client, InteractionService interaction,
            InternalConfiguration config, IServiceProvider services, ILogger<BotWorker> logger)
        {
            _client = client;
            _config = config;
            _interaction = interaction;
            _services = services;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                try
                {
                    await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Modules could not initialize!");
                    return;
                }
            }

            await _client.LoginAsync(TokenType.Bot, _config.GetBotToken());
            await _client.StartAsync();
            await _client.SetGameAsync(_config.GetBaseUrl(), type: ActivityType.Watching);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
        }

    }
}

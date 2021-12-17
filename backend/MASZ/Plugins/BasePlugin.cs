using MASZ.Data;
using MASZ.Services;

namespace MASZ.Plugins
{
    public class BasePlugin
    {
        protected readonly Database _database;
        protected readonly IdentityManager _identityManager;
        protected readonly InternalConfiguration _config;
        protected readonly DiscordAPIInterface _discordAPI;
        protected readonly DiscordBot _discordBot;
        protected readonly Scheduler _scheduler;
        protected readonly InternalEventHandler _eventHandler;
        protected readonly IServiceProvider _serviceProvider;

        public BasePlugin() { }

        public BasePlugin(IServiceProvider serviceProvider)
        {
            _database = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<Database>();

            _identityManager = serviceProvider.GetRequiredService<IdentityManager>();
            _config = serviceProvider.GetRequiredService<InternalConfiguration>();
            _discordAPI = serviceProvider.GetRequiredService<DiscordAPIInterface>();
            _discordBot = serviceProvider.GetRequiredService<DiscordBot>();
            _scheduler = serviceProvider.GetRequiredService<Scheduler>();
            _eventHandler = serviceProvider.GetRequiredService<InternalEventHandler>();
            _serviceProvider = serviceProvider;
        }
    }
}
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
        protected readonly EventHandler _eventHandler;
        protected readonly IServiceProvider _serviceProvider;

        public BasePlugin() { }

        public BasePlugin(IServiceProvider serviceProvider)
        {
            _database = (Database)serviceProvider.GetRequiredService(typeof(Database));
            _identityManager = (IdentityManager)serviceProvider.GetRequiredService(typeof(IdentityManager));
            _config = (InternalConfiguration)serviceProvider.GetRequiredService(typeof(InternalConfiguration));
            _discordAPI = (DiscordAPIInterface)serviceProvider.GetRequiredService(typeof(DiscordAPIInterface));
            _discordBot = (DiscordBot)serviceProvider.GetRequiredService(typeof(DiscordBot));
            _scheduler = (Scheduler)serviceProvider.GetRequiredService(typeof(Scheduler));
            _eventHandler = (EventHandler)serviceProvider.GetRequiredService(typeof(EventHandler));
            _serviceProvider = serviceProvider;
        }
    }
}
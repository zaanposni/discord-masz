using MASZ.Services;

namespace MASZ.Plugins
{
    public class BasePlugin
    {
        protected readonly IDatabase _database;
        protected readonly IIdentityManager _identityManager;
        protected readonly IInternalConfiguration _config;
        protected readonly IDiscordAPIInterface _discordAPI;
        protected readonly IDiscordBot _discordBot;
        protected readonly IScheduler _scheduler;
        protected readonly IEventHandler _eventHandler;
        protected readonly IServiceProvider _serviceProvider;

        public BasePlugin() { }

        public BasePlugin(IServiceProvider serviceProvider)
        {
            _database = (IDatabase)serviceProvider.GetService(typeof(IDatabase));
            _identityManager = (IIdentityManager)serviceProvider.GetService(typeof(IIdentityManager));
            _config = (IInternalConfiguration)serviceProvider.GetService(typeof(IInternalConfiguration));
            _discordAPI = (IDiscordAPIInterface)serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _discordBot = (IDiscordBot)serviceProvider.GetService(typeof(IDiscordBot));
            _scheduler = (IScheduler)serviceProvider.GetService(typeof(IScheduler));
            _eventHandler = (IEventHandler)serviceProvider.GetService(typeof(IEventHandler));
            _serviceProvider = serviceProvider;
        }
    }
}
using System;
using masz.Models;
using masz.Services;
using Microsoft.Extensions.Options;

namespace masz.Plugins
{
    public class BasePlugin
    {
        protected readonly IDatabase _database;
        protected readonly IIdentityManager _identityManager;
        protected readonly IOptions<InternalConfig> _config;
        protected readonly IDiscordAPIInterface _discordAPI;
        protected readonly IDiscordBot _discordBot;
        protected readonly IScheduler _scheduler;
        protected readonly IServiceProvider _serviceProvider;

        public BasePlugin() { }

        public BasePlugin(IServiceProvider serviceProvider)
        {
            _database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            _identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            _config = (IOptions<InternalConfig>) serviceProvider.GetService(typeof(IOptions<InternalConfig>));
            _discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _discordBot = (IDiscordBot) serviceProvider.GetService(typeof(IDiscordBot));
            _scheduler = (IScheduler) serviceProvider.GetService(typeof(IScheduler));
            _serviceProvider = serviceProvider;
        }
    }
}
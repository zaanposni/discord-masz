using System;
using masz.Models;
using masz.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Repositories
{

    public class BaseRepository<T>
    {

        protected ILogger<T> _logger { get; set; }
        protected IDatabase _database { get; set; }
        protected IDiscordAPIInterface _discordAPI { get; set; }
        protected readonly IOptions<InternalConfig> _config;
        protected readonly IIdentityManager _identityManager;
        protected readonly IDiscordAnnouncer _discordAnnouncer;
        protected readonly IFilesHandler _filesHandler;
        protected readonly IPunishmentHandler _punishmentHandler;
        protected readonly IScheduler _scheduler;
        protected readonly ITranslator _translator;
        protected readonly IServiceProvider _serviceProvider;
        public BaseRepository(IServiceProvider serviceProvider)
        {
            this._logger = (ILogger<T>) serviceProvider.GetService(typeof(ILogger<T>));
            this._database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            this._discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            this._config = (IOptions<InternalConfig>) serviceProvider.GetService(typeof(IOptions<InternalConfig>));
            this._identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            this._discordAnnouncer = (IDiscordAnnouncer) serviceProvider.GetService(typeof(IDiscordAnnouncer));
            this._filesHandler = (IFilesHandler) serviceProvider.GetService(typeof(IFilesHandler));
            this._punishmentHandler = (IPunishmentHandler) serviceProvider.GetService(typeof(IPunishmentHandler));
            this._scheduler = (IScheduler) serviceProvider.GetService(typeof(IScheduler));
            this._translator = (ITranslator) serviceProvider.GetService(typeof(ITranslator));
            this._serviceProvider = serviceProvider;
        }
    }
}
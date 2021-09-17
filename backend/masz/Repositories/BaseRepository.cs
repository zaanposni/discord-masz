using System;
using System.Linq;
using DSharpPlus.Entities;
using masz.Models;
using masz.Models.Views;
using masz.Services;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class BaseRepository<T>
    {

        protected ILogger<T> _logger { get; set; }
        protected IDatabase _database { get; set; }
        protected IDiscordAPIInterface _discordAPI { get; set; }
        protected readonly IInternalConfiguration _config;
        protected readonly IIdentityManager _identityManager;
        protected readonly IDiscordAnnouncer _discordAnnouncer;
        protected readonly IFilesHandler _filesHandler;
        protected readonly IPunishmentHandler _punishmentHandler;
        protected readonly IScheduler _scheduler;
        protected readonly ITranslator _translator;
        protected readonly IDiscordBot _discordBot;
        protected readonly IEventHandler _eventHandler;
        protected readonly IServiceProvider _serviceProvider;
        public BaseRepository(IServiceProvider serviceProvider)
        {
            this._logger = (ILogger<T>) serviceProvider.GetService(typeof(ILogger<T>));
            this._database = (IDatabase) serviceProvider.GetService(typeof(IDatabase));
            this._discordAPI = (IDiscordAPIInterface) serviceProvider.GetService(typeof(IDiscordAPIInterface));
            this._config = (IInternalConfiguration) serviceProvider.GetService(typeof(IInternalConfiguration));
            this._identityManager = (IIdentityManager) serviceProvider.GetService(typeof(IIdentityManager));
            this._discordAnnouncer = (IDiscordAnnouncer) serviceProvider.GetService(typeof(IDiscordAnnouncer));
            this._filesHandler = (IFilesHandler) serviceProvider.GetService(typeof(IFilesHandler));
            this._punishmentHandler = (IPunishmentHandler) serviceProvider.GetService(typeof(IPunishmentHandler));
            this._scheduler = (IScheduler) serviceProvider.GetService(typeof(IScheduler));
            this._translator = (ITranslator) serviceProvider.GetService(typeof(ITranslator));
            this._discordBot = (IDiscordBot) serviceProvider.GetService(typeof(IDiscordBot));
            this._eventHandler = (IEventHandler) serviceProvider.GetService(typeof(IEventHandler));
            this._serviceProvider = serviceProvider;
        }

        protected bool contains(ModCaseTableEntry obj, string search)
        {
            if (obj == null) return false;
            return contains(obj.ModCase, search) ||
                   contains(obj.Moderator, search) ||
                   contains(obj.Suspect, search);
        }

        protected bool contains(AutoModerationEventExpandedView obj, string search)
        {
            if (obj == null) return false;
            return contains(obj.AutoModerationEvent, search) ||
                   contains(obj.Suspect, search);
        }

        protected bool contains(ModCase obj, string search)
        {
            if (obj == null) return false;
            return contains(obj.Title, search) ||
                   contains(obj.Description, search) ||
                   contains(obj.GetPunishment(_translator), search) ||
                   contains(obj.Username, search) ||
                   contains(obj.Discriminator, search) ||
                   contains(obj.Nickname, search) ||
                   contains(obj.UserId, search) ||
                   contains(obj.ModId, search) ||
                   contains(obj.LastEditedByModId, search) ||
                   contains(obj.CreatedAt, search) ||
                   contains(obj.OccuredAt, search) ||
                   contains(obj.LastEditedAt, search) ||
                   contains(obj.Labels, search) ||
                   contains(obj.CaseId.ToString(), search) ||
                   contains($"#{obj.CaseId}", search);
        }

        protected bool contains(CaseView obj, string search)
        {
            if (obj == null) return false;
            return contains(obj.Title, search) ||
                   contains(obj.Description, search) ||
                   contains(obj.GetPunishment(_translator), search) ||
                   contains(obj.Username, search) ||
                   contains(obj.Discriminator, search) ||
                   contains(obj.Nickname, search) ||
                   contains(obj.UserId, search) ||
                   contains(obj.ModId, search) ||
                   contains(obj.LastEditedByModId, search) ||
                   contains(obj.CreatedAt, search) ||
                   contains(obj.OccuredAt, search) ||
                   contains(obj.LastEditedAt, search) ||
                   contains(obj.Labels, search) ||
                   contains(obj.CaseId.ToString(), search) ||
                   contains($"#{obj.CaseId}", search);
        }

        protected bool contains(AutoModerationEventView obj, string search)
        {
            if (obj == null) return false;
            return contains(obj.AutoModerationAction.ToString(), search) ||
                   contains(obj.AutoModerationType.ToString(), search) ||
                   contains(obj.CreatedAt, search) ||
                   contains(obj.Discriminator, search) ||
                   contains(obj.Username, search) ||
                   contains(obj.Nickname, search) ||
                   contains(obj.UserId, search) ||
                   contains(obj.MessageContent, search) ||
                   contains(obj.MessageId, search);
        }

        protected bool contains(string obj, string search)
        {
            if (String.IsNullOrWhiteSpace(obj)) return false;
            return obj.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        protected bool contains(ulong obj, string search)
        {
            return contains(obj.ToString(), search);
        }

        protected bool contains(DateTime obj, string search)
        {
            if (obj == null) return false;
            return contains(obj.ToString(), search);
        }

        protected bool contains(string[] obj, string search)
        {
            if (obj == null) return false;
            return obj.Any(x => contains(x, search));
        }

        protected bool contains(DiscordUser obj, string search)
        {
            if (obj == null) return false;
            return contains($"{obj.Username}#{obj.Discriminator}", search);
        }

        protected bool contains(DiscordUserView obj, string search)
        {
            if (obj == null) return false;
            return contains($"{obj.Username}#{obj.Discriminator}", search);
        }
    }
}
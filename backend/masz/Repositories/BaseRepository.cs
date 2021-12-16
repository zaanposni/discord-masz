using Discord;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Services;

namespace MASZ.Repositories
{

    public class BaseRepository<T>
    {

        protected ILogger<T> Logger { get; set; }
        protected IDatabase Database { get; set; }
        protected IDiscordAPIInterface DiscordAPI { get; set; }

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
            Logger = (ILogger<T>)serviceProvider.GetService(typeof(ILogger<T>));
            Database = (IDatabase)serviceProvider.GetService(typeof(IDatabase));
            DiscordAPI = (IDiscordAPIInterface)serviceProvider.GetService(typeof(IDiscordAPIInterface));
            _config = (IInternalConfiguration)serviceProvider.GetService(typeof(IInternalConfiguration));
            _identityManager = (IIdentityManager)serviceProvider.GetService(typeof(IIdentityManager));
            _discordAnnouncer = (IDiscordAnnouncer)serviceProvider.GetService(typeof(IDiscordAnnouncer));
            _filesHandler = (IFilesHandler)serviceProvider.GetService(typeof(IFilesHandler));
            _punishmentHandler = (IPunishmentHandler)serviceProvider.GetService(typeof(IPunishmentHandler));
            _scheduler = (IScheduler)serviceProvider.GetService(typeof(IScheduler));
            _translator = (ITranslator)serviceProvider.GetService(typeof(ITranslator));
            _discordBot = (IDiscordBot)serviceProvider.GetService(typeof(IDiscordBot));
            _eventHandler = (IEventHandler)serviceProvider.GetService(typeof(IEventHandler));
            _serviceProvider = serviceProvider;
        }

        protected bool Contains(ModCaseTableEntry obj, string search)
        {
            if (obj == null) return false;
            return Contains(obj.ModCase, search) ||
                   Contains(obj.Moderator, search) ||
                   Contains(obj.Suspect, search);
        }

        protected bool Contains(AutoModerationEventExpandedView obj, string search)
        {
            if (obj == null) return false;
            return Contains(obj.AutoModerationEvent, search) ||
                   Contains(obj.Suspect, search);
        }

        protected bool Contains(ModCase obj, string search)
        {
            if (obj == null) return false;
            return Contains(obj.Title, search) ||
                   Contains(obj.Description, search) ||
                   Contains(obj.GetPunishment(_translator), search) ||
                   Contains(obj.Username, search) ||
                   Contains(obj.Discriminator, search) ||
                   Contains(obj.Nickname, search) ||
                   Contains(obj.UserId, search) ||
                   Contains(obj.ModId, search) ||
                   Contains(obj.LastEditedByModId, search) ||
                   Contains(obj.CreatedAt, search) ||
                   Contains(obj.OccuredAt, search) ||
                   Contains(obj.LastEditedAt, search) ||
                   Contains(obj.Labels, search) ||
                   Contains(obj.CaseId.ToString(), search) ||
                   Contains($"#{obj.CaseId}", search);
        }

        protected bool Contains(CaseView obj, string search)
        {
            if (obj == null) return false;
            return Contains(obj.Title, search) ||
                   Contains(obj.Description, search) ||
                   Contains(obj.GetPunishment(_translator), search) ||
                   Contains(obj.Username, search) ||
                   Contains(obj.Discriminator, search) ||
                   Contains(obj.Nickname, search) ||
                   Contains(obj.UserId, search) ||
                   Contains(obj.ModId, search) ||
                   Contains(obj.LastEditedByModId, search) ||
                   Contains(obj.CreatedAt, search) ||
                   Contains(obj.OccuredAt, search) ||
                   Contains(obj.LastEditedAt, search) ||
                   Contains(obj.Labels, search) ||
                   Contains(obj.CaseId.ToString(), search) ||
                   Contains($"#{obj.CaseId}", search);
        }

        protected bool Contains(AutoModerationEventView obj, string search)
        {
            if (obj == null) return false;
            return Contains(obj.AutoModerationAction.ToString(), search) ||
                   Contains(obj.AutoModerationType.ToString(), search) ||
                   Contains(obj.CreatedAt, search) ||
                   Contains(obj.Discriminator, search) ||
                   Contains(obj.Username, search) ||
                   Contains(obj.Nickname, search) ||
                   Contains(obj.UserId, search) ||
                   Contains(obj.MessageContent, search) ||
                   Contains(obj.MessageId, search);
        }

        protected bool Contains(string obj, string search)
        {
            if (string.IsNullOrWhiteSpace(obj)) return false;
            return obj.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        protected bool Contains(ulong obj, string search)
        {
            return Contains(obj.ToString(), search);
        }

        protected bool Contains(DateTime obj, string search)
        {
            if (obj == default) return false;
            return Contains(obj.ToString(), search);
        }

        protected bool Contains(string[] obj, string search)
        {
            if (obj == null) return false;
            return obj.Any(x => Contains(x, search));
        }

        protected bool Contains(IUser obj, string search)
        {
            if (obj == null) return false;
            return Contains($"{obj.Username}#{obj.Discriminator}", search);
        }

        protected bool Contains(DiscordUserView obj, string search)
        {
            if (obj == null) return false;
            return Contains($"{obj.Username}#{obj.Discriminator}", search);
        }
    }
}
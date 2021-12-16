using Discord;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Services;

namespace MASZ.Repositories
{

    public class BaseRepository<T>
    {

        protected ILogger<T> Logger { get; set; }
        protected Database Database { get; set; }
        protected DiscordAPIInterface DiscordAPI { get; set; }

        protected readonly InternalConfiguration _config;
        protected readonly IdentityManager _identityManager;
        protected readonly DiscordAnnouncer _discordAnnouncer;
        protected readonly FilesHandler _filesHandler;
        protected readonly PunishmentHandler _punishmentHandler;
        protected readonly Scheduler _scheduler;
        protected readonly Translator _translator;
        protected readonly DiscordBot _discordBot;
        protected readonly DiscordEventHandler _eventHandler;
        protected readonly IServiceProvider _serviceProvider;
        public BaseRepository(IServiceProvider serviceProvider)
        {
            Logger = (ILogger<T>)serviceProvider.GetRequiredService(typeof(ILogger<T>));
            Database = (Database)serviceProvider.GetRequiredService(typeof(Database));
            DiscordAPI = (DiscordAPIInterface)serviceProvider.GetRequiredService(typeof(DiscordAPIInterface));
            _config = (InternalConfiguration)serviceProvider.GetRequiredService(typeof(InternalConfiguration));
            _identityManager = (IdentityManager)serviceProvider.GetRequiredService(typeof(IdentityManager));
            _discordAnnouncer = (DiscordAnnouncer)serviceProvider.GetRequiredService(typeof(DiscordAnnouncer));
            _filesHandler = (FilesHandler)serviceProvider.GetRequiredService(typeof(FilesHandler));
            _punishmentHandler = (PunishmentHandler)serviceProvider.GetRequiredService(typeof(PunishmentHandler));
            _scheduler = (Scheduler)serviceProvider.GetRequiredService(typeof(Scheduler));
            _translator = (Translator)serviceProvider.GetRequiredService(typeof(Translator));
            _discordBot = (DiscordBot)serviceProvider.GetRequiredService(typeof(DiscordBot));
            _eventHandler = (DiscordEventHandler)serviceProvider.GetRequiredService(typeof(DiscordEventHandler));
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
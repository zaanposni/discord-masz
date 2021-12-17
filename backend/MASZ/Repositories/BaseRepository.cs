using Discord;
using Discord.WebSocket;
using MASZ.Data;
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
        protected readonly Punishments _punishmentHandler;
        protected readonly Scheduler _scheduler;
        protected readonly Translator _translator;
        protected readonly DiscordBot _discordBot;
        protected readonly DiscordSocketClient _client;
        protected readonly InternalEventHandler _eventHandler;
        protected readonly IServiceProvider _serviceProvider;

        public BaseRepository(IServiceProvider serviceProvider)
        {
            Logger = serviceProvider.GetRequiredService<ILogger<T>>();
            Database = serviceProvider.GetRequiredService<Database>();
            DiscordAPI = serviceProvider.GetRequiredService<DiscordAPIInterface>();

            _config = serviceProvider.GetRequiredService<InternalConfiguration>();
            _identityManager = serviceProvider.GetRequiredService<IdentityManager>();
            _discordAnnouncer = serviceProvider.GetRequiredService<DiscordAnnouncer>();
            _filesHandler = serviceProvider.GetRequiredService<FilesHandler>();
            _punishmentHandler = serviceProvider.GetRequiredService<Punishments>();
            _scheduler = serviceProvider.GetRequiredService<Scheduler>();
            _translator = serviceProvider.GetRequiredService<Translator>();
            _discordBot = serviceProvider.GetRequiredService<DiscordBot>();
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            _eventHandler = serviceProvider.GetRequiredService<InternalEventHandler>();
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
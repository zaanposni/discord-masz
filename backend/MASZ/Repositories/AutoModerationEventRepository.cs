using Discord;
using MASZ.Enums;
using MASZ.Extensions;
using MASZ.Models;
using System.Text;

namespace MASZ.Repositories
{

    public class AutoModerationEventRepository : BaseRepository<AutoModerationEventRepository>
    {
        private AutoModerationEventRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static AutoModerationEventRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<int> CountEvents()
        {
            return await Database.CountAllModerationEvents();
        }
        public async Task<int> CountEventsByGuild(ulong guildId)
        {
            return await Database.CountAllModerationEventsForGuild(guildId);
        }
        public async Task<int> CountEventsByGuildAndUser(ulong guildId, ulong userId)
        {
            return await Database.CountAllModerationEventsForSpecificUserOnGuild(guildId, userId);
        }
        public async Task<AutoModerationEvent> RegisterEvent(AutoModerationEvent modEvent, ITextChannel channel, IUser author)
        {
            GuildConfig guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modEvent.GuildId);

            _translator.SetContext(guildConfig);
            AutoModerationConfig modConfig = await AutoModerationConfigRepository.CreateWithBotIdentity(_serviceProvider).GetConfigsByGuildAndType(modEvent.GuildId, modEvent.AutoModerationType);

            IUser user = await DiscordAPI.FetchUserInfo(modEvent.UserId, CacheBehavior.Default);
            if (user != null)
            {
                modEvent.Username = user.Username;
                modEvent.Discriminator = user.Discriminator;
            }

            modEvent.CreatedAt = DateTime.UtcNow;

            if (modConfig.AutoModerationAction == AutoModerationAction.CaseCreated || modConfig.AutoModerationAction == AutoModerationAction.ContentDeletedAndCaseCreated)
            {
                ModCase modCase = new()
                {
                    Title = $"{_translator.T().Automoderation()}: {_translator.T().Enum(modEvent.AutoModerationType)}"
                };

                StringBuilder description = new();
                description.AppendLine(_translator.T().NotificationAutomoderationCase(user));
                description.AppendLine(_translator.T().Type() + ": " + _translator.T().Enum(modEvent.AutoModerationType));
                description.AppendLine(_translator.T().Action() + ": " + _translator.T().Enum(modEvent.AutoModerationAction));
                description.AppendLine(_translator.T().Message() + ": " + modEvent.MessageId.ToString());
                description.AppendLine(_translator.T().MessageContent() + ": " + modEvent.MessageContent);

                modCase.Description = description.ToString();

                modCase.Labels = new List<string>() { "automoderation", modEvent.AutoModerationType.ToString() }.ToArray();
                modCase.CreationType = CaseCreationType.AutoModeration;
                modCase.PunishmentType = PunishmentType.Warn;
                modCase.PunishedUntil = null;
                if (modConfig.PunishmentType != null)
                {
                    modCase.PunishmentType = modConfig.PunishmentType.Value;
                }
                if (modConfig.PunishmentDurationMinutes != null)
                {
                    modCase.PunishedUntil = DateTime.UtcNow.AddMinutes(modConfig.PunishmentDurationMinutes.Value);
                }
                modCase.UserId = modEvent.UserId;
                modCase.GuildId = modEvent.GuildId;

                try
                {
                    modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).CreateModCase(modCase, true, modConfig.SendPublicNotification, modConfig.SendDmNotification);

                    modEvent.AssociatedCaseId = modCase.CaseId;
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Failed to create modcase for modevent {modEvent.GuildId}/{modEvent.UserId}/{modEvent.AutoModerationType}");
                }
            }

            await Database.SaveModerationEvent(modEvent);
            await Database.SaveChangesAsync();

            if (modConfig.AutoModerationAction == AutoModerationAction.Timeout && modConfig.PunishmentDurationMinutes.HasValue)
            {
                string reason = _translator.T().NotificationDiscordAuditLogPunishmentsExecuteAutomod(_translator.T().Enum(modEvent.AutoModerationType));
                DateTime until = DateTime.UtcNow.AddMinutes(modConfig.PunishmentDurationMinutes.Value);
                await DiscordAPI.TimeoutGuildUser(modEvent.GuildId, modEvent.UserId, until, reason);
            }

            _eventHandler.OnAutoModerationEventRegisteredEvent.InvokeAsync(modEvent, modConfig, guildConfig, channel, author);

            return modEvent;
        }
        public async Task DeleteEventsForGuild(ulong guildId)
        {
            await Database.DeleteAllModerationEventsForGuild(guildId);
            await Database.SaveChangesAsync();
        }
        public async Task<List<AutoModerationEvent>> GetPagination(ulong guildId, int startPage = 1, int pageSize = 20)
        {
            return await Database.SelectAllModerationEventsForGuild(guildId, startPage, pageSize);
        }
        public async Task<List<AutoModerationEvent>> GetPaginationFilteredForUser(ulong guildId, ulong userId, int startPage = 1, int pageSize = 20)
        {
            return await Database.SelectAllModerationEventsForSpecificUserOnGuild(guildId, userId, startPage, pageSize);
        }
        public async Task<List<AutoModerationEvent>> GetAllEventsForUser(ulong userId)
        {
            return await Database.SelectAllModerationEventsForSpecificUser(userId);
        }
        public async Task<List<AutoModerationEvent>> GetAllEventsForUserAndGuild(ulong guildId, ulong userId)
        {
            return await Database.SelectAllModerationEventsForSpecificUserAndGuild(guildId, userId);
        }
        public async Task<List<AutoModerationEvent>> GetAllEventsForGuild(ulong guildId)
        {
            return await Database.SelectAllModerationEventsForSpecificGuild(guildId);
        }
        public async Task<List<AutoModerationEvent>> GetAllEventsForUserSinceMinutes(ulong userId, int minutes)
        {
            return await Database.SelectAllModerationEventsForSpecificUser(userId, minutes);
        }
        public async Task<List<DbCount>> GetCounts(ulong guildId, DateTime since)
        {
            return await Database.GetModerationCountGraph(guildId, since);
        }
        public async Task<List<AutoModerationTypeSplit>> GetCountsByType(ulong guildId)
        {
            return await Database.GetModerationSplitGraph(guildId);
        }
        public async Task<List<AutoModerationEvent>> SearchInGuild(ulong guildId, string searchString)
        {
            List<AutoModerationEvent> events = await Database.SelectAllModerationEventsForGuild(guildId);
            List<AutoModerationEvent> filteredEvents = new();
            foreach (var c in events)
            {
                var entry = new AutoModerationEventExpandedView(
                    c,
                    await DiscordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (Contains(entry, searchString))
                {
                    filteredEvents.Add(c);
                }
            }
            return filteredEvents;
        }
    }
}
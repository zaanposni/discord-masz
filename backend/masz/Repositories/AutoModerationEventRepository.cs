using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Events;
using masz.Models;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class AutoModerationEventRepository : BaseRepository<AutoModerationEventRepository>
    {
        private AutoModerationEventRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public static AutoModerationEventRepository CreateDefault(IServiceProvider serviceProvider) => new AutoModerationEventRepository(serviceProvider);

        public async Task<int> CountEvents()
        {
            return await _database.CountAllModerationEvents();
        }
        public async Task<int> CountEventsByGuild(ulong guildId)
        {
            return await _database.CountAllModerationEventsForGuild(guildId);
        }
        public async Task<int> CountEventsByGuildAndUser(ulong guildId, ulong userId)
        {
            return await _database.CountAllModerationEventsForSpecificUserOnGuild(guildId, userId);
        }
        public async Task<AutoModerationEvent> RegisterEvent(AutoModerationEvent modEvent)
        {
            AutoModerationConfig modConfig = await AutoModerationConfigRepository.CreateDefault(_serviceProvider).GetConfigsByGuildAndType(modEvent.GuildId, modEvent.AutoModerationType);

            DiscordUser user = await _discordAPI.FetchUserInfo(modEvent.UserId, CacheBehavior.Default);
            if (user != null)
            {
                modEvent.Username = user.Username;
                modEvent.Discriminator = user.Discriminator;
            }

            modEvent.CreatedAt = DateTime.UtcNow;

            if (modConfig.AutoModerationAction == AutoModerationAction.CaseCreated || modConfig.AutoModerationAction == AutoModerationAction.CaseCreated)
            {
                ModCase modCase = new ModCase();
                modCase.Title = $"AutoModeration: {modEvent.AutoModerationType.ToString()}";
                modCase.Description = $"User triggered AutoModeration\nEvent: {modEvent.AutoModerationType.ToString()}\nAction: {modConfig.AutoModerationAction.ToString()}\nMessageId: {modEvent.MessageId}\nMessage content: {modEvent.MessageContent}";
                modCase.Labels = new List<string>() { "automoderation", modEvent.AutoModerationType.ToString() }.ToArray();
                modCase.CreationType = CaseCreationType.AutoModeration;
                modCase.PunishmentType = PunishmentType.None;
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

                    modEvent.AssociatedCaseId = modCase.Id;
                } catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to create modcase for modevent {modEvent.GuildId}/{modEvent.UserId}/{modEvent.AutoModerationType}");
                }
            }

            await _database.SaveModerationEvent(modEvent);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeAutoModerationEventRegistered(new AutoModerationEventRegisteredEventArgs(modEvent));

            return modEvent;
        }
        public async Task DeleteEventsForGuild(ulong guildId)
        {
            await _database.DeleteAllModerationEventsForGuild(guildId);
            await _database.SaveChangesAsync();
        }
        public async Task<List<AutoModerationEvent>> GetPagination(ulong guildId, int startPage = 1, int pageSize = 20)
        {
            return await _database.SelectAllModerationEventsForGuild(guildId, startPage, pageSize);
        }
        public async Task<List<AutoModerationEvent>> GetPaginationFilteredForUser(ulong guildId, ulong userId, int startPage = 1, int pageSize = 20)
        {
            return await _database.SelectAllModerationEventsForSpecificUserOnGuild(guildId, userId, startPage, pageSize);
        }
        public async Task<List<AutoModerationEvent>> GetAllEventsForUser(ulong userId)
        {
            return await _database.SelectAllModerationEventsForSpecificUser(userId);
        }
        public async Task<List<DbCount>> GetCounts(ulong guildId, DateTime since)
        {
            return await _database.GetModerationCountGraph(guildId, since);
        }
        public async Task<List<AutoModerationTypeSplit>> GetCountsByType(ulong guildId, DateTime since)
        {
            return await _database.GetModerationSplitGraph(guildId, since);
        }
        public async Task<List<AutoModerationEvent>> SearchInGuild(ulong guildId, string searchString)
        {
            List<AutoModerationEvent> events = await _database.SelectAllModerationEventsForGuild(guildId);
            List<AutoModerationEvent> filteredEvents = new List<AutoModerationEvent>();
            foreach (var c in events)
            {
                var entry = new AutoModerationEventExpandedView(
                    c,
                    await _discordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (contains(entry, searchString)) {
                    filteredEvents.Add(c);
                }
            }
            return filteredEvents;
        }
    }
}
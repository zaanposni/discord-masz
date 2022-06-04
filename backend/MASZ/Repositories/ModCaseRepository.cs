using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class ModCaseRepository : BaseRepository<ModCaseRepository>
    {
        private readonly IUser _currentUser;
        private ModCaseRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private ModCaseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }
        public static ModCaseRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static ModCaseRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<LabelUsage>> GetLabelUsages(ulong guildId)
        {
            var labels = await Database.GetAllLabels(guildId);

            Dictionary<string, int> countMap = new();

            foreach (string label in labels)
            {
                if (countMap.ContainsKey(label))
                {
                    countMap[label]++;
                }
                else
                {
                    countMap[label] = 1;
                }
            }

            List<LabelUsage> result = new();
            foreach (string label in countMap.Keys)
            {
                result.Add(new LabelUsage()
                {
                    Label = label,
                    Count = countMap[label]
                });
            }

            return result.OrderByDescending(x => x.Count).ToList();
        }
        public async Task<ModCase> ImportModCase(ModCase modCase)
        {
            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(modCase.GuildId);
            }

            modCase.CreationType = CaseCreationType.Imported;
            modCase.CaseId = await Database.GetHighestCaseIdForGuild(modCase.GuildId) + 1;
            modCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt == default || modCase.OccuredAt == DateTime.MinValue)
            {
                modCase.OccuredAt = modCase.CreatedAt;
            }
            modCase.ModId = _currentUser.Id;
            modCase.LastEditedAt = modCase.CreatedAt;
            modCase.LastEditedByModId = _currentUser.Id;
            if (modCase.Labels != null)
            {
                modCase.Labels = modCase.Labels.Distinct().ToArray();
            }
            else
            {
                modCase.Labels = Array.Empty<string>();
            }
            modCase.Valid = true;
            if (modCase.PunishmentType == PunishmentType.Warn || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            }
            else
            {
                modCase.PunishmentActive = modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow;
            }

            await Database.SaveModCase(modCase);
            await Database.SaveChangesAsync();

            return modCase;
        }
        public async Task<ModCase> CreateModCase(ModCase modCase, bool handlePunishment, bool sendPublicNotification, bool sendDmNotification)
        {
            IUser currentReportedUser = await DiscordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(modCase.GuildId);
            }

            if (currentReportedUser == null)
            {
                Logger.LogError("Failed to fetch modcase suspect.");
                throw new InvalidIUserException(modCase.ModId);
            }
            if (currentReportedUser.IsBot)
            {
                Logger.LogError("Cannot create cases for bots.");
                throw new ProtectedModCaseSuspectException("Cannot create cases for bots.", modCase).WithError(APIError.ProtectedModCaseSuspectIsBot);
            }
            if (_config.GetSiteAdmins().Contains(currentReportedUser.Id))
            {
                Logger.LogInformation("Cannot create cases for site admins.");
                throw new ProtectedModCaseSuspectException("Cannot create cases for site admins.", modCase).WithError(APIError.ProtectedModCaseSuspectIsSiteAdmin);
            }

            modCase.Username = currentReportedUser.Username;
            modCase.Discriminator = currentReportedUser.Discriminator;

            IGuildUser currentReportedMember = await DiscordAPI.FetchMemberInfo(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            if (currentReportedMember != null)
            {
                if (currentReportedMember.RoleIds.Where(x => guildConfig.ModRoles.Contains(x)).Any() ||
                    currentReportedMember.RoleIds.Where(x => guildConfig.AdminRoles.Contains(x)).Any())
                {
                    Logger.LogInformation("Cannot create cases for team members.");
                    throw new ProtectedModCaseSuspectException("Cannot create cases for team members.", modCase).WithError(APIError.ProtectedModCaseSuspectIsTeam);
                }
                modCase.Nickname = currentReportedMember.Nickname;
            }

            modCase.CaseId = await Database.GetHighestCaseIdForGuild(modCase.GuildId) + 1;
            modCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt == default || modCase.OccuredAt == DateTime.MinValue)
            {
                modCase.OccuredAt = modCase.CreatedAt;
            }
            modCase.ModId = _currentUser.Id;
            modCase.LastEditedAt = modCase.CreatedAt;
            modCase.LastEditedByModId = _currentUser.Id;
            if (modCase.Labels != null)
            {
                modCase.Labels = modCase.Labels.Distinct().ToArray();
            }
            else
            {
                modCase.Labels = Array.Empty<string>();
            }
            modCase.Valid = true;
            if (modCase.PunishmentType == PunishmentType.Warn || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            }
            else
            {
                modCase.PunishmentActive = modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow;
            }

            await Database.SaveModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseCreatedEvent.InvokeAsync(modCase, _currentUser, sendPublicNotification, sendDmNotification);

            if (handlePunishment && (modCase.PunishmentActive || modCase.PunishmentType == PunishmentType.Kick))
            {
                if (modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow)
                {
                    await _punishmentHandler.ExecutePunishment(modCase);
                }
            }

            return modCase;
        }
        public async Task<ModCase> GetModCase(ulong guildId, int caseId)
        {
            ModCase modCase = await Database.SelectSpecificModCase(guildId, caseId);
            if (modCase == null)
            {
                throw new ResourceNotFoundException($"ModCase with id {caseId} does not exist.");
            }
            return modCase;
        }
        public async Task<ModCase> DeleteModCase(ulong guildId, int caseId, bool forceDelete = false, bool handlePunishment = true, bool announcePublic = true)
        {
            ModCase modCase = await GetModCase(guildId, caseId);

            if (forceDelete)
            {
                try
                {
                    _filesHandler.DeleteDirectory(Path.Combine(_config.GetFileUploadPath(), guildId.ToString(), caseId.ToString()));
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Failed to delete files directory for modcase {guildId}/{caseId}.");
                }

                Logger.LogInformation($"Force deleting modCase {guildId}/{caseId}.");
                Database.DeleteSpecificModCase(modCase);
                await Database.SaveChangesAsync();

                _eventHandler.OnModCaseDeletedEvent.InvokeAsync(modCase, _currentUser, announcePublic, false);
            }
            else
            {
                modCase.MarkedToDeleteAt = DateTime.UtcNow.AddDays(7);
                modCase.DeletedByUserId = _currentUser.Id;
                modCase.PunishmentActive = false;

                Logger.LogInformation($"Marking modcase {guildId}/{caseId} as deleted.");
                Database.UpdateModCase(modCase);
                await Database.SaveChangesAsync();

                _eventHandler.OnModCaseMarkedToBeDeletedEvent.InvokeAsync(modCase, _currentUser, announcePublic, false);
            }

            if (handlePunishment)
            {
                try
                {
                    Logger.LogInformation($"Handling punishment for case {guildId}/{caseId}.");
                    await _punishmentHandler.UndoPunishment(modCase);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Failed to handle punishment for modcase {guildId}/{caseId}.");
                }
            }
            return modCase;
        }
        public async Task<ModCase> UpdateModCase(ModCase modCase, bool handlePunishment, bool sendPublicNotification)
        {
            IUser currentReportedUser = await DiscordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);
            }
            catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(modCase.GuildId);
            }

            if (currentReportedUser == null)
            {
                Logger.LogError("Failed to fetch modcase suspect.");
                throw new InvalidIUserException(modCase.ModId);
            }
            if (currentReportedUser.IsBot)
            {
                Logger.LogError("Cannot edit cases for bots.");
                throw new ProtectedModCaseSuspectException("Cannot edit cases for bots.", modCase).WithError(APIError.ProtectedModCaseSuspectIsBot);
            }
            if (_config.GetSiteAdmins().Contains(currentReportedUser.Id))
            {
                Logger.LogInformation("Cannot edit cases for site admins.");
                throw new ProtectedModCaseSuspectException("Cannot edit cases for site admins.", modCase).WithError(APIError.ProtectedModCaseSuspectIsSiteAdmin);
            }

            modCase.Username = currentReportedUser.Username;
            modCase.Discriminator = currentReportedUser.Discriminator;

            IGuildUser currentReportedMember = await DiscordAPI.FetchMemberInfo(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            if (currentReportedMember != null)
            {
                if (currentReportedMember.RoleIds.Where(x => guildConfig.ModRoles.Contains(x)).Any() ||
                    currentReportedMember.RoleIds.Where(x => guildConfig.AdminRoles.Contains(x)).Any())
                {
                    Logger.LogInformation("Cannot create cases for team members.");
                    throw new ProtectedModCaseSuspectException("Cannot create cases for team members.", modCase).WithError(APIError.ProtectedModCaseSuspectIsTeam);
                }
                modCase.Nickname = currentReportedMember.Nickname;
            }

            modCase.LastEditedAt = DateTime.UtcNow;
            modCase.LastEditedByModId = _currentUser.Id;
            modCase.Valid = true;
            if (modCase.PunishmentType == PunishmentType.Warn || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            }
            else
            {
                modCase.PunishmentActive = modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow;
            }

            Database.UpdateModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseUpdatedEvent.InvokeAsync(modCase, _currentUser, sendPublicNotification, false);

            if (handlePunishment && (modCase.PunishmentActive || modCase.PunishmentType == PunishmentType.Kick))
            {
                if (modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow)
                {
                    await _punishmentHandler.ExecutePunishment(modCase);
                }
            }
            return modCase;
        }
        public async Task<List<ModCase>> GetCasePagination(ulong guildId, int startPage = 1, int pageSize = 20)
        {
            return await Database.SelectAllModCasesForGuild(guildId, startPage, pageSize);
        }
        public async Task<List<ModCase>> GetCasePaginationFilteredForUser(ulong guildId, ulong userId, int startPage = 1, int pageSize = 20)
        {
            return await Database.SelectAllModcasesForSpecificUserOnGuild(guildId, userId, startPage, pageSize);
        }
        public async Task<List<ModCase>> GetCasesForUser(ulong userId)
        {
            return await Database.SelectAllModCasesForSpecificUser(userId);
        }
        public async Task<List<ModCase>> GetCasesForGuild(ulong guildId)
        {
            return await Database.SelectAllModCasesForGuild(guildId);
        }
        public async Task<List<ModCase>> GetCasesForGuildAndUser(ulong guildId, ulong userId)
        {
            return await Database.SelectAllModcasesForSpecificUserOnGuild(guildId, userId);
        }
        public async Task<int> CountAllCases()
        {
            return await Database.CountAllModCases();
        }
        public async Task<int> CountAllCasesForGuild(ulong guildId)
        {
            return await Database.CountAllModCasesForGuild(guildId);
        }
        public async Task<int> CountAllPunishmentsForGuild(ulong guildId)
        {
            return await Database.CountAllActivePunishmentsForGuild(guildId);
        }
        public async Task<int> CountAllPunishmentsForGuild(ulong guildId, PunishmentType type)
        {
            return await Database.CountAllPunishmentsForGuild(guildId, type);
        }
        public async Task<int> CountAllActiveMutesForGuild(ulong guildId)
        {
            return await Database.CountAllActivePunishmentsForGuild(guildId, PunishmentType.Mute);
        }
        public async Task<int> CountAllActiveBansForGuild(ulong guildId)
        {
            return await Database.CountAllActivePunishmentsForGuild(guildId, PunishmentType.Ban);
        }
        public async Task<List<ModCase>> SearchCases(ulong guildId, string searchString, int limit = 10)
        {
            List<ModCase> modCases = await Database.SelectAllModCasesForGuild(guildId);
            List<ModCase> filteredModCases = new();
            foreach (var c in modCases)
            {
                if (filteredModCases.Count >= limit)
                {
                    break;
                }
                var entry = new ModCaseTableEntry(
                    c,
                    await DiscordAPI.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache),
                    await DiscordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (Contains(entry, searchString))
                {
                    filteredModCases.Add(c);
                }
            }
            return filteredModCases;
        }
        public async Task<List<ModCase>> SearchCasesFilteredForUser(ulong guildId, ulong userId, string searchString)
        {
            List<ModCase> modCases = await Database.SelectAllModcasesForSpecificUserOnGuild(guildId, userId);
            List<ModCase> filteredModCases = new();
            foreach (var c in modCases)
            {
                var entry = new ModCaseTableEntry(
                    c,
                    await DiscordAPI.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache),
                    await DiscordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (Contains(entry, searchString))
                {
                    filteredModCases.Add(c);
                }
            }
            return filteredModCases;
        }
        public async Task<ModCase> LockCaseComments(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.AllowComments = false;
            modCase.LockedAt = DateTime.UtcNow;
            modCase.LockedByUserId = _currentUser.Id;

            Database.UpdateModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseUpdatedEvent.InvokeAsync(modCase, _currentUser, false, false);

            return modCase;
        }
        public async Task<ModCase> UnlockCaseComments(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.AllowComments = true;
            modCase.LockedAt = null;
            modCase.LockedByUserId = 0;

            Database.UpdateModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseUpdatedEvent.InvokeAsync(modCase, _currentUser, false, false);

            return modCase;
        }
        public async Task<ModCase> RestoreCase(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.MarkedToDeleteAt = null;
            modCase.DeletedByUserId = 0;
            if (modCase.PunishmentType == PunishmentType.Warn || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishmentActive = false;
            }
            else
            {
                modCase.PunishmentActive = modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow;
            }

            Database.UpdateModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseRestoredEvent.InvokeAsync(modCase);

            try
            {
                Logger.LogInformation($"Handling punishment for case {guildId}/{caseId}.");
                await _punishmentHandler.ExecutePunishment(modCase);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Failed to handle punishment for modcase {guildId}/{caseId}.");
            }

            return modCase;
        }
        public async Task<List<CaseCount>> GetCounts(ulong guildId, DateTime since)
        {
            return await Database.GetCaseCountGraph(guildId, since);
        }
        public async Task<List<ModeratorCaseCount>> GetModeratorCasesCount(ulong guildId)
        {
            return await Database.GetModeratorCaseCountGraph(guildId);
        }
        public async Task<ModCase> ActivateModCase(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.PunishmentActive = true;
            modCase.LastEditedAt = DateTime.UtcNow;
            modCase.LastEditedByModId = _currentUser.Id;

            Database.UpdateModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseUpdatedEvent.InvokeAsync(modCase, _currentUser, false, false);

            try
            {
                Logger.LogInformation($"Handling punishment for case {guildId}/{caseId}.");
                await _punishmentHandler.ExecutePunishment(modCase);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Failed to handle punishment for modcase {guildId}/{caseId}.");
            }

            return modCase;
        }
        public async Task<ModCase> DeactivateModCase(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.PunishmentActive = false;
            modCase.LastEditedAt = DateTime.UtcNow;
            modCase.LastEditedByModId = _currentUser.Id;

            Database.UpdateModCase(modCase);
            await Database.SaveChangesAsync();

            _eventHandler.OnModCaseUpdatedEvent.InvokeAsync(modCase, _currentUser, false, false);

            try
            {
                Logger.LogInformation($"Handling punishment for case {guildId}/{caseId}.");
                await _punishmentHandler.UndoPunishment(modCase);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Failed to handle punishment for modcase {guildId}/{caseId}.");
            }

            return modCase;
        }

        public async Task DeactivateModCase(params ModCase[] modCases)
        {
            foreach (ModCase modCase in modCases)
            {
                modCase.PunishmentActive = false;
                modCase.LastEditedAt = DateTime.UtcNow;
                modCase.LastEditedByModId = _currentUser.Id;

                Database.UpdateModCase(modCase);
                await Database.SaveChangesAsync();

                _eventHandler.OnModCaseUpdatedEvent.InvokeAsync(modCase, _currentUser, false, false);

                try
                {
                    Logger.LogInformation($"Handling punishment for case {modCase.GuildId}/{modCase.CaseId}.");
                    await _punishmentHandler.UndoPunishment(modCase);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Failed to handle punishment for modcase {modCase.GuildId}/{modCase.CaseId}.");
                }
            }
        }

        public async Task LinkCases(ulong guildId, int caseAId, int caseBId)
        {
            ModCase caseA = await GetModCase(guildId, caseAId);
            ModCase caseB = await GetModCase(guildId, caseBId);

            ModCaseMapping existing = await Database.GetModCaseMapping(caseA.Id, caseB.Id);
            if (existing != null)
            {
                throw new BaseAPIException("Cases are already linked.");
            }

            existing = await Database.GetModCaseMapping(caseB.Id, caseA.Id);
            if (existing != null)
            {
                throw new BaseAPIException("Cases are already linked.");
            }

            ModCaseMapping mapping = new ModCaseMapping()
            {
                CaseA = caseA,
                CaseB = caseB
            };

            Database.CreateModCaseMapping(mapping);
            await Database.SaveChangesAsync();
        }

        public async Task UnlinkCases(ulong guildId, int caseAId, int caseBId)
        {
            ModCase caseA = await GetModCase(guildId, caseAId);
            ModCase caseB = await GetModCase(guildId, caseBId);

            ModCaseMapping mapping = await Database.GetModCaseMapping(caseA.Id, caseB.Id);

            if (mapping != null)
            {
                Database.DeleteModCaseMapping(mapping);
                await Database.SaveChangesAsync();
                return;
            }

            mapping = await Database.GetModCaseMapping(caseB.Id, caseA.Id);
            if (mapping == null)
            {
                throw new BaseAPIException("Cases are not linked.");
            }

            Database.DeleteModCaseMapping(mapping);
            await Database.SaveChangesAsync();
        }
    }
}
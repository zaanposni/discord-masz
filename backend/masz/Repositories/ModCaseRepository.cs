using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class ModCaseRepository : BaseRepository<ModCaseRepository>
    {
        private readonly Identity _identity;
        private ModCaseRepository(IServiceProvider serviceProvider, Identity identity) : base(serviceProvider)
        {
            _identity = identity;
        }

        public static ModCaseRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new ModCaseRepository(serviceProvider, identity);
        public async Task<ModCase> CreateModCase(ModCase modCase, bool handlePunishment, bool sendPublicNotification, bool sendDmNotification)
        {
            DiscordUser moderator = await _discordAPI.FetchUserInfo(modCase.ModId, CacheBehavior.Default);
            DiscordUser currentReportedUser = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            GuildConfig guildConfig = await _database.SelectSpecificGuildConfig(modCase.GuildId);

            if (guildConfig == null)
            {
                _logger.LogError("Guild is not registered.");
                throw new UnregisteredGuildException(modCase.GuildId);
            }

            if (currentReportedUser == null)
            {
                _logger.LogError("Failed to fetch modcase suspect.");
                throw new InvalidDiscordUserException(modCase.ModId);
            }
            if (currentReportedUser.IsBot)
            {
                _logger.LogError("Cannot create cases for bots.");
                throw new ProtectedModCaseSuspectException("Cannot create cases for bots.", modCase).WithError(APIError.ProtectedModCaseSuspectIsBot);
            }
            if (_config.Value.SiteAdminDiscordUserIds.Contains(currentReportedUser.Id))
            {
                _logger.LogInformation("Cannot create cases for site admins.");
                throw new ProtectedModCaseSuspectException("Cannot create cases for site admins.", modCase).WithError(APIError.ProtectedModCaseSuspectIsSiteAdmin);
            }
            // TODO: uncomment this ?!
            // if (! await HasPermissionToExecutePunishment(modCase.GuildId, modCase.PunishmentType))
            // {
            //     _logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized - Missing discord permissions.");
            //     return Unauthorized("Missing discord permissions. Strict permissions enabled.");
            // }

            modCase.Username = currentReportedUser.Username;
            modCase.Discriminator = currentReportedUser.Discriminator;

            DiscordMember currentReportedMember = await _discordAPI.FetchMemberInfo(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            if (currentReportedMember != null)
            {
                if (currentReportedMember.Roles.Where(x => guildConfig.ModRoles.Contains(x.Id.ToString())).Any() ||
                    currentReportedMember.Roles.Where(x => guildConfig.AdminRoles.Contains(x.Id.ToString())).Any())
                {
                    _logger.LogInformation("Cannot create cases for team members.");
                    throw new ProtectedModCaseSuspectException("Cannot create cases for team members.", modCase).WithError(APIError.ProtectedModCaseSuspectIsTeam);
                }
                modCase.Nickname = currentReportedMember.Nickname;
            }

            modCase.CaseId = await _database.GetHighestCaseIdForGuild(modCase.GuildId) + 1;
            modCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt == null)
            {
                modCase.OccuredAt = modCase.CreatedAt;
            } else
            {
                modCase.OccuredAt = modCase.CreatedAt;
            }
            modCase.LastEditedAt = modCase.CreatedAt;
            modCase.Labels = modCase.Labels.Distinct().ToArray();
            modCase.Valid = true;
            if (modCase.PunishmentType == PunishmentType.None || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            } else
            {
                modCase.PunishmentActive = modCase.PunishedUntil > modCase.CreatedAt;
            }

            await _database.SaveModCase(modCase);
            await _database.SaveChangesAsync();

            await _discordAnnouncer.AnnounceModCase(modCase, RestAction.Created, moderator, sendPublicNotification, sendDmNotification);

            if (handlePunishment && (modCase.PunishmentActive || modCase.PunishmentType != PunishmentType.Kick))
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
            return await _database.SelectSpecificModCase(guildId, caseId);
        }
        public async Task DeleteModCase(ulong guildId, int caseId, bool forceDelete=false)
        {
            ModCase modCase = await this.GetModCase(guildId, caseId);

            if (modCase == null)
            {
                _logger.LogError($"ModCase with id {caseId} does not exist.");
                throw new ResourceNotFoundException($"ModCase with id {caseId} does not exist.");
            }

            if (forceDelete)
            {
                try {
                    _filesHandler.DeleteDirectory(Path.Combine(_config.Value.AbsolutePathToFileUpload, guildId.ToString(), caseId.ToString()));
                } catch (Exception e) {
                    _logger.LogError(e, $"Failed to delete files directory for modcase {guildId}/{caseId}.");
                }

                _logger.LogInformation($"Force deleting modCase {guildId}/{caseId}.");
                _database.DeleteSpecificModCase(modCase);
                await _database.SaveChangesAsync();
            } else {
                modCase.MarkedToDeleteAt = DateTime.UtcNow.AddDays(7);
                modCase.DeletedByUserId = _identity.GetCurrentUser().Id;
                modCase.PunishmentActive = false;

                _logger.LogInformation($"Marking modcase {guildId}/{caseId} as deleted.");
                _database.UpdateModCase(modCase);
                await _database.SaveChangesAsync();
            }
        }
    }
}
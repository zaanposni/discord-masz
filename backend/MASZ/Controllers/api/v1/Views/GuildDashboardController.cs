using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/dashboard")]
    [Authorize]
    public class GuildDashbordController : SimpleController
    {
        private static readonly DateTime epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public GuildDashbordController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("chart")]
        public async Task<IActionResult> GetModCaseGrid([FromRoute] ulong guildId, [FromQuery] long? since = null)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            DateTime sinceTime = DateTime.UtcNow.AddYears(-1);
            if (since != null)
            {
                sinceTime = epoch.AddSeconds(since.Value);
            }

            ModCaseRepository modCaseRepo = ModCaseRepository.CreateDefault(_serviceProvider, identity);
            AutoModerationEventRepository automodRepo = AutoModerationEventRepository.CreateDefault(_serviceProvider);
            AppealRepository appealRepo = AppealRepository.CreateDefault(_serviceProvider);

            return Ok(new
            {
                modCases = await modCaseRepo.GetCounts(guildId, sinceTime),
                punishments = await modCaseRepo.GetPunishmentCounts(guildId, sinceTime),
                appeals = await appealRepo.GetCounts(guildId, sinceTime),
                autoModerations = await automodRepo.GetCounts(guildId, sinceTime)
            });
        }

        [HttpGet("automodchart")]
        public async Task<IActionResult> GetAutomodSplitChart([FromRoute] ulong guildId, [FromQuery] long? since = null)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);

            DateTime sinceTime = DateTime.UtcNow.AddYears(-1);
            if (since != null)
            {
                sinceTime = epoch.AddSeconds(since.Value);
            }

            AutoModerationEventRepository automodRepo = AutoModerationEventRepository.CreateDefault(_serviceProvider);

            return Ok(await automodRepo.GetCountsByType(guildId, sinceTime));
        }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            ModCaseRepository modCaseRepo = ModCaseRepository.CreateDefault(_serviceProvider, identity);
            int modCases = await modCaseRepo.CountAllCasesForGuild(guildId);
            int activePunishments = await modCaseRepo.CountAllPunishmentsForGuild(guildId);
            int allBans = await modCaseRepo.CountAllPunishmentsForGuild(guildId, PunishmentType.Ban);
            int activeBans = await modCaseRepo.CountAllActiveBansForGuild(guildId);
            int allMutes = await modCaseRepo.CountAllPunishmentsForGuild(guildId, PunishmentType.Mute);
            int activeMutes = await modCaseRepo.CountAllActiveMutesForGuild(guildId);
            int allKicks = await modCaseRepo.CountAllPunishmentsForGuild(guildId, PunishmentType.Kick);
            int allWarns = await modCaseRepo.CountAllPunishmentsForGuild(guildId, PunishmentType.Warn);
            int autoModerations = await AutoModerationEventRepository.CreateDefault(_serviceProvider).CountEventsByGuild(guildId);
            int trackedInvites = await InviteRepository.CreateDefault(_serviceProvider).CountInvitesForGuild(guildId);
            int userMappings = await UserMapRepository.CreateDefault(_serviceProvider, identity).CountAllUserMapsByGuild(guildId);
            int userNotes = await UserNoteRepository.CreateDefault(_serviceProvider, identity).CountUserNotesForGuild(guildId);
            int comments = await ModCaseCommentRepository.CreateDefault(_serviceProvider, identity).CountCommentsByGuild(guildId);
            int appeals = await AppealRepository.CreateDefault(_serviceProvider).CountAppeals(guildId);

            return Ok(new
            {
                caseCount = modCases,
                activeCount = activePunishments,
                banCount = allBans,
                activeBanCount = activeBans,
                muteCount = allMutes,
                activeMuteCount = activeMutes,
                kickCount = allKicks,
                warnCount = allWarns,
                moderationCount = autoModerations,
                trackedInvites,
                userMappings,
                userNotes,
                comments,
                appeals
            });
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> DashboardNotifications([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            bool appealConfigured = await AppealStructureRepository.CreateDefault(_serviceProvider).ConfiguredForGuild(guildId);

            return Ok(new
            {
                appealConfigured
            });
        }


        [HttpGet("latestcomments")]
        public async Task<IActionResult> LatestComments([FromRoute] ulong guildId)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            List<CommentExpandedView> view = new();
            foreach (ModCaseComment comment in await ModCaseCommentRepository.CreateDefault(_serviceProvider, identity).GetLastCommentsByGuild(guildId))
            {
                view.Add(new CommentExpandedTableView(
                    comment,
                    await _discordAPI.FetchUserInfo(comment.UserId, CacheBehavior.OnlyCache),
                    guildId,
                    comment.ModCase.CaseId
                ));
            }

            return Ok(view);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromRoute] ulong guildId, [FromQuery] string search)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();

            if (string.IsNullOrWhiteSpace(search))
            {
                return Ok(new List<string>());
            }

            List<IQuickSearchEntry> entries = new();

            foreach (ModCase item in await ModCaseRepository.CreateDefault(_serviceProvider, identity).SearchCases(guildId, search))
            {
                entries.Add(new QuickSearchEntry<CaseExpandedView>
                {
                    Entry = new CaseExpandedView(
                        item,
                        await _discordAPI.FetchUserInfo(item.ModId, CacheBehavior.OnlyCache),
                        await _discordAPI.FetchUserInfo(item.LastEditedByModId, CacheBehavior.OnlyCache),
                        await _discordAPI.FetchUserInfo(item.UserId, CacheBehavior.OnlyCache),
                        new List<CommentExpandedView>(),
                        null
                    ),
                    CreatedAt = item.CreatedAt,
                    QuickSearchEntryType = QuickSearchEntryType.ModCase
                });
            }

            foreach (AutoModerationEvent item in await AutoModerationEventRepository.CreateDefault(_serviceProvider).SearchInGuild(guildId, search))
            {
                entries.Add(new QuickSearchEntry<AutoModerationEventExpandedView>
                {
                    Entry = new AutoModerationEventExpandedView(
                        item,
                        await _discordAPI.FetchUserInfo(item.UserId, CacheBehavior.OnlyCache)
                    ),
                    CreatedAt = item.CreatedAt,
                    QuickSearchEntryType = QuickSearchEntryType.AutoModeration
                });
            }


            UserNoteExpandedView userNote = null;
            try
            {
                ulong userId = ulong.Parse(search);
                UserNote note = await UserNoteRepository.CreateDefault(_serviceProvider, identity).GetUserNote(guildId, userId);
                userNote = new UserNoteExpandedView(
                    note,
                    await _discordAPI.FetchUserInfo(note.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(note.CreatorId, CacheBehavior.OnlyCache)
                );
            }
            catch (ResourceNotFoundException) { }
            catch (FormatException) { }
            catch (ArgumentException) { }
            catch (OverflowException) { }

            List<UserMappingExpandedView> userMappingViews = new();
            try
            {
                ulong userId = ulong.Parse(search);
                List<UserMapping> userMappings = await UserMapRepository.CreateDefault(_serviceProvider, identity).GetUserMapsByGuildAndUser(guildId, userId);
                foreach (UserMapping userMapping in userMappings)
                {
                    userMappingViews.Add(new UserMappingExpandedView(
                        userMapping,
                        await _discordAPI.FetchUserInfo(userMapping.UserA, CacheBehavior.OnlyCache),
                        await _discordAPI.FetchUserInfo(userMapping.UserB, CacheBehavior.OnlyCache),
                        await _discordAPI.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.OnlyCache)
                    ));
                }
            }
            catch (ResourceNotFoundException) { }
            catch (FormatException) { }
            catch (ArgumentException) { }
            catch (OverflowException) { }

            return Ok(new
            {
                searchEntries = entries.OrderByDescending(x => x.CreatedAt).ToList(),
                userNoteView = userNote,
                userMappingViews
            });
        }
    }
}
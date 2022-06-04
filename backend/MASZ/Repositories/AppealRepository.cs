using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class AppealRepository : BaseRepository<AppealRepository>
    {
        private AppealRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public static AppealRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<int> CountAppeals()
        {
            return await Database.GetAppealCount();
        }
        public async Task<int> CountAppeals(ulong guildId)
        {
            return await Database.GetAppealCount(guildId);
        }
        public async Task<Appeal> GetById(int id)
        {
            Appeal appeal = await Database.GetAppeal(id);
            if (appeal == null)
            {
                throw new ResourceNotFoundException($"Appeal {id} not found");
            }
            return appeal;
        }
        public async Task<List<Appeal>> GetForGuild(ulong guildId)
        {
            return await Database.GetAppeals(guildId);
        }
        public async Task<Appeal> Create(Appeal appeal, List<AppealAnswer> answers)
        {
            appeal.CreatedAt = DateTime.UtcNow;
            appeal.UpdatedAt = appeal.CreatedAt;
            appeal.InvalidDueToLaterRejoinAt = null;
            appeal.UserCanCreateNewAppeals = null;
            appeal.Status = AppealStatus.Pending;

            Database.SaveAppeal(appeal);
            await Database.SaveChangesAsync();

            AppealAnswerRepository answerRepository = AppealAnswerRepository.CreateDefault(_serviceProvider);
            foreach (AppealAnswer answer in answers)
            {
                answer.Appeal = appeal;
                await answerRepository.Create(answer);
            }

            _eventHandler.OnAppealCreatedEvent.InvokeAsync(appeal, await DiscordAPI.FetchUserInfo(appeal.UserId, CacheBehavior.OnlyCache));

            return appeal;
        }
        public async Task<Appeal> Update(Appeal appeal)
        {
            appeal.UpdatedAt = DateTime.UtcNow;

            Database.UpdateAppeal(appeal);
            await Database.SaveChangesAsync();

            _eventHandler.OnAppealUpdatedEvent.InvokeAsync(
                appeal,
                await DiscordAPI.FetchUserInfo(appeal.LastModeratorId, CacheBehavior.OnlyCache),
                await DiscordAPI.FetchUserInfo(appeal.UserId, CacheBehavior.OnlyCache)
            );

            return appeal;
        }
        public async Task<bool> UserIsAllowedToCreateNewAppeal(ulong guildId, ulong userId)
        {
            GuildConfig config = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(guildId);

            ModCase latestBanModCase = (await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetCasesForGuildAndUser(guildId, userId))
                                        .Where(x => x.PunishmentType == PunishmentType.Ban && x.PunishmentActive)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .FirstOrDefault();

            if (latestBanModCase != null)
            {
                if (latestBanModCase.CreatedAt.AddDays(config.AllowBanAppealAfterDays) > DateTime.UtcNow)
                {
                    return false;
                }
            }

            IBan ban = await DiscordAPI.GetGuildUserBan(guildId, userId, CacheBehavior.IgnoreButCacheOnError);
            if (ban == null && latestBanModCase == null)
            {
                return false;
            }

            Appeal lastestAppeal = await Database.GetLatestAppealForUser(guildId, userId);
            if (lastestAppeal == null)
            {
                return true;
            }
            if (latestBanModCase != null)
            {
                if (lastestAppeal.CreatedAt < latestBanModCase.CreatedAt)
                {
                    return true;
                }
            }
            if (lastestAppeal.Status == AppealStatus.Pending)
            {
                return false;
            }
            if (lastestAppeal.Status == AppealStatus.Approved)
            {
                return true;
            }
            if (lastestAppeal.InvalidDueToLaterRejoinAt != null)
            {
                return true;
            }
            if (lastestAppeal.UserCanCreateNewAppeals == null)
            {
                return false;
            }
            return lastestAppeal.UserCanCreateNewAppeals.Value <= DateTime.UtcNow;
        }
        public async Task<List<AppealCount>> GetCounts(ulong guildId, DateTime since)
        {
            return await Database.GetAppealCount(guildId, since);
        }
        public async Task SetAllAppealsAsInvalid(ulong guildId, ulong userId)
        {
            List<Appeal> appeals = await Database.GetAppealsForUser(guildId, userId);
            foreach (Appeal appeal in appeals.Where(a => a.InvalidDueToLaterRejoinAt == null))
            {
                appeal.InvalidDueToLaterRejoinAt = DateTime.UtcNow;
                Database.UpdateAppeal(appeal);
            }
            await Database.SaveChangesAsync();
        }
        public async Task<bool> UserHasConfirmedAppeal(ulong guildId, ulong userId)
        {
            List<Appeal> appeals = await Database.GetAppealsForUser(guildId, userId);
            return appeals.Any(a => a.Status == AppealStatus.Approved && a.InvalidDueToLaterRejoinAt == null);
        }
        public async Task<bool> UserHasPendingOrDeclinedAppeal(ulong guildId, ulong userId)
        {
            List<Appeal> appeals = await Database.GetAppealsForUser(guildId, userId);
            return appeals.Any(a => (a.Status == AppealStatus.Pending || a.Status == AppealStatus.Declined) && a.InvalidDueToLaterRejoinAt == null);
        }
    }
}
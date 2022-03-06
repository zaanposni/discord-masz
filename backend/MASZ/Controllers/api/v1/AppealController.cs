using MASZ.Models;
using MASZ.Enums;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MASZ.Dtos.Appeal;
using MASZ.Exceptions;
using System.ComponentModel.DataAnnotations;
using Discord;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/appeal")]
    [Authorize]
    public class AppealController : SimpleController
    {
        private readonly ILogger<AppealController> _logger;
        public AppealController(IServiceProvider serviceProvider, ILogger<AppealController> logger) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("allowed")]
        public async Task<IActionResult> GetAppeal([FromRoute] ulong guildId)
        {
            Identity identity = await GetIdentity();
            IUser currentUser = identity.GetCurrentUser();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);

            return Ok(new { allowed = await AppealRepository.CreateDefault(_serviceProvider).UserIsAllowedToCreateNewAppeal(guildId, currentUser.Id) });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppeal([FromRoute] ulong guildId, [FromRoute] int id)
        {
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);
            IUser currentUser = identity.GetCurrentUser();
            bool isModOrHigher = await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);

            AppealRepository appealRepo = AppealRepository.CreateDefault(_serviceProvider);
            ModCaseRepository modCaseRepo = ModCaseRepository.CreateWithBotIdentity(_serviceProvider);

            Appeal appeal = await appealRepo.GetById(id);
            if (appeal.GuildId != guildId)
            {
                return BadRequest();
            }

            if (appeal.UserId != currentUser.Id && !isModOrHigher)
            {
                return Unauthorized();
            }

            List<ModCaseTableEntry> latestCases = new List<ModCaseTableEntry>();
            if (isModOrHigher)
            {
                List<ModCase> latestCasesRaw = (await modCaseRepo.GetCasesForGuildAndUser(guildId, appeal.UserId))
                    .Where(c => c.PunishmentType == PunishmentType.Ban)
                    .OrderByDescending(c => c.PunishmentActive ? 1 : 0)
                    .ThenByDescending(c => c.CreatedAt)
                    .Take(2)
                    .ToList();
                foreach (ModCase modCase in latestCasesRaw)
                {
                    latestCases.Add(new ModCaseTableEntry(
                        modCase,
                        await _discordAPI.FetchUserInfo(modCase.ModId, CacheBehavior.OnlyCache),
                        await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.OnlyCache)
                    ));
                }
            }

            // PUBLISH MOD MODE
            bool publishMod = guildConfig.PublishModeratorInfo || await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);
            return Ok(new AppealView(
                appeal,
                await _discordAPI.FetchUserInfo(appeal.UserId, CacheBehavior.OnlyCache),
                publishMod ? await _discordAPI.FetchUserInfo(appeal.LastModeratorId, CacheBehavior.OnlyCache) : null,
                await AppealAnswerRepository.CreateDefault(_serviceProvider).GetForAppeal(id),
                await AppealStructureRepository.CreateDefault(_serviceProvider).GetForGuild(guildId),
                latestCases
            ));
        }

        [HttpPost("table")]
        public async Task<IActionResult> GetAppeals([FromRoute] ulong guildId, [FromQuery][Range(0, int.MaxValue)] int page = 0, [FromBody] AppealFilterDto filter = null)
        {
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);

            ulong userOnly = 0;
            if (!await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                userOnly = identity.GetCurrentUser().Id;
            }

            // SELECT
            List<Appeal> appeals = await AppealRepository.CreateDefault(_serviceProvider).GetForGuild(guildId);

            // WHERE
            if (userOnly != 0)
            {
                appeals = appeals.Where(x => x.UserId == userOnly).ToList();
            }

            // PUBLISH MOD MODE
            bool publishMod = guildConfig.PublishModeratorInfo || await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);
            List<AppealView> tmp = new();
            foreach (var c in appeals)
            {
                var entry = new AppealView(
                    c,
                    await _discordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(c.LastModeratorId, CacheBehavior.OnlyCache),
                    await AppealAnswerRepository.CreateDefault(_serviceProvider).GetForAppeal(c.Id),
                    new List<AppealStructure>()
                );
                if (!publishMod)
                {
                    entry.RemoveModeratorInfo();
                }
                tmp.Add(entry);
            }

            IEnumerable<AppealView> table = tmp.AsEnumerable();

            // FILTER
            if (filter?.UserIds != null && filter.UserIds.Count > 0)
            {
                table = table.Where(x => filter.UserIds.Contains(x.User?.Id));
            }
            if (filter?.Since != null && filter.Since != DateTime.MinValue)
            {
                table = table.Where(x => x.CreatedAt >= filter.Since);
            }
            if (filter?.Before != null && filter.Before != DateTime.MinValue)
            {
                table = table.Where(x => x.CreatedAt <= filter.Before);
            }
            if (filter?.Edited != null)
            {
                table = table.Where(x => x.UpdatedAt == x.CreatedAt != filter.Edited.Value);
            }
            if (filter?.Status != null && filter.Status.Count > 0)
            {
                table = table.Where(x => filter.Status.Contains(x.Status));
            }

            return Ok(new AppealTable(table.Skip(page * 20).Take(20).ToList(), table.Count()));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppeal([FromRoute] ulong guildId, [FromBody] AppealDto dto)
        {
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);
            IUser currentUser = identity.GetCurrentUser();

            List<AppealStructure> questions = await AppealStructureRepository.CreateDefault(_serviceProvider).GetForGuild(guildId);

            if (dto.Answers.Count == 0 || dto.Answers.Where(x => questions.Any(y => y.Id == x.QuestionId && !y.Deleted)).Count() != dto.Answers.Count)
            {
                return BadRequest();
            }

            AppealRepository repo = AppealRepository.CreateDefault(_serviceProvider);
            if (! await repo.UserIsAllowedToCreateNewAppeal(guildId, currentUser.Id))
            {
                return BadRequest();
            }

            Appeal appeal = new();
            appeal.UserId = currentUser.Id;
            appeal.Username = currentUser.Username;
            appeal.Discriminator = currentUser.Discriminator;
            appeal.Mail = string.Empty;
            appeal.GuildId = guildId;

            List<AppealAnswer> answers = new();
            foreach (var answer in dto.Answers)
            {
                answers.Add(new AppealAnswer()
                {
                    AppealQuestion = questions.First(x => x.Id == answer.QuestionId),
                    Answer = answer.Answer
                });
            }

            appeal = await repo.Create(appeal, answers);

            return Ok(new AppealView(
                appeal,
                await _discordAPI.FetchUserInfo(appeal.UserId, CacheBehavior.OnlyCache),
                null,
                answers,
                questions
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppeal([FromRoute] ulong guildId, [FromRoute] int id, [FromBody] AppealUpdateDto dto)
        {
            await RequirePermission(guildId, DiscordPermission.Moderator);
            Identity identity = await GetIdentity();
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);
            IUser currentUser = identity.GetCurrentUser();
            _translator.SetContext(guildConfig);

            AppealRepository repo = AppealRepository.CreateDefault(_serviceProvider);

            Appeal appeal = await repo.GetById(id);
            if (appeal.GuildId != guildId)
            {
                return BadRequest();
            }

            if (appeal.Status != AppealStatus.Approved && dto.Status == AppealStatus.Approved)
            {
                try
                {
                    // TODO: notify user?
                    await _discordAPI.UnBanUser(guildId, appeal.UserId, _translator.T().NotificationDiscordAuditLogPunishmentsAppealApproved(appeal.Id, dto.ModeratorComment));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Could not  unban user {appeal.UserId} in guild {guildId} by approved ban appeal {appeal.Id}");
                }
            }

            appeal.Status = dto.Status;
            appeal.ModeratorComment = dto.ModeratorComment;
            appeal.LastModeratorId = currentUser.Id;
            appeal.UserCanCreateNewAppeals = dto.UserCanCreateNewAppeals;

            await repo.Update(appeal);

            return Ok(new AppealView(
                appeal,
                await _discordAPI.FetchUserInfo(appeal.UserId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(appeal.LastModeratorId, CacheBehavior.OnlyCache),
                await AppealAnswerRepository.CreateDefault(_serviceProvider).GetForAppeal(id),
                await AppealStructureRepository.CreateDefault(_serviceProvider).GetForGuild(guildId)
            ));
        }
    }
}

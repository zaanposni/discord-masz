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

        public AppealController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
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

            AppealRepository repo = AppealRepository.CreateDefault(_serviceProvider);

            Appeal appeal = await repo.GetById(id);
            if (appeal.GuildId != guildId)
            {
                return BadRequest();
            }

            if (appeal.UserId != currentUser.Id && await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                return Unauthorized();
            }

            return Ok(new AppealView(
                appeal,
                await _discordAPI.FetchUserInfo(appeal.UserId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(appeal.LastModeratorId, CacheBehavior.OnlyCache),
                await AppealAnswerRepository.CreateDefault(_serviceProvider).GetForAppeal(id),
                await AppealStructureRepository.CreateDefault(_serviceProvider).GetForGuild(guildId)
            ));
        }

        [HttpGet]
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
                    new List<AppealAnswer>(),
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

            IBan ban = await _discordAPI.GetGuildUserBan(guildId, currentUser.Id, CacheBehavior.IgnoreButCacheOnError);
            if (ban == null)
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
            appeal.Mail = dto.Email;
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

            AppealRepository repo = AppealRepository.CreateDefault(_serviceProvider);

            Appeal appeal = await repo.GetById(id);
            if (appeal.GuildId != guildId)
            {
                return BadRequest();
            }

            appeal.Status = dto.Status;
            appeal.ModeratorComment = dto.ModeratorComment;
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

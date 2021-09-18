using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Models;
using masz.Models.Views;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildId}/cases/{caseId}/view")]
    [Authorize]
    public class CaseViewController : SimpleCaseController
    {
        private readonly ILogger<CaseViewController> _logger;

        public CaseViewController(ILogger<CaseViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetModCaseView([FromRoute] ulong guildId, [FromRoute] int caseId)
        {
            await RequirePermission(guildId, caseId, APIActionPermission.View);
            GuildConfig guildConfig = await GetRegisteredGuild(guildId);
            Identity identity = await GetIdentity();

            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, identity).GetModCase(guildId, caseId);

            List<CommentExpandedView> comments = new List<CommentExpandedView>();
            foreach (ModCaseComment comment in modCase.Comments)
            {
                comments.Add(new CommentExpandedView(
                    comment,
                    await _discordAPI.FetchUserInfo(comment.UserId, CacheBehavior.OnlyCache)
                ));
            }

            CaseExpandedView caseView = new CaseExpandedView(
                modCase,
                await _discordAPI.FetchUserInfo(modCase.ModId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(modCase.LastEditedByModId, CacheBehavior.OnlyCache),
                await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.OnlyCache),
                comments
            );

            if (modCase.LockedByUserId != 0) {
                caseView.LockedBy = DiscordUserView.CreateOrDefault(await _discordAPI.FetchUserInfo(modCase.LockedByUserId, CacheBehavior.OnlyCache));
            }
            if (modCase.DeletedByUserId != 0) {
                caseView.DeletedBy = DiscordUserView.CreateOrDefault(await _discordAPI.FetchUserInfo(modCase.DeletedByUserId, CacheBehavior.OnlyCache));
            }

            if (! (await identity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId) || guildConfig.PublishModeratorInfo)) {
                caseView.RemoveModeratorInfo();
            }

            return Ok(caseView);
        }
    }
}
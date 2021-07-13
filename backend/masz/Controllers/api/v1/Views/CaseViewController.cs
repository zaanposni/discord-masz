using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using masz.data;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/guilds/{guildid}/modcases/{caseid}/view")]
    [Authorize]
    public class CaseViewController : SimpleCaseController
    {
        private readonly ILogger<CaseViewController> logger;

        public CaseViewController(ILogger<CaseViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider, logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetModCaseView([FromRoute] string guildid, [FromRoute] string caseid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            IActionResult result = await this.HandleRequest(guildid, caseid, APIActionPermission.View);
            if (result != null) {
                return result;
            }

            ModCase modCase = await this.database.SelectSpecificModCase(guildid, caseid);
            List<CommentsView> comments = new List<CommentsView>();
            foreach (ModCaseComment comment in modCase.Comments)
            {
                comments.Add(new CommentsView() {
                    Id = comment.Id,
                    Message = comment.Message,
                    CreatedAt = comment.CreatedAt,
                    UserId = comment.UserId,
                    User = await discord.FetchUserInfo(comment.UserId, CacheBehavior.OnlyCache),
                });
            }

            CaseView caseView = new CaseView(
                modCase,
                await discord.FetchUserInfo(modCase.ModId, CacheBehavior.OnlyCache),
                await discord.FetchUserInfo(modCase.LastEditedByModId, CacheBehavior.OnlyCache),
                await discord.FetchUserInfo(modCase.UserId, CacheBehavior.OnlyCache),
                comments
            );

            if (modCase.LockedByUserId != null) {
                caseView.LockedBy = await discord.FetchUserInfo(modCase.LockedByUserId, CacheBehavior.OnlyCache);
            }
            if (modCase.MarkedToDeleteAt != null) {
                caseView.DeletedBy = await discord.FetchUserInfo(modCase.DeletedByUserId, CacheBehavior.OnlyCache);
            }

            if (!(await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid) || (await this.GuildIsRegistered(guildid)).PublishModeratorInfo)) {
                caseView.RemoveModeratorInfo();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning ModCase.");
            return Ok(caseView);
        }
    }
}
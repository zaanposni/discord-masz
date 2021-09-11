using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Route("api/v1/guilds/{guildid}/usernoteview")]
    [Authorize]
    public class UserNoteViewController : SimpleController
    {
        private readonly ILogger<UserNoteViewController> logger;

        public UserNoteViewController(ILogger<UserNoteViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGuildUserNoteView([FromRoute] string guildid)
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            List<UserNote> userNotes = await this.database.GetUserNotesByGuildId(guildid);
            List<UserNoteView> userNoteViews = new List<UserNoteView>();
            foreach (UserNote userNote in userNotes)
            {
                userNoteViews.Add(new UserNoteView() {
                    UserNote = userNote,
                    Moderator = await discord.FetchUserInfo(userNote.CreatorId, CacheBehavior.OnlyCache),
                    User = await discord.FetchUserInfo(userNote.UserId, CacheBehavior.OnlyCache)
                });
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning list.");
            return Ok(userNoteViews);
        }
    }
}
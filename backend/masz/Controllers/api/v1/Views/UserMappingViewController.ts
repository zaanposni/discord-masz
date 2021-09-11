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
    [Route("api/v1/guilds/{guildid}/usermapview")]
    [Authorize]
    public class UserMappingViewController : SimpleController
    {
        private readonly ILogger<UserMappingViewController> logger;

        public UserMappingViewController(ILogger<UserMappingViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
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

            List<UserMapping> userMappings = await this.database.GetUserMappingsByGuildId(guildid);
            List<UserMappingView> userMappingViews = new List<UserMappingView>();
            foreach (UserMapping userMapping in userMappings)
            {
                userMappingViews.Add(new UserMappingView() {
                    UserMapping = userMapping,
                    Moderator = await discord.FetchUserInfo(userMapping.CreatorUserId, CacheBehavior.OnlyCache),
                    UserA = await discord.FetchUserInfo(userMapping.UserA, CacheBehavior.OnlyCache),
                    UserB = await discord.FetchUserInfo(userMapping.UserB, CacheBehavior.OnlyCache)
                });
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning list.");
            return Ok(userMappingViews);
        }
    }
}
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
    [Route("api/v1/templatesview")]
    [Authorize]
    public class TemplateViewController : SimpleController
    {
        private readonly ILogger<TemplateViewController> logger;


        public TemplateViewController(ILogger<TemplateViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        private async Task<bool> allowedToView(CaseTemplate template) {
            var currentUser = await this.IsValidUser();
            if (currentUser == null) {
                return false;
            }
            if (config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id)) {
                return true;
            }
            if (template.UserId == currentUser.Id) {
                return true;
            }

            if (template.ViewPermission == ViewPermission.Self) {
                return false;
            }

            if (template.ViewPermission == ViewPermission.Global) {
                return true;
            }

            return await this.HasPermissionOnGuild(DiscordPermission.Moderator, template.CreatedForGuildId);
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplatesView([FromQuery] string userId) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            if (await this.IsValidUser() == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            // ========================================================

            List<CaseTemplate> templates;
            if (String.IsNullOrWhiteSpace(userId)) {
                templates = await database.GetAllCaseTemplates();
            } else {
                templates = await database.GetAllTemplatesFromUser(userId);
            }
            List<TemplateView> templatesView = new List<TemplateView>();
            foreach (var template in templates)
            {
                if (await allowedToView(template)) {
                    templatesView.Add( new TemplateView() {
                        CaseTemplate = template,
                        Creator = await discord.FetchUserInfo(template.UserId, CacheBehavior.OnlyCache),
                        Guild = await discord.FetchGuildInfo(template.CreatedForGuildId, CacheBehavior.Default)
                    });
                }
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning Templates.");
            return Ok(templatesView);
        }
    }
}
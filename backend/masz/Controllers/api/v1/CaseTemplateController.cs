
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/templates")]
    [Authorize]
    public class CaseTemplateController : SimpleController
    {
        private readonly ILogger<CaseTemplateController> logger;

        public CaseTemplateController(ILogger<CaseTemplateController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
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

        [HttpGet("{templateid}")]
        public async Task<IActionResult> GetTemplate([FromRoute] string templateid, [FromQuery] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            var template = await database.GetSpecificCaseTemplate(templateid);
            if (template == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 NotFound.");
                return NotFound();
            }
            if (! await allowedToView(template)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Returning Template.");
            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CaseTemplateForCreateDto templateDto, [FromQuery] string guildid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            User currentUser = await this.IsValidUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }
            if (! await this.HasPermissionOnGuild(DiscordPermission.Moderator, guildid))
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            var existingTemplates = await database.GetAllTemplatesFromUser(currentUser.Id);
            if (existingTemplates.Count > 10 && !config.Value.SiteAdminDiscordUserIds.Contains(currentUser.Id)) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Max Template number reached.");
                return BadRequest("You can only create up to 10 templates.");
            }

            CaseTemplate template = new CaseTemplate() {
                TemplateName = templateDto.TemplateName,
                UserId = currentUser.Id,
                ViewPermission = templateDto.ViewPermission,
                CreatedForGuildId = guildid,
                CreatedAt = DateTime.UtcNow,
                CaseTitle = templateDto.Title,
                CaseDescription = templateDto.Description,
                CaseLabels = templateDto.Labels,
                CasePunishment = templateDto.Punishment,
                CasePunishedUntil = templateDto.PunishedUntil,
                CasePunishmentType = templateDto.PunishmentType,
                sendPublicNotification = templateDto.sendPublicNotification,
                announceDm = templateDto.announceDm,
                handlePunishment = templateDto.handlePunishment
            };

            await database.SaveCaseTemplate(template);
            await database.SaveChangesAsync();

            logger.LogInformation(HttpContext.Request.Method + " " + HttpContext.Request.Path + " | 201 Resource created.");
            return StatusCode(201, template);
        }

        [HttpDelete("{templateid}")]
        public async Task<IActionResult> DeleteTemplate([FromRoute] string templateid) 
        {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");
            User currentUser = await this.IsValidUser();
            if (currentUser == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            var template = await database.GetSpecificCaseTemplate(templateid);
            if (template == null) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 404 NotFound.");
                return NotFound();
            }

            if (template.UserId != currentUser.Id && ! await this.IsSiteAdmin()) {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 401 Unauthorized.");
                return Unauthorized();
            }

            database.DeleteSpecificCaseTemplate(template);
            await database.SaveChangesAsync();

            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 200 Template deleted.");
            return Ok(template);
        }
    }
}
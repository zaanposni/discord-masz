
using System;
using System.Threading.Tasks;
using masz.Dtos.ModCase;
using masz.Models;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/templates")]
    [Authorize]
    public class CaseTemplateController : SimpleController
    {
        private readonly ILogger<CaseTemplateController> _logger;

        public CaseTemplateController(ILogger<CaseTemplateController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CaseTemplateForCreateDto templateDto, [FromQuery] ulong guildId)
        {
            Identity currentIdentity = await GetIdentity();
            await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId);


            CaseTemplate template = new CaseTemplate() {
                TemplateName = templateDto.TemplateName,
                UserId = currentIdentity.GetCurrentUser().Id,
                ViewPermission = templateDto.ViewPermission,
                CreatedForGuildId = guildId,
                CreatedAt = DateTime.UtcNow,
                CaseTitle = templateDto.Title,
                CaseDescription = templateDto.Description,
                CaseLabels = templateDto.Labels,
                CasePunishedUntil = templateDto.PunishedUntil,
                CasePunishmentType = templateDto.PunishmentType,
                sendPublicNotification = templateDto.sendPublicNotification,
                announceDm = templateDto.announceDm,
                handlePunishment = templateDto.handlePunishment
            };

            return StatusCode(201, await CaseTemplateRepository.CreateDefault(_serviceProvider, currentIdentity).CreateTemplate(template));
        }

        [HttpDelete("{templateId}")]
        public async Task<IActionResult> DeleteTemplate([FromRoute] int templateId)
        {
            Identity currentIdentity = await GetIdentity();

            CaseTemplateRepository repo = CaseTemplateRepository.CreateDefault(_serviceProvider, currentIdentity);

            CaseTemplate template = await repo.GetTemplate(templateId);

            if (! (await currentIdentity.IsAllowedTo(APIActionPermission.Delete, template)))
            {
                return Unauthorized();
            }

            return Ok(repo.DeleteTemplate(template));
        }
    }
}
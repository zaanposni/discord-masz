using MASZ.Dtos.ModCase;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/templates")]
    [Authorize]
    public class CaseTemplateController : SimpleController
    {
        public CaseTemplateController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CaseTemplateForCreateDto templateDto, [FromQuery] ulong guildId)
        {
            Identity currentIdentity = await GetIdentity();
            if (!await currentIdentity.HasPermissionOnGuild(DiscordPermission.Moderator, guildId))
            {
                throw new UnauthorizedException();
            }


            CaseTemplate template = new()
            {
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
                SendPublicNotification = templateDto.SendPublicNotification,
                AnnounceDm = templateDto.AnnounceDm,
                HandlePunishment = templateDto.HandlePunishment
            };

            CaseTemplate createdTemplate = await CaseTemplateRepository.CreateDefault(_serviceProvider, currentIdentity).CreateTemplate(template);

            return StatusCode(201, new CaseTemplateView(createdTemplate));
        }

        [HttpDelete("{templateId}")]
        public async Task<IActionResult> DeleteTemplate([FromRoute] int templateId)
        {
            Identity currentIdentity = await GetIdentity();

            CaseTemplateRepository repo = CaseTemplateRepository.CreateDefault(_serviceProvider, currentIdentity);

            CaseTemplate template = await repo.GetTemplate(templateId);

            if (!await currentIdentity.IsAllowedTo(APIActionPermission.Delete, template))
            {
                return Unauthorized();
            }

            await repo.DeleteTemplate(template);

            return Ok(new CaseTemplateView(template));
        }
    }
}
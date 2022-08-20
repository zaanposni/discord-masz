using Discord;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Models.Views;
using MASZ.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/templatesview")]
    [Authorize]
    public class TemplateViewController : SimpleController
    {

        public TemplateViewController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplatesView([FromQuery] ulong userId = 0)
        {
            Identity currentIdentity = await GetIdentity();

            CaseTemplateRepository repository = CaseTemplateRepository.CreateDefault(_serviceProvider, currentIdentity);
            List<CaseTemplate> templates = await repository.GetTemplatesBasedOnPermissions();
            List<CaseTemplateExpandedView> templatesView = new();
            foreach (var template in templates.Where(x => x.UserId == userId || userId == 0))
            {
                IGuild guild = _discordAPI.FetchGuildInfo(template.CreatedForGuildId, CacheBehavior.Default);
                if (guild == null) {
                    continue;
                }
                templatesView.Add(new CaseTemplateExpandedView(
                    template,
                    await _discordAPI.FetchUserInfo(template.UserId, CacheBehavior.OnlyCache),
                    guild
                ));
            }

            return Ok(templatesView);
        }
    }
}
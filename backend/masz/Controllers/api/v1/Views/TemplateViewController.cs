using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.Models;
using masz.Models.Views;
using masz.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using masz.Enums;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/templatesview")]
    [Authorize]
    public class TemplateViewController : SimpleController
    {
        private readonly ILogger<TemplateViewController> _logger;


        public TemplateViewController(ILogger<TemplateViewController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplatesView([FromQuery] ulong userId = 0)
        {
            Identity currentIdentity = await GetIdentity();

            CaseTemplateRepository repository = CaseTemplateRepository.CreateDefault(_serviceProvider, currentIdentity);
            List<CaseTemplate> templates = await repository.GetTemplatesBasedOnPermissions();
            if (userId != 0)
            {
                templates.Where(x => x.UserId == userId).ToList();
            }
            List<CaseTemplateExpandedView> templatesView = new List<CaseTemplateExpandedView>();
            foreach (var template in templates)
            {
                templatesView.Add( new CaseTemplateExpandedView(
                    template,
                    await _discordAPI.FetchUserInfo(template.UserId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchGuildInfo(template.CreatedForGuildId, CacheBehavior.Default)
                ));
            }

            return Ok(templatesView);
        }
    }
}
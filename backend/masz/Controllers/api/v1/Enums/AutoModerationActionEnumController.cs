using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
using masz.Enums;
using masz.Translations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class AutoModerationActionEnumController : SimpleController
    {
        private readonly ILogger<AutoModerationActionEnumController> _logger;

        public AutoModerationActionEnumController(ILogger<AutoModerationActionEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("automodaction")]
        public IActionResult AutoModActions([FromQuery] Language? language = null) {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) AutoModerationAction.None, _translator.T().Enum(AutoModerationAction.None)),
                EnumDto.Create((int) AutoModerationAction.ContentDeleted, _translator.T().Enum(AutoModerationAction.ContentDeleted)),
                EnumDto.Create((int) AutoModerationAction.CaseCreated, _translator.T().Enum(AutoModerationAction.CaseCreated)),
                EnumDto.Create((int) AutoModerationAction.ContentDeletedAndCaseCreated, _translator.T().Enum(AutoModerationAction.ContentDeletedAndCaseCreated))
            });
        }
    }
}
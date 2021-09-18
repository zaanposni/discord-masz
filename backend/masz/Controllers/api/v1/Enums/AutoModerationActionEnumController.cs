using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
using masz.Models;
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
            List<EnumDto> enums = new List<EnumDto>();
            enums.Add(new EnumDto((int) AutoModerationAction.None, _translator.T(language).EnumsAutoModActionsNone()));
            enums.Add(new EnumDto((int) AutoModerationAction.ContentDeleted, _translator.T(language).EnumsAutoModActionsContentDeleted()));
            enums.Add(new EnumDto((int) AutoModerationAction.CaseCreated, _translator.T(language).EnumsAutoModActionsCaseCreated()));
            enums.Add(new EnumDto((int) AutoModerationAction.ContentDeletedAndCaseCreated, _translator.T(language).EnumsAutoModActionsContentDeletedAndCaseCreated()));

            return Ok(enums);
        }
    }
}
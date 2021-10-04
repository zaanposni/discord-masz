using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
using masz.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class CaseCreationTypeEnumController : SimpleController
    {
        private readonly ILogger<CaseCreationTypeEnumController> _logger;

        public CaseCreationTypeEnumController(ILogger<CaseCreationTypeEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("casecreationtype")]
        public IActionResult CreationType([FromQuery] Language? language = null) {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) CaseCreationType.Default, _translator.T().Enum(CaseCreationType.Default)),
                EnumDto.Create((int) CaseCreationType.AutoModeration, _translator.T().Enum(CaseCreationType.AutoModeration)),
                EnumDto.Create((int) CaseCreationType.Imported, _translator.T().Enum(CaseCreationType.Imported)),
                EnumDto.Create((int) CaseCreationType.ByCommand, _translator.T().Enum(CaseCreationType.ByCommand))
            });
        }
    }
}
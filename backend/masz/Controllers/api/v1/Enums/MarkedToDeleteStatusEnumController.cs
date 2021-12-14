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
    public class MarkedToDeleteStatusEnumController : SimpleController
    {
        private readonly ILogger<MarkedToDeleteStatusEnumController> _logger;

        public MarkedToDeleteStatusEnumController(ILogger<MarkedToDeleteStatusEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("markedtodeletestatus")]
        public IActionResult ViewMarkedToDeleteStatus([FromQuery] Language? language = null) {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) MarkedToDeleteStatus.None, _translator.T().Enum(MarkedToDeleteStatus.None)),
                EnumDto.Create((int) MarkedToDeleteStatus.Unmarked, _translator.T().Enum(MarkedToDeleteStatus.Unmarked)),
                EnumDto.Create((int) MarkedToDeleteStatus.Marked, _translator.T().Enum(MarkedToDeleteStatus.Marked))
            });
        }
    }
}
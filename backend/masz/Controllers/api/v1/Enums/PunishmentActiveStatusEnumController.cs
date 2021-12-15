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
    public class PunishmentActiveStatusEnumController : SimpleController
    {
        private readonly ILogger<PunishmentActiveStatusEnumController> _logger;

        public PunishmentActiveStatusEnumController(ILogger<PunishmentActiveStatusEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("punishmentactivestatus")]
        public IActionResult ViewPunishmentActiveStatus([FromQuery] Language? language = null) {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) PunishmentActiveStatus.None, _translator.T().Enum(PunishmentActiveStatus.None)),
                EnumDto.Create((int) PunishmentActiveStatus.Inactive, _translator.T().Enum(PunishmentActiveStatus.Inactive)),
                EnumDto.Create((int) PunishmentActiveStatus.Active, _translator.T().Enum(PunishmentActiveStatus.Active))
            });
        }
    }
}
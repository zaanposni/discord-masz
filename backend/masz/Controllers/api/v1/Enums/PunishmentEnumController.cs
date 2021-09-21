using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
using masz.Enums;
using masz.Models;
using masz.Services;
using masz.Translations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class PunishmentEnumController : SimpleController
    {
        private readonly ILogger<PunishmentEnumController> _logger;

        public PunishmentEnumController(ILogger<PunishmentEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("punishment")]
        public IActionResult Punishment([FromQuery] Language? language = null) {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) PunishmentType.None, _translator.T().Enum(PunishmentType.None)),
                EnumDto.Create((int) PunishmentType.Mute, _translator.T().Enum(PunishmentType.Mute)),
                EnumDto.Create((int) PunishmentType.Kick, _translator.T().Enum(PunishmentType.Kick)),
                EnumDto.Create((int) PunishmentType.Ban, _translator.T().Enum(PunishmentType.Ban))
            });
        }
    }
}
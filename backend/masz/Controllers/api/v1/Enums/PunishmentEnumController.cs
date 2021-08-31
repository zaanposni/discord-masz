using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
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
        private readonly ILogger<PunishmentEnumController> logger;

        public PunishmentEnumController(ILogger<PunishmentEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet("punishment")]
        public IActionResult Punishment([FromQuery] Language? language = null) {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");

            List<EnumDto> enums = new List<EnumDto>();
            enums.Add(new EnumDto((int) PunishmentType.None, translator.T(language).EnumsPunishmentWarn()));
            enums.Add(new EnumDto((int) PunishmentType.Mute, translator.T(language).EnumsPunishmentMute()));
            enums.Add(new EnumDto((int) PunishmentType.Kick, translator.T(language).EnumsPunishmentKick()));
            enums.Add(new EnumDto((int) PunishmentType.Ban, translator.T(language).EnumsPunishmentBan()));

            return Ok(enums);
        }
    }
}
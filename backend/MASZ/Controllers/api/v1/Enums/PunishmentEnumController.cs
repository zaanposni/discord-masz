using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class PunishmentEnumController : SimpleController
    {

        public PunishmentEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("punishment")]
        public IActionResult Punishment([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) PunishmentType.Warn, _translator.T().Enum(PunishmentType.Warn)),
                EnumDto.Create((int) PunishmentType.Mute, _translator.T().Enum(PunishmentType.Mute)),
                EnumDto.Create((int) PunishmentType.Kick, _translator.T().Enum(PunishmentType.Kick)),
                EnumDto.Create((int) PunishmentType.Ban, _translator.T().Enum(PunishmentType.Ban))
            });
        }
    }
}
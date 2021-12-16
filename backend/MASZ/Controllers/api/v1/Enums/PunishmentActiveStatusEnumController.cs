using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class PunishmentActiveStatusEnumController : SimpleController
    {

        public PunishmentActiveStatusEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("punishmentactivestatus")]
        public IActionResult ViewPunishmentActiveStatus([FromQuery] Language? language = null)
        {
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
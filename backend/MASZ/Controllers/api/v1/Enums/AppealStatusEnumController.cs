using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class AppealStatusEnumController : SimpleController
    {

        public AppealStatusEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("appealstatus")]
        public IActionResult Status([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) AppealStatus.Pending, _translator.T().Enum(AppealStatus.Pending)),
                EnumDto.Create((int) AppealStatus.Approved, _translator.T().Enum(AppealStatus.Approved)),
                EnumDto.Create((int) AppealStatus.Declined, _translator.T().Enum(AppealStatus.Declined))
            });
        }
    }
}
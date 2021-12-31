using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class MarkedToDeleteStatusEnumController : SimpleController
    {

        public MarkedToDeleteStatusEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("markedtodeletestatus")]
        public IActionResult ViewMarkedToDeleteStatus([FromQuery] Language? language = null)
        {
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
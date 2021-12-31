using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class CaseCreationTypeEnumController : SimpleController
    {
        public CaseCreationTypeEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("casecreationtype")]
        public IActionResult CreationType([FromQuery] Language? language = null)
        {
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
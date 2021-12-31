using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class EditStatusEnumController : SimpleController
    {
        public EditStatusEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("editstatus")]
        public IActionResult ViewEditStatus([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) EditStatus.None, _translator.T().Enum(EditStatus.None)),
                EnumDto.Create((int) EditStatus.Unedited, _translator.T().Enum(EditStatus.Unedited)),
                EnumDto.Create((int) EditStatus.Edited, _translator.T().Enum(EditStatus.Edited))
            });
        }
    }
}
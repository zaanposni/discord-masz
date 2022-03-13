using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class AutoModerationActionEnumController : SimpleController
    {
        public AutoModerationActionEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("automodaction")]
        public IActionResult AutoModActions([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) AutoModerationAction.None, _translator.T().Enum(AutoModerationAction.None)),
                EnumDto.Create((int) AutoModerationAction.ContentDeleted, _translator.T().Enum(AutoModerationAction.ContentDeleted)),
                EnumDto.Create((int) AutoModerationAction.CaseCreated, _translator.T().Enum(AutoModerationAction.CaseCreated)),
                EnumDto.Create((int) AutoModerationAction.ContentDeletedAndCaseCreated, _translator.T().Enum(AutoModerationAction.ContentDeletedAndCaseCreated)),
                EnumDto.Create((int) AutoModerationAction.Timeout, _translator.T().Enum(AutoModerationAction.Timeout))
            });
        }
    }
}
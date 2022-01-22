using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class ScheduledMessageStatusEnumController : SimpleController
    {

        public ScheduledMessageStatusEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("scheduledmessagestatus")]
        public IActionResult Status([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) ScheduledMessageStatus.Pending, _translator.T().Enum(ScheduledMessageStatus.Pending)),
                EnumDto.Create((int) ScheduledMessageStatus.Sent, _translator.T().Enum(ScheduledMessageStatus.Sent)),
                EnumDto.Create((int) ScheduledMessageStatus.Failed, _translator.T().Enum(ScheduledMessageStatus.Failed))
            });
        }
    }
}
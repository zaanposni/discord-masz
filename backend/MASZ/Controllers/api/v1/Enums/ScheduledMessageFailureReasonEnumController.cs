using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class ScheduledMessageFailureReasonEnumController : SimpleController
    {

        public ScheduledMessageFailureReasonEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("scheduledmessagefailurereason")]
        public IActionResult FailureReason([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) ScheduledMessageFailureReason.Unknown, _translator.T().Enum(ScheduledMessageFailureReason.Unknown)),
                EnumDto.Create((int) ScheduledMessageFailureReason.ChannelNotFound, _translator.T().Enum(ScheduledMessageFailureReason.ChannelNotFound)),
                EnumDto.Create((int) ScheduledMessageFailureReason.InsufficientPermission, _translator.T().Enum(ScheduledMessageFailureReason.InsufficientPermission))
            });
        }
    }
}
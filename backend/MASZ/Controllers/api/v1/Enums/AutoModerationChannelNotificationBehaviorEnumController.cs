using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class AutoModerationChannelNotificationBehaviorEnumController : SimpleController
    {
        public AutoModerationChannelNotificationBehaviorEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("automodchannelbehavior")]
        public IActionResult ChannelNotificationBehavior([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) AutoModerationChannelNotificationBehavior.SendNotification, _translator.T().Enum(AutoModerationChannelNotificationBehavior.SendNotification)),
                EnumDto.Create((int) AutoModerationChannelNotificationBehavior.SendNotificationAndDelete, _translator.T().Enum(AutoModerationChannelNotificationBehavior.SendNotificationAndDelete)),
                EnumDto.Create((int) AutoModerationChannelNotificationBehavior.NoNotification, _translator.T().Enum(AutoModerationChannelNotificationBehavior.NoNotification))
            });
        }
    }
}
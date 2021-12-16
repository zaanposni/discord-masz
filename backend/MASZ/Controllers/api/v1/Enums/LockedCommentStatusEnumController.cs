using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class LockedCommentStatusEnumController : SimpleController
    {

        public LockedCommentStatusEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("lockedcommentstatus")]
        public IActionResult ViewLockedCommentStatus([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) LockedCommentStatus.None, _translator.T().Enum(LockedCommentStatus.None)),
                EnumDto.Create((int) LockedCommentStatus.Unlocked, _translator.T().Enum(LockedCommentStatus.Unlocked)),
                EnumDto.Create((int) LockedCommentStatus.Locked, _translator.T().Enum(LockedCommentStatus.Locked))
            });
        }
    }
}
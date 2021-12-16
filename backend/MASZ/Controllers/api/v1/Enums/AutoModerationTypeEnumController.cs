using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class AutoModerationTypeEnumController : SimpleController
    {
        public AutoModerationTypeEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("automodtype")]
        public IActionResult AutoModTypes([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) AutoModerationType.InvitePosted, _translator.T().Enum(AutoModerationType.InvitePosted)),
                EnumDto.Create((int) AutoModerationType.TooManyEmotes, _translator.T().Enum(AutoModerationType.TooManyEmotes)),
                EnumDto.Create((int) AutoModerationType.TooManyMentions, _translator.T().Enum(AutoModerationType.TooManyMentions)),
                EnumDto.Create((int) AutoModerationType.TooManyAttachments, _translator.T().Enum(AutoModerationType.TooManyAttachments)),
                EnumDto.Create((int) AutoModerationType.TooManyEmbeds, _translator.T().Enum(AutoModerationType.TooManyEmbeds)),
                EnumDto.Create((int) AutoModerationType.TooManyAutoModerations, _translator.T().Enum(AutoModerationType.TooManyAutoModerations)),
                EnumDto.Create((int) AutoModerationType.CustomWordFilter, _translator.T().Enum(AutoModerationType.CustomWordFilter)),
                EnumDto.Create((int) AutoModerationType.TooManyMessages, _translator.T().Enum(AutoModerationType.TooManyMessages)),
                EnumDto.Create((int) AutoModerationType.TooManyDuplicatedCharacters, _translator.T().Enum(AutoModerationType.TooManyDuplicatedCharacters)),
                EnumDto.Create((int) AutoModerationType.TooManyLinks, _translator.T().Enum(AutoModerationType.TooManyLinks))
            });
        }
    }
}
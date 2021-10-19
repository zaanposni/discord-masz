using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
using masz.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class AutoModerationTypeEnumController : SimpleController
    {
        private readonly ILogger<AutoModerationTypeEnumController> _logger;

        public AutoModerationTypeEnumController(ILogger<AutoModerationTypeEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("automodtype")]
        public IActionResult AutoModTypes([FromQuery] Language? language = null) {
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
                EnumDto.Create((int) AutoModerationType.TooManyDuplicatedCharacters, _translator.T().Enum(AutoModerationType.TooManyDuplicatedCharacters))
            });
        }
    }
}
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
    public class LockedCommentStatusEnumController : SimpleController
    {
        private readonly ILogger<LockedCommentStatusEnumController> _logger;

        public LockedCommentStatusEnumController(ILogger<LockedCommentStatusEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("lockedcommentstatus")]
        public IActionResult ViewLockedCommentStatus([FromQuery] Language? language = null) {
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
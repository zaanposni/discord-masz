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
    public class EditStatusEnumController : SimpleController
    {
        private readonly ILogger<EditStatusEnumController> _logger;

        public EditStatusEnumController(ILogger<EditStatusEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("editstatus")]
        public IActionResult ViewEditStatus([FromQuery] Language? language = null) {
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
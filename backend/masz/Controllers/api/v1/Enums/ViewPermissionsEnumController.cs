using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
using masz.Enums;
using masz.Models;
using masz.Translations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class ViewPermissionsEnumController : SimpleController
    {
        private readonly ILogger<ViewPermissionsEnumController> _logger;

        public ViewPermissionsEnumController(ILogger<ViewPermissionsEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        [HttpGet("viewpermission")]
        public IActionResult ViewPermissions([FromQuery] Language? language = null) {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) ViewPermission.Global, _translator.T().Enum(ViewPermission.Global)),
                EnumDto.Create((int) ViewPermission.Guild, _translator.T().Enum(ViewPermission.Guild)),
                EnumDto.Create((int) ViewPermission.Self, _translator.T().Enum(ViewPermission.Self))
            });
        }
    }
}
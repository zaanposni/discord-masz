using System;
using System.Collections.Generic;
using masz.Dtos.Enum;
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
            List<EnumDto> enums = new List<EnumDto>();
            enums.Add(new EnumDto((int) ViewPermission.Global, _translator.T(language).EnumsViewPermissionGlobal()));
            enums.Add(new EnumDto((int) ViewPermission.Guild, _translator.T(language).EnumsViewPermissionGuild()));
            enums.Add(new EnumDto((int) ViewPermission.Self, _translator.T(language).EnumsViewPermissionSelf()));

            return Ok(enums);
        }
    }
}
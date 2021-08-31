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
        private readonly ILogger<ViewPermissionsEnumController> logger;

        public ViewPermissionsEnumController(ILogger<ViewPermissionsEnumController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.logger = logger;
        }

        [HttpGet("viewpermission")]
        public IActionResult ViewPermissions([FromQuery] Language? language = null) {
            logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | Incoming request.");

            List<EnumDto> enums = new List<EnumDto>();
            enums.Add(new EnumDto((int) ViewPermission.Global, translator.T(language).EnumsViewPermissionGlobal()));
            enums.Add(new EnumDto((int) ViewPermission.Guild, translator.T(language).EnumsViewPermissionGuild()));
            enums.Add(new EnumDto((int) ViewPermission.Self, translator.T(language).EnumsViewPermissionSelf()));

            return Ok(enums);
        }
    }
}
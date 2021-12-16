using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class ViewPermissionsEnumController : SimpleController
    {
        public ViewPermissionsEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("viewpermission")]
        public IActionResult ViewPermissions([FromQuery] Language? language = null)
        {
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
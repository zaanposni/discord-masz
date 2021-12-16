using MASZ.Dtos.Enum;
using MASZ.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/enums/")]
    public class LanguageEnumController : SimpleController
    {

        public LanguageEnumController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("language")]
        public IActionResult LanguageEnum([FromQuery] Language? language = null)
        {
            _translator.SetContext(language);
            return Ok(new List<EnumDto>()
            {
                EnumDto.Create((int) Language.en, _translator.T().Enum(Language.en)),
                EnumDto.Create((int) Language.de, _translator.T().Enum(Language.de)),
                EnumDto.Create((int) Language.fr, _translator.T().Enum(Language.fr)),
                EnumDto.Create((int) Language.es, _translator.T().Enum(Language.es)),
                EnumDto.Create((int) Language.it, _translator.T().Enum(Language.it)),
                EnumDto.Create((int) Language.at, _translator.T().Enum(Language.at)),
                EnumDto.Create((int) Language.ru, _translator.T().Enum(Language.ru))
            });
        }
    }
}
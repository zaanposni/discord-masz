using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    public class ModCaseController : ControllerBase
    {
        private readonly ILogger<ModCaseController> logger;

        public ModCaseController(ILogger<ModCaseController> logger)
        {
            this.logger = logger;
        }
    }
}
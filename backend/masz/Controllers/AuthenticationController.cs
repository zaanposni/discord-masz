using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/login")]
        public async Task<IActionResult> Login()
        {
            var properties = new AuthenticationProperties()
            {
                // actual redirect endpoint for your app
                RedirectUri = $"/weatherforecast",
                Items =
                {
                    { "LoginProvider", "Discord" },
                },
                AllowRefresh = true,
            };

            return this.Challenge(properties, "Discord");
        }
    }
}
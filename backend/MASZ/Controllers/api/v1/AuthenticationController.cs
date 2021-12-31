using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class AuthenticationController : SimpleController
    {
        public AuthenticationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string ReturnUrl)
        {
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = "/guilds";
            }

            var properties = new AuthenticationProperties()
            {
                RedirectUri = ReturnUrl,
                Items =
                {
                    { "LoginProvider", "Discord" },
                    { "scheme", "Discord" }
                },
                AllowRefresh = true,
            };
            return Challenge(properties, "Discord");
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            _identityManager.RemoveIdentity(HttpContext);
            var properties = new AuthenticationProperties()
            {
                RedirectUri = "/",
                Items =
                {
                    { "LoginProvider", "Discord" },
                    { "scheme", "Discord" }
                },
                AllowRefresh = true,
            };
            return SignOut(properties, CookieAuthenticationDefaults.AuthenticationScheme);
        }

    }
}
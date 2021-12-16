using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class StatusController : SimpleController
    {
        public StatusController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("status")]
        [HttpGet("health")]
        [HttpGet("healthcheck")]
        [HttpGet("ping")]
        public IActionResult Status()
        {
            if (HttpContext.Request.Headers.ContainsKey("Accept"))
            {
                if (HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Ok(new
                    {
                        status = "ok",
                        lang = _config.GetDefaultLanguage(),
                        name = _config.GetHostName(),
                        server_time = DateTime.Now.ToString(),
                        server_time_utc = DateTime.UtcNow.ToString()
                    });
                }
            }
            return Ok("OK");
        }
    }
}
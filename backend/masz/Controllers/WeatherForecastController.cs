using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masz.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Discord.OAuth2;
using Microsoft.AspNetCore.Authentication;

namespace masz.Controllers
{
    [ApiController]
    [Route("test")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> logger;
        private readonly IAuthRepository repo;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IAuthRepository repo)
        {
            this.logger = logger;
            this.repo = repo;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            List<String> dummylist = new List<string>();
            foreach(var claim in User.Claims)
            {
                dummylist.Add(claim.Value);
            }
            return Enumerable.Range(1, 1).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                Test = dummylist
            })
            .ToArray();
        }

        [HttpGet("secret")]
        [Authorize]
        public async Task<IActionResult> GetSecret()
        {
            if (! await repo.DiscordUserIsAuthorized(HttpContext)) {
                return Unauthorized("Unauthorized");
            }
            return Ok("Ok");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace MASZ.Controllers
{
    [ApiController]
    public class IndexController
    {
        public IndexController()
        {
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return new ContentResult()
            {
                Content = "<a href=\"/api/v1/login\" target=\"blank\">Login with Discord.</a>",
                ContentType = "text/html",
            };
        }
    }
}
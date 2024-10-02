using Application.Base;
using Application.Services.WebInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace API.Controllers
{
    [Route("/api/[controller]")]
    [Authorize]
    public class TestController : BaseController<TestController>
    {

        public TestController(
            ILogger<TestController> logger,
            IUserService userService
            ) : base(logger, userService)
        {
        }
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}

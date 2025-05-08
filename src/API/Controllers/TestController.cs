using Application.Base;
using Application.Services.WebInterfaces;
using Core.Models.SearchModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("/api/[controller]")]
    [AllowAnonymous]
    public class TestController : BaseController<TestController>
    {

        public TestController(
            ILogger<TestController> logger,
            IUserService userService
            ) : base(logger, userService)
        {
        }
        [HttpPost]
        [Route("GetMockDataAPI")]
        public async Task<IActionResult> GetMockDataAPI([FromBody]UserSearchModel searchModel)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            //var result1= await UserService.MockDataRole();
            //var result = await UserService.GetMockData();
            //result.Data = result.Data.Take(10).ToList();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            return Ok(new { Time = elapsedMs });
        }
    }
}

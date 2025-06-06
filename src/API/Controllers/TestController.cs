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

        /// <summary>
        /// Test API to get current user ID from UserService
        /// </summary>
        /// <returns>Current user information including ID, authentication status, and user details</returns>
        [HttpGet]
        [Route("GetCurrentUserId")]
        public async Task<IActionResult> GetCurrentUserId()
        {
            try
            {
                var result = await UserService.GetCurrentUserId();
                return MapToIActionResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in GetCurrentUserId API");
                return StatusCode(500, new {
                    Message = "Internal server error",
                    Error = ex.Message,
                    Timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Test API to get current user ID with additional context information
        /// </summary>
        /// <returns>Extended current user information</returns>
        [HttpGet]
        [Route("GetCurrentUserInfo")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            try
            {
                var currentUserResult = await UserService.GetCurrentUserId();

                // Add additional context information
                var response = new
                {
                    CurrentUser = currentUserResult.Data,
                    RequestInfo = new
                    {
                        Timestamp = DateTime.Now,
                        RequestId = Guid.NewGuid(),
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        RemoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        IsHttps = Request.IsHttps
                    },
                    ServerInfo = new
                    {
                        MachineName = Environment.MachineName,
                        ProcessId = Environment.ProcessId,
                        WorkingSet = Environment.WorkingSet
                    }
                };

                if (currentUserResult.IsSuccess)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode((int)currentUserResult.StatusCode, response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in GetCurrentUserInfo API");
                return StatusCode(500, new {
                    Message = "Internal server error",
                    Error = ex.Message,
                    Timestamp = DateTime.Now
                });
            }
        }
    }
}

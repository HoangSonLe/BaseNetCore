using Application.Services.WebInterfaces;
using Core.CommonModels.BaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
namespace Application.Base
{
    [ApiController]
    //[Route("api/[controller]")]
    public class BaseController<T> : Controller
    {
        private readonly ILogger<T> _logger;
        private readonly IUserService _userService;


        public string _currentUserId => "1";//User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;// HttpContext.User.FindFirst("UserID").Value;

        public BaseController(ILogger<T> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        public IUserService UserService => _userService;
        public ILogger<T> Logger => _logger;
        protected IActionResult MapToIActionResult(Acknowledgement ack)
        {
            if (!ack.IsSuccess)
            {
                return StatusCode((int)ack.StatusCode, ack.ErrorMessageList);
            }
            return Ok(); // Or Ok(someData) if you want to return additional data
        }

        protected IActionResult MapToIActionResult<T>(Acknowledgement<T> ack)
        {
            if (ack == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new List<string> { "ReturnData is null" });
            }

            if (!ack.IsSuccess)
            {
                return StatusCode((int)ack.StatusCode, ack.ErrorMessageList);
            }

            return Ok(ack.Data); // Return the data part of the acknowledgment
        }

    }
}

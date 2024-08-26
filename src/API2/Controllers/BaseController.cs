//using Common.ConfigModel;
//using DAL.Interfaces;
//using Microsoft.AspNetCore.Mvc;
//using Service.WebInterfaces;

//namespace API.Controllers
//{
//    public class BaseController<T> : Controller
//    {
//        private readonly ILogger<T> _logger;
//        private readonly IUserService _userService;

//        public BaseController(ILogger<T> logger, IUserService userService)
//        {
//            _logger = logger;
//            _userService = userService;
//        }
//        public IUserService UserService => _userService;
//        public ILogger<T> Logger => _logger;
//        [NonAction]
//        public string GetCurrentUserId()
//        {
//            return HttpContext.User.FindFirst("UserID").Value;

//        }
//    }
//}

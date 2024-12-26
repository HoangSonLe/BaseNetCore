using Application.Base;
using Application.Services.WebInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseWebsite.Controllers
{
    [Authorize]
    public class HomeController : BaseController<HomeController>
    {
        private readonly IConfiguration _configuration;
        public HomeController(
            ILogger<HomeController> logger,
            IUserService userService,
            IRoleService roleService,
              IConfiguration configuration
            ) : base(logger, userService)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var _user = await UserService.GetUserById(int.Parse(_currentUserId));
            return MapToIActionResult(_user);
        }
        [AllowAnonymous]
        public string Values()
        {
            return "Server is running";
        }
        //public IActionResult QRCode()
        //{
        //    var ack = new Acknowledgement<string>();
        //    try
        //    {
        //        var urlBotTelegram = _configuration.GetSection("TelegramBotUrl").Value;
        //        var qrCode = Helper.GenerateQrCodeAsBase64(urlBotTelegram);
        //        ack.StatusCode = HttpStatusCode.OK;
        //        ack.Data = qrCode;
        //        return Json(ack);
        //    }
        //    catch (Exception ex)
        //    {
        //        ack.ExtractMessage(ex);
        //        return Json(ack);
        //    }
        //}
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    // Path to the HTML file
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/403Page.html");

        //    // Read the HTML file content
        //    var htmlContent = System.IO.File.ReadAllText(filePath);

        //    // Pass the HTML content to the view
        //    ViewBag.HtmlContent = htmlContent;

        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}

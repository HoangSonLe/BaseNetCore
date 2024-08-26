using Application.Base;
using Application.Services.WebInterfaces;
using Core.CommonModels.BaseModels;
using Core.Enums;
using Core.Helper;
using Core.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            if (_user != null && _user.IsSuccess)
            {
                if (_user.Data.RoleIdList.Count == 1 && _user.Data.RoleIdList.Contains((int)ERoleType.User)) return Redirect("/Urn");
                if (_user.Data.RoleIdList.Contains((int)ERoleType.Admin) || _user.Data.RoleIdList.Contains((int)ERoleType.Reporter))
                {
                    return Redirect("/Urn");
                }
            }
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }
        public string Values()
        {
            return "Server is running";
        }
        public IActionResult QRCode()
        {
            var ack = new Acknowledgement<string>();
            try
            {
                var urlBotTelegram = _configuration.GetSection("TelegramBotUrl").Value;
                var qrCode = Helper.GenerateQrCodeAsBase64(urlBotTelegram);
                ack.IsSuccess = true;
                ack.Data = qrCode;
                return Json(ack);
            }
            catch (Exception ex)
            {
                ack.ExtractMessage(ex);
                return Json(ack);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Path to the HTML file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/403Page.html");

            // Read the HTML file content
            var htmlContent = System.IO.File.ReadAllText(filePath);

            // Pass the HTML content to the view
            ViewBag.HtmlContent = htmlContent;

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

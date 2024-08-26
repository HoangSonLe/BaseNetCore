using Application.Base;
using Application.Services.WebInterfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseWebsite.Controllers
{
    [Authorize]
    public class TestController : BaseController<TestController>
    {
        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailSender;
        private readonly IHttpContextAccessor _accessor;

        public object ContextType { get; private set; }

        public TestController(
            IUserService userService,
            ILogger<TestController> logger,
            IConfiguration configuration,
            EmailService emailService
            ) : base(logger, userService)
        {
            _userService = userService;
            _configuration = configuration;
            _emailSender = emailService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task Test([FromHeader] string mail)
        {
            await _emailSender.SendEmailAsync(mail);
        }
    }
}

using Microsoft.AspNetCore.Authorization;

namespace BaseWebsite.Authorizations
{
    public class C3FunctionAuthorizationHandler : AuthorizationHandler<C3FunctionAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public C3FunctionAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, C3FunctionAuthorizationRequirement requirement)
        {
            bool Succeed = false;
            var claim = context.User.FindFirst("Actions");
            var a = _httpContextAccessor.HttpContext.User.Claims;
            var listFunction = requirement.PermissionId;
            if (claim != null)
            {
                foreach (var funcId in claim?.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (listFunction.ListPermission.IndexOf(Convert.ToInt32(funcId)) != -1)
                    {
                        context.Succeed(requirement);
                        Succeed = true;
                        break;
                    }
                }
            }

            if (!Succeed && listFunction.Redirect)
            {
                context.Fail();
                //var Response = _httpContextAccessor.HttpContext.Response;
                //var message = Encoding.UTF8.GetBytes("<div><p>Bạn không có quyền truy cập vào đường dẫn này</p></div>");

                //Response.OnStarting(async () =>
                //{
                //    Response.ContentType = "text/html; charset=utf-8";
                //    _httpContextAccessor.HttpContext.Response.StatusCode = 429;
                //    await Response.Body.WriteAsync(message, 0, message.Length);
                //});

                var htmlFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "403Page.html");
                var htmlContent = File.ReadAllText(htmlFilePath);
                _httpContextAccessor.HttpContext.Response.ContentType = "text/html";
                _httpContextAccessor.HttpContext.Response.StatusCode = 403;
                _httpContextAccessor.HttpContext.Response.WriteAsync(htmlContent);
                Thread.Sleep(3000);
            }

            return Task.CompletedTask;
        }
    }
}

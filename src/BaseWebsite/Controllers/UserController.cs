using Application.Base;
using Application.Services.WebInterfaces;
using BaseWebsite.Authorization;
using Core.CommonModels.BaseModels;
using Core.Enums;
using Core.Helper;
using Core.Models.SearchModels;
using Core.Models.ViewModels;
using Core.Models.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseWebsite.Controllers
{
    [Authorize]
    public class UserController : BaseController<UserController>
    {
        public UserController(IUserService userService,
             ILogger<UserController> logger) : base(logger, userService)
        {
        }

        [PermissionAuthorization(true, functionIdList: [(int)EActionRole.ReadUser])]
        public IActionResult Index()
        {
            ViewBag.RoleDatasource = EnumHelper.ToDropdownList<ERoleType>();
            return View();
        }
        public async Task<IActionResult> Authentication()
        {
            var response = new Acknowledgement();
            if (User.Identity.IsAuthenticated)
            {
                var userAck = await UserService.GetUserById(int.Parse(_currentUserId));
                return MapToIActionResult(userAck);
            }
            return MapToIActionResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserDropdownList(string searchString, string selectedIdList)
        {
            var selectedIds = selectedIdList?.Split(',').Select(int.Parse).ToList();
            var result = await UserService.GetUserDataDropdownList(searchString, selectedIds ?? new List<int>());
            return MapToIActionResult(result);
        }
        [PermissionAuthorization(true, functionIdList: [(int)EActionRole.ReadUser])]
        [HttpPost]
        public async Task<IActionResult> GetUserList(UserSearchModel searchModel)
        {

            var result = await UserService.GetUserList(searchModel);
            return MapToIActionResult(result);
        }
        [PermissionAuthorization(true, functionIdList: [(int)EActionRole.DeleteUser])]
        [HttpGet]
        public async Task<Acknowledgement> DeleteUserById(int userId)
        {
            return await UserService.DeleteUserById(userId);
        }
        [PermissionAuthorization(true, functionIdList: [(int)EActionRole.UpdateUser])]
        [HttpGet]
        public async Task<Acknowledgement> ResetUserPasswordById(int userId)
        {
            return await UserService.ResetUserPasswordById(userId);
        }
        [PermissionAuthorization(true, functionIdList: [(int)EActionRole.CreateUser, (int)EActionRole.UpdateUser])]
        [HttpPost]
        public async Task<Acknowledgement> CreateOrUpdateUser([FromBody] UserViewModel postData)
        {
            return await UserService.CreateOrUpdateUser(postData);
        }
        [HttpPost]
        public async Task<Acknowledgement> ChangePassword([FromBody] ChangePasswordModel postData)
        {
            return await UserService.ChangePassword(postData);
        }
        [PermissionAuthorization(true, functionIdList: [(int)EActionRole.ReadUser, (int)EActionRole.UpdateUser])]
        [HttpGet]
        public async Task<Acknowledgement<UserViewModel>> GetUserById(int userId)
        {
            var ack = await UserService.GetUserById(userId);
            return ack;
        }
        public string Values()
        {
            return "Server is running";
        }


    }
}

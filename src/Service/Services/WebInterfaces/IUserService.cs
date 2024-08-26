using Core.CommonModels;
using Core.CommonModels.BaseModels;
using Core.Models.SearchModels;
using Core.Models.ViewModels;
using Core.Models.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.WebInterfaces
{
    public interface IUserService : IBaseService, IDisposable
    {
        Task<Acknowledgement<UserViewModel>> Login(LoginViewModel loginModel);
        Task<Acknowledgement> LockUser(string userName);

        Task<Acknowledgement<JsonResultPaging<List<UserViewModel>>>> GetUserList(UserSearchModel postData);
        Task<Acknowledgement<UserViewModel>> GetUserById(int userId);
        Task<Acknowledgement> CreateOrUpdateUser(UserViewModel postData);

        Task<Acknowledgement> DeleteUserById(int userId);
        Task<Acknowledgement> ResetUserPasswordById(int userId);
        Task<Acknowledgement> ChangePassword([FromBody] ChangePasswordModel postData);

        Task<Acknowledgement<List<KendoDropdownListModel<int>>>> GetUserDataDropdownList(string searchString, List<int> selectedIdList);

    }
}

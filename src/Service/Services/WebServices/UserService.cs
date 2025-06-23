using Application.Helpers;
using Application.Interfaces;
using Application.Services.AuthorPredicates;
using Application.Services.WebInterfaces;
using AutoMapper;
using Core.CommonModels;
using Core.CommonModels.BaseModels;
using Core.CoreUtils;
using Core.Enums;
using Core.Models.SearchModels;
using Core.Models.ViewModels;
using Core.Models.ViewModels.AccountViewModels;
using Infrastructure.Entitites;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;


namespace Application.Services.WebServices
{
    public class UserService : BaseService<UserService>, IUserService
    {
        private readonly IMapper _mapper;
        //private readonly TelegramService _telegramService;
        public UserService(
            ILogger<UserService> logger,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IMapper mapper
            //TelegramService telegramService
            ) : base(logger, configuration, userRepository, roleRepository, httpContextAccessor)
        {
            _mapper = mapper;
            //_telegramService = telegramService;
        }
        private async Task<Acknowledgement<User>> GetUser(string userName, string password)
        {
            var response = new Acknowledgement<User>();

            var hashPassword = Utils.EncodePassword(password, EEncodeType.SHA_256);
            //var userDB = (await _userRepository.ReadOnlyRespository.GetAsync(u => u.UserName.ToLower() == userName.ToLower() && u.State == (short)EState.Active)).FirstOrDefault();


            //if (userDB == null)
            //{
            //    response.AddMessage("Không tìm thấy người dùng!");
            //    return response;
            //}

            //if (!hashPassword.Equals(userDB.Password))
            //{
            //    response.AddMessage("Tên đăng nhập hoặc mật khẩu không đúng!");
            //    return response;
            //}
            response.Data = new User() { Id = 1};
            response.StatusCode = HttpStatusCode.OK;
            return response;

        }
        public async Task<Acknowledgement<UserViewModel>> Login(LoginViewModel loginModel)
        {
            //var ack = await _telegramService.SendMessageAsync("5247682503", "Test send telegram message!");
            var response = new Acknowledgement<UserViewModel>();
            try
            {
                var userResponse = await GetUser(loginModel.UserName, loginModel.Password);
                response.StatusCode = userResponse.StatusCode;
                if (userResponse.IsSuccess)
                {
                    var userDB = userResponse.Data;
                    //var roleDBList = await _roleRepository.ReadOnlyRespository.GetAsync(i => userDB.RoleIdList.Contains(i.Id));
                    UserViewModel userViewModel = _mapper.Map<UserViewModel>(userDB);
                    //userViewModel.RoleName = string.Join(",", roleDBList.Select(i => i.Description));
                    //userViewModel.EnumActionList = roleDBList.SelectMany(i => i.EnumActionList).Distinct().ToList();
                    response.Data = userViewModel;
                }
                else
                {
                    response.ErrorMessageList = userResponse.ErrorMessageList;
                }
            }
            catch (Exception ex)
            {
                response.ExtractMessage(ex);
            }

            return response;
        }
        public async Task<Acknowledgement> LockUser(string userName)
        {
            var response = new Acknowledgement();
            try
            {
                var user = await _userRepository.Repository.FirstOrDefaultAsync(u =>
                    u.UserName.ToLower() == userName.ToLower()
                    && u.State == (short)EState.Delete
                    );

                if (user != null)
                {
                    user.State = (short)EState.Active;
                    await response.TrySaveChangesAsync(res => res.UpdateAsync(user), _userRepository.Repository);
                }
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.ExtractMessage(ex);
                _logger.LogError("LockUser", ex.ToString());
            }
            return response;
        }
        public async Task<Acknowledgement<List<KendoDropdownListModel<int>>>> GetUserDataDropdownList(string searchString, List<int> selectedIdList)
        {
            var predicate = PredicateBuilder.New<User>(i => i.State == (int)EState.Active);
            predicate = UserAuthorPredicate.GetUserAuthorPredicate(predicate, CurrentUserRoles.ToList(), CurrentUserId);
            var selectedUserList = new List<User>();
            if (selectedIdList.Count > 0 && string.IsNullOrEmpty(searchString))
            {
                var tmpPredicate = PredicateBuilder.New<User>(predicate);
                tmpPredicate = tmpPredicate.And(i => selectedIdList.Contains(i.Id));
                selectedUserList = (await _userRepository.ReadOnlyRespository.GetAsync(tmpPredicate, i => i.OrderBy(p => p.Name))).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchStringNonUnicode = Utils.NonUnicode(searchString.Trim().ToLower());
                predicate = predicate.And(i => i.UserName.ToLower().Contains(searchStringNonUnicode) ||
                                                i.NameNonUnicode.Contains(searchStringNonUnicode)
                                         );
            }
            var userDbList = await _userRepository.ReadOnlyRespository.GetWithPagingAsync(new PagingParameters(1, 50 - selectedUserList.Count()), predicate, i => i.OrderBy(p => p.Name));
            var data = userDbList.Data.Concat(selectedUserList).Select(i => new KendoDropdownListModel<int>()
            {
                Value = i.Id.ToString(),
                Text = $"{i.Name} - {i.Phone}",
            }).ToList();
            return new Acknowledgement<List<KendoDropdownListModel<int>>>()
            {
                StatusCode = HttpStatusCode.OK,
                Data = data
            };
        }
        public async Task<Acknowledgement<JsonResultPaging<List<UserViewModel>>>> GetUserList(UserSearchModel searchModel)
        {
            var response = new Acknowledgement<JsonResultPaging<List<UserViewModel>>>();
            try
            {
                var predicate = PredicateBuilder.New<User>(i => i.State == (int)EState.Active);

                if (!string.IsNullOrEmpty(searchModel.SearchString))
                {
                    var searchStringNonUnicode = Utils.NonUnicode(searchModel.SearchString.Trim().ToLower());
                    predicate = predicate.And(i => i.UserName.ToLower().Contains(searchStringNonUnicode) ||
                                                    i.NameNonUnicode.Contains(searchStringNonUnicode) ||
                                                    !string.IsNullOrEmpty(i.Phone) && i.Phone.ToLower().Contains(searchStringNonUnicode)
                                             );
                }
                if (searchModel.RoleIdList.Count > 0)
                {
                    predicate = predicate.And(p => p.RoleIdList.Intersect(searchModel.RoleIdList).Any());
                }

                var userList = new List<UserViewModel>();
                predicate = UserAuthorPredicate.GetUserAuthorPredicate(predicate, CurrentUserRoles.ToList(), CurrentUserId);
                var userDbQuery = await _userRepository.ReadOnlyRespository.GetWithPagingAsync(
                    new PagingParameters(searchModel.PageNumber, searchModel.PageSize),
                    predicate,
                    i => i.OrderByDescending(u => u.UpdatedDate)
                    );
                var userDBList = _mapper.Map<List<UserViewModel>>(userDbQuery.Data);
                var roleDBList = await _roleRepository.ReadOnlyRespository.GetAsync();
                var updateByUserIdList = userDBList.Select(i => i.UpdatedBy).ToList();
                var updateByUserList = await _userRepository.ReadOnlyRespository.GetAsync(i => updateByUserIdList.Contains(i.Id));
                foreach (var user in userDBList)
                {
                    var roles = roleDBList.Where(j => user.RoleIdList.Contains(j.Id)).ToList();
                    user.RoleViewList = _mapper.Map<List<RoleViewModel>>(roles);

                    var updateUser = updateByUserList.First(i => i.Id == user.UpdatedBy);
                    user.UpdatedByName = updateUser.Name;
                }
                response.Data = new JsonResultPaging<List<UserViewModel>>()
                {
                    Data = userDBList,
                    PageNumber = searchModel.PageNumber,
                    PageSize = searchModel.PageSize,
                    Total = userDbQuery.TotalRecords
                };
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.ExtractMessage(ex);
                _logger.LogError("GetUserList " + ex.Message);
                return response;

            }
        }
        public async Task<Acknowledgement> CreateOrUpdateUser(UserViewModel postData)
        {
            var ack = new Acknowledgement();
            if (string.IsNullOrWhiteSpace(postData.Name))
            {
                ack.AddMessage("Vui lòng nhập họ tên");
                return ack;
            }
            if (postData.Id == 0 && !Validate.IsValidPassword(postData.Password))
            {
                ack.AddMessage("Mật khẩu không đúng định dạng. (Ít nhất 8 kí tự, 1 chữ hoa,1 chữ thường, 1 chữ số và 1 kí tự đặc biệt)");
                return ack;
            }
            if (string.IsNullOrWhiteSpace(postData.UserName))
            {
                ack.AddMessage("Vui lòng nhập tên đăng nhập");
                return ack;
            }
            var phone = postData.Phone;
            var validatePhoneMessage = Validate.ValidPhoneNumber(ref phone);
            if (validatePhoneMessage != null)
            {
                ack.AddMessage(validatePhoneMessage);
                return ack;
            }
            postData.Phone = phone;
            if (!string.IsNullOrWhiteSpace(postData.Email))
            {
                var isValidEmail = Validate.ValidEmail(postData.Email);
                if (!isValidEmail)
                {
                    ack.AddMessage("Email không đúng định dạng.");
                    return ack;
                }
            }
            if (postData.RoleIdList.Count() == 0)
            {
                ack.AddMessage("Vui lòng chọn vai trò.");
                return ack;
            }
            //postData.Password = Utils.EncodePassword(postData.Password, EEncodeType.SHA_256);
            var defaultResetPassword = Configuration.GetSection("DefaultResetPassword").Value;
            postData.Password = defaultResetPassword;
            var checkSameUserNameItem = await _userRepository.Repository.FirstOrDefaultAsync(i => i.UserName == postData.UserName && i.State == (int)EState.Active);
            if (checkSameUserNameItem != null && postData.Id == 0)
            {
                ack.AddMessage("Đã tồn tại tài khoản với tên đăng nhập này");
                return ack;
            }

            if (postData.Id == 0)
            {
                var newUser = _mapper.Map<User>(postData);
                newUser.NameNonUnicode = Utils.NonUnicode(newUser.Name);
                newUser.Password = postData.Password;
                newUser.CreatedDate = DateTime.Now;
                newUser.CreatedBy = CurrentUserId;
                newUser.UpdatedDate = newUser.CreatedDate;
                newUser.UpdatedBy = newUser.CreatedBy;
                await ack.TrySaveChangesAsync(res => res.AddAsync(newUser), _userRepository.Repository);
            }
            else
            {
                var existItem = await _userRepository.Repository.FirstOrDefaultAsync(i => i.Id == postData.Id && i.State == (int)EState.Active);
                if (existItem == null)
                {
                    ack.AddMessage("Không tìm thấy người dùng");
                    ack.StatusCode = HttpStatusCode.BadRequest;
                    return ack;
                }
                else
                {
                    existItem.Name = postData.Name;
                    existItem.Phone = postData.Phone;
                    existItem.Email = postData.Email;
                    existItem.UserName = postData.UserName;
                    existItem.NameNonUnicode = Utils.NonUnicode(postData.Name);
                    existItem.UpdatedDate = DateTime.Now;
                    existItem.UpdatedBy = CurrentUserId;
                    await ack.TrySaveChangesAsync(res => res.UpdateAsync(existItem), _userRepository.Repository);
                }
            }
            return ack;
        }
        public async Task<Acknowledgement> DeleteUserById(int userId)
        {
            var ack = new Acknowledgement();
            var user = await _userRepository.Repository.FirstOrDefaultAsync(i => i.Id == userId, "UrnList,UrnList.Urn");
            if (user == null)
            {
                ack.AddMessage("Không tìm thấy người dùng");
                return ack;
            }
            user.State = (int)EState.Delete;
            await ack.TrySaveChangesAsync(res => res.UpdateAsync(user), _userRepository.Repository);
            return ack;
        }
        public async Task<Acknowledgement> ResetUserPasswordById(int userId)
        {
            var ack = new Acknowledgement();
            var user = await _userRepository.Repository.FirstOrDefaultAsync(i => i.Id == userId, "UrnList,UrnList.Urn");
            if (user == null)
            {
                ack.AddMessage("Không tìm thấy người dùng");
                return ack;
            }
            var defaultResetPassword = Configuration.GetSection("DefaultResetPassword").Value;
            if (string.IsNullOrEmpty(defaultResetPassword))
            {
                ack.AddMessage("Lỗi thiếu setting mật khẩu mặc định");
                return ack;
            }
            user.Password = Utils.EncodePassword(defaultResetPassword, EEncodeType.SHA_256);
            await ack.TrySaveChangesAsync(res => res.UpdateAsync(user), _userRepository.Repository);
            return ack;
        }

        public async Task<Acknowledgement<UserViewModel>> GetUserById(int userId)
        {
            var ack = new Acknowledgement<UserViewModel>();
            try
            {
                var user = await _userRepository.ReadOnlyRespository.FindAsync(userId);
                if (user == null)
                {
                    ack.StatusCode = HttpStatusCode.BadRequest;
                    ack.AddMessages("Không tìm thấy user");
                    return ack;
                }

                ack.Data = _mapper.Map<UserViewModel>(user);
                ack.StatusCode = HttpStatusCode.OK;
                if (user.RoleIdList.Count > 0)
                {
                    var predicate = PredicateBuilder.New<Role>(i => i.State == (int)EState.Active);
                    predicate = predicate.And(e => user.RoleIdList.Contains(e.Id));

                    var listRole = await _roleRepository.ReadOnlyRespository.GetAsync(predicate);
                    if (listRole != null)
                    {
                        var listPermission = new List<int>();
                        listPermission = listRole.SelectMany(e => e.EnumActionList).Distinct().ToList();
                        ack.Data.EnumActionList = listPermission;
                    }
                }

                return ack;

            }
            catch (Exception ex)
            {
                ack.ExtractMessage(ex);
                _logger.LogError("GetUserList " + ex.Message);
                return ack;
            }
        }
        public async Task<Acknowledgement> ChangePassword(ChangePasswordModel postData)
        {
            var ack = new Acknowledgement();
            #region Validate
            if (postData.NewPassword != postData.RepeatPassword)
            {
                ack.AddMessage("Xác nhận mật khẩu không giống mật khẩu mới.");
                return ack;
            }
            if (!Validate.IsValidPassword(postData.NewPassword))
            {
                ack.AddMessage("Mật khẩu không đúng định dạng. (Ít nhất 8 kí tự, 1 chữ hoa,1 chữ thường, 1 chữ số và 1 kí tự đặc biệt)");
                return ack;
            }
            #endregion

            postData.OldPassword = Utils.EncodePassword(postData.OldPassword, EEncodeType.SHA_256);
            postData.NewPassword = Utils.EncodePassword(postData.NewPassword, EEncodeType.SHA_256);
            var user = await _userRepository.Repository.FirstOrDefaultAsync(i => i.Id == CurrentUserId);
            if (user == null)
            {
                ack.AddMessage("Không tìm thấy người dùng.");
                return ack;
            }
            if (user.Password != postData.OldPassword)
            {
                ack.AddMessage("Mật khẩu cũ không chính xác.");
                return ack;
            }
            user.Password = postData.NewPassword;
            user.UpdatedBy = CurrentUserId;
            user.UpdatedDate = DateTime.Now;
            await ack.TrySaveChangesAsync(res => res.UpdateAsync(user), _userRepository.Repository);
            return ack;
        }

        /// <summary>
        /// Gets the current user ID from the authentication context - Optimized version
        /// </summary>
        /// <returns>Current user ID and authentication status</returns>
        public async Task<Acknowledgement<object>> GetCurrentUserId()
        {
            var ack = new Acknowledgement<object>();
            try
            {
                // Sử dụng CurrentUserId property - đã tối ưu và readonly
                if (IsAuthenticated)
                {
                    // Optionally get user details
                    var user = await _userRepository.ReadOnlyRespository.FindAsync(CurrentUserId);

                    ack.Data = new
                    {
                        UserId = CurrentUserId,
                        IsAuthenticated = IsAuthenticated,
                        UserName = user?.UserName ?? "Unknown",
                        Name = user?.Name ?? "Unknown",
                        Roles = CurrentUserRoles.Select(r => new {
                            Id = (int)r,
                            Name = r.ToString()
                        }).ToList(),
                        HasAdminRole = _IsHasAdminRole(),
                        Message = "Current user retrieved successfully"
                    };
                    ack.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    ack.Data = new
                    {
                        UserId = (int?)null,
                        IsAuthenticated = false,
                        UserName = (string?)null,
                        Name = (string?)null,
                        Roles = new List<object>(),
                        HasAdminRole = false,
                        Message = "User is not authenticated"
                    };
                    ack.StatusCode = HttpStatusCode.Unauthorized;
                }

                return ack;
            }
            catch (Exception ex)
            {
                ack.ExtractMessage(ex);
                _logger.LogError(ex, "Error getting current user ID");
                return ack;
            }
        }

    }
}

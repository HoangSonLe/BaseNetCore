using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Core.Models.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Nhớ đăng nhập?")]
        public bool RememberMe { get; set; }
        //public string? ReturnUrl { get; set; }
        public int AccountType { get; set; }
        public bool IsMobile { get; set; } = false;
        public string? Captcha { get; set; }
    }
    public class LogoutModel
    {
        public int UserId { get; set; }
    }
    public class UserLoginResponse
    {
        public string UserName { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}

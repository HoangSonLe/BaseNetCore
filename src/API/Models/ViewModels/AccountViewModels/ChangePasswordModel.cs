using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.ViewModels.AccountViewModels
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password, ErrorMessage = "Mật khẩu cũ không hợp lệ")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không hợp lệ")]
        [DataType(DataType.Password, ErrorMessage = "Mật khẩu không hợp lệ")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Độ dài từ 3-255 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "'Mật khẩu mới' và 'Xác nhận mật khẩu' không khớp, Vui lòng nhập lại")]
        [DataType(DataType.Password, ErrorMessage = "Mật khẩu không hợp lệ")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Độ dài từ 3-255 ký tự")]
        public string RepeatPassword { get; set; } = string.Empty;
    }
}

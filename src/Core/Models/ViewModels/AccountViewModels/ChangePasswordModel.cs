using System.ComponentModel.DataAnnotations;

namespace Core.Models.ViewModels.AccountViewModels
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string RepeatPassword { get; set; } = string.Empty;
    }
}

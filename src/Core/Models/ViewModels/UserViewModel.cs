using AutoMapper.Configuration.Annotations;

namespace Core.Models.ViewModels
{
    public class UserViewModel : BaseAuditableViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        /// <summary>
        /// Không dùng nữa
        /// </summary>
        //public int TypeAccount { get; set; }
        public List<int> RoleIdList { get; set; }
        [Ignore]
        public List<RoleViewModel> RoleViewList { get; set; }
        [Ignore]
        public List<int> EnumActionList { get; set; }
        [Ignore]
        public string RoleName { get; set; }
        public string UpdatedByName { get; set; }



    }
}

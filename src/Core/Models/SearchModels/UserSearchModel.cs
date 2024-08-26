using Core.CommonModels;
using Core.Models.ViewModels;

namespace Core.Models.SearchModels
{
    public class UserSearchModel : SearchPagingModel<UserViewModel>
    {
        public string SearchString { get; set; }
        public List<int> RoleIdList { get; set; } = new List<int>();
    }
}

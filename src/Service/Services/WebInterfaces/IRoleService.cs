using Core.CommonModels;
using Core.CommonModels.BaseModels;

namespace Application.Services.WebInterfaces
{
    public interface IRoleService : IBaseService, IDisposable
    {
        Task<Acknowledgement<List<KendoDropdownListModel<int>>>> GetRoleDropdownList(string searchString);

    }
}

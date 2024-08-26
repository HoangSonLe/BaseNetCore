using Application.Interfaces;
using Application.Services.WebInterfaces;
using Core.CommonModels;
using Core.CommonModels.BaseModels;
using Core.CoreUtils;
using Infrastructure.Entitites;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.WebServices
{
    public class RoleService : BaseService<RoleService>, IRoleService
    {
        public RoleService(
            ILogger<RoleService> logger,
            IConfiguration configuration,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IHttpContextAccessor httpContextAccessor
            ) : base(logger, configuration, userRepository, roleRepository, httpContextAccessor)
        {
        }

        public async Task<Acknowledgement<List<KendoDropdownListModel<int>>>> GetRoleDropdownList(string searchString)
        {
            var response = new Acknowledgement<List<KendoDropdownListModel<int>>>();
            try
            {
                var maxLevel = Utils.GetMaxLevelRole(_currentUserRoleId);
                var predicate = PredicateBuilder.New<Role>(i => i.Level > maxLevel);
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.Trim().ToLower().NonUnicode();
                    predicate = predicate.And(i => i.NameNonUnicode.Trim().ToLower() == searchString.ToLower());
                }
                var roleList = (await _roleRepository.Repository.GetAsync(predicate))
                                                   .Select(i => new KendoDropdownListModel<int>()
                                                   {
                                                       Value = i.Id.ToString(),
                                                       Text = i.Description,
                                                   })
                                                   .ToList();
                response.Data = roleList;
                response.IsSuccess = true;
                _logger.LogError("GetUserList " + "Tét");
                return response;

            }
            catch (Exception ex)
            {
                response.ExtractMessage(ex);
                _logger.LogError("GetUserList " + ex.Message);
                return response;

            }
        }
    }
}

using Core.CommonModels;
using Microsoft.AspNetCore.Authorization;

namespace BaseWebsite.Authorizations
{
    public class C3FunctionAuthorizationRequirement : IAuthorizationRequirement
    {
        public Permission PermissionId { get; private set; }
        public C3FunctionAuthorizationRequirement(Permission permissionId)
        {
            PermissionId = permissionId;
        }
    }
}

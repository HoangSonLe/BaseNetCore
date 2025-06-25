using Core.CommonModels;
using Microsoft.AspNetCore.Authorization;

namespace BaseWebsite.Authorization
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public Permission PermissionId { get; private set; }
        public PermissionAuthorizationRequirement(Permission permissionId)
        {
            PermissionId = permissionId;
        }
    }
}

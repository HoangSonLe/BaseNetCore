using Core.CommonModels;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace BaseWebsite.Authorization
{
    public class PermissionAuthorizationAttribute : AuthorizeAttribute
    {
        public PermissionAuthorizationAttribute(bool redirect = false, params int[] functionIdList)
        {
            PermissionId = new()
            {
                ListPermission = functionIdList.ToList(),
                Redirect = redirect
            };
        }

        public Permission PermissionId
        {
            get
            {
                return new Permission();
            }
            set
            {

                Policy = JsonConvert.SerializeObject(value);
            }
        }


    }
}

using Core.CommonModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BaseWebsite.Authorizations
{
    public class C3FunctionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "UserFunction";
        public DefaultAuthorizationPolicyProvider defaultPolicyProvider { get; }
        public C3FunctionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return defaultPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var permission = JsonConvert.DeserializeObject<Permission>(policyName);

            if (permission.ListPermission.Count > 0)
            {
                var functionID = policyName.Substring(POLICY_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.AddRequirements(new C3FunctionAuthorizationRequirement(permission));
                return Task.FromResult(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}

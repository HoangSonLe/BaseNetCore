//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Primitives;
//using System.IdentityModel.Tokens.Jwt;

//public class InitTokenMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly IMemoryCache _memoryCache;
//    public InitTokenMiddleware(RequestDelegate next, IMemoryCache memoryCache)
//    {
//        _next = next;
//        _memoryCache = memoryCache;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        var userId = string.Empty;
//        var orgranizationUserCBCS = string.Empty;
//        JwtSecurityToken jwtToken = new();
//        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//        if (token != null)
//        {
//            jwtToken = new JwtSecurityToken(token);
//            userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

//            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(orgranizationUserCBCS))
//            {
//                var queryCollection = new Dictionary<string, StringValues>();
//                queryCollection["userId"] = userId;
//                queryCollection["orgranizationUserCBCS"] = orgranizationUserCBCS;
//                foreach (var key in context.Request.Query.Keys)
//                {
//                    queryCollection[key] = context.Request.Query[key];
//                }

//                var newQueryCollection = new QueryCollection(queryCollection);
//                context.Request.Query = newQueryCollection;
//                await _next(context);
//            }
//        }
//        else
//        {
//            await _next(context);
//        }
//    }
//}


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Application.Attributes
{
    /// <summary>
    /// Attribute to cache API responses
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CacheResponseAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _durationInSeconds;
        private readonly bool _varyByUser;
        private readonly bool _varyByQueryString;

        /// <summary>
        /// Initialize cache response attribute
        /// </summary>
        /// <param name="durationInSeconds">Cache duration in seconds</param>
        /// <param name="varyByUser">Whether to vary cache by user</param>
        /// <param name="varyByQueryString">Whether to vary cache by query string</param>
        public CacheResponseAttribute(
            int durationInSeconds = 300, 
            bool varyByUser = false, 
            bool varyByQueryString = true)
        {
            _durationInSeconds = durationInSeconds;
            _varyByUser = varyByUser;
            _varyByQueryString = varyByQueryString;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<CacheResponseAttribute>>();
            
            try
            {
                // Generate cache key
                var cacheKey = GenerateCacheKey(context);
                
                // Try to get from cache (in-memory for now, could be Redis later)
                var cachedResponse = GetFromCache(context.HttpContext, cacheKey);
                if (cachedResponse != null)
                {
                    logger?.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                    context.Result = cachedResponse;
                    return;
                }

                // Execute action
                var executedContext = await next();

                // Cache the response if successful
                if (executedContext.Result is ObjectResult objectResult && 
                    objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
                {
                    logger?.LogDebug("Caching response for key: {CacheKey}", cacheKey);
                    SetCache(context.HttpContext, cacheKey, objectResult);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error in cache response filter");
                // Continue without caching
                await next();
            }
        }

        private string GenerateCacheKey(ActionExecutingContext context)
        {
            var keyBuilder = new StringBuilder();
            
            // Add controller and action
            keyBuilder.Append($"{context.Controller.GetType().Name}_{context.ActionDescriptor.DisplayName}");
            
            // Add user ID if varying by user
            if (_varyByUser)
            {
                var userId = context.HttpContext.User?.FindFirst("UserID")?.Value ?? "anonymous";
                keyBuilder.Append($"_user_{userId}");
            }
            
            // Add query string if varying by query string
            if (_varyByQueryString && context.HttpContext.Request.QueryString.HasValue)
            {
                keyBuilder.Append($"_query_{context.HttpContext.Request.QueryString.Value}");
            }
            
            // Add action parameters
            foreach (var param in context.ActionArguments)
            {
                if (param.Value != null)
                {
                    var paramValue = param.Value is string str ? str : JsonSerializer.Serialize(param.Value);
                    keyBuilder.Append($"_param_{param.Key}_{paramValue.GetHashCode()}");
                }
            }
            
            return keyBuilder.ToString();
        }

        private ObjectResult? GetFromCache(HttpContext context, string cacheKey)
        {
            // Simple in-memory cache using HttpContext.Items
            // In production, you might want to use IMemoryCache or Redis
            var cacheStore = GetCacheStore(context);
            
            if (cacheStore.TryGetValue(cacheKey, out var cachedItem) && cachedItem is CachedResponse cached)
            {
                if (cached.ExpiresAt > DateTime.UtcNow)
                {
                    return new ObjectResult(cached.Data)
                    {
                        StatusCode = cached.StatusCode
                    };
                }
                else
                {
                    // Remove expired item
                    cacheStore.Remove(cacheKey);
                }
            }
            
            return null;
        }

        private void SetCache(HttpContext context, string cacheKey, ObjectResult result)
        {
            var cacheStore = GetCacheStore(context);
            
            var cachedResponse = new CachedResponse
            {
                Data = result.Value,
                StatusCode = result.StatusCode ?? 200,
                ExpiresAt = DateTime.UtcNow.AddSeconds(_durationInSeconds)
            };
            
            cacheStore[cacheKey] = cachedResponse;
            
            // Clean up expired items periodically (simple approach)
            CleanupExpiredItems(cacheStore);
        }

        private static Dictionary<string, object> GetCacheStore(HttpContext context)
        {
            const string cacheStoreKey = "ResponseCache";
            
            if (!context.Items.ContainsKey(cacheStoreKey))
            {
                context.Items[cacheStoreKey] = new Dictionary<string, object>();
            }
            
            return (Dictionary<string, object>)context.Items[cacheStoreKey]!;
        }

        private static void CleanupExpiredItems(Dictionary<string, object> cacheStore)
        {
            // Simple cleanup - remove expired items
            // In production, you might want a more sophisticated cleanup strategy
            var expiredKeys = cacheStore
                .Where(kvp => kvp.Value is CachedResponse cached && cached.ExpiresAt <= DateTime.UtcNow)
                .Select(kvp => kvp.Key)
                .ToList();
            
            foreach (var key in expiredKeys)
            {
                cacheStore.Remove(key);
            }
        }

        private class CachedResponse
        {
            public object? Data { get; set; }
            public int StatusCode { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }

    /// <summary>
    /// Attribute to disable caching for specific actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NoCacheAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult)
            {
                context.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                context.HttpContext.Response.Headers.Add("Pragma", "no-cache");
                context.HttpContext.Response.Headers.Add("Expires", "0");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Nothing to do before action execution
        }
    }

    /// <summary>
    /// Cache profiles for common scenarios
    /// </summary>
    public static class CacheProfiles
    {
        /// <summary>
        /// Short cache for frequently changing data (5 minutes)
        /// </summary>
        public class ShortCache : CacheResponseAttribute
        {
            public ShortCache() : base(300, false, true) { }
        }

        /// <summary>
        /// Medium cache for moderately changing data (30 minutes)
        /// </summary>
        public class MediumCache : CacheResponseAttribute
        {
            public MediumCache() : base(1800, false, true) { }
        }

        /// <summary>
        /// Long cache for rarely changing data (2 hours)
        /// </summary>
        public class LongCache : CacheResponseAttribute
        {
            public LongCache() : base(7200, false, true) { }
        }

        /// <summary>
        /// User-specific cache (15 minutes, varies by user)
        /// </summary>
        public class UserCache : CacheResponseAttribute
        {
            public UserCache() : base(900, true, true) { }
        }
    }
}

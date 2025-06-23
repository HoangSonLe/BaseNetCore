using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Extensions
{
    /// <summary>
    /// Extension methods for easy cookie memory operations
    /// </summary>
    public static class CookieMemoryExtensions
    {
        /// <summary>
        /// Get cookie memory service from HttpContext
        /// </summary>
        public static ICookieMemoryService GetCookieMemory(this HttpContext context)
        {
            var service = context.RequestServices.GetService(typeof(ICookieMemoryService)) as ICookieMemoryService;
            if (service == null)
                throw new InvalidOperationException("CookieMemoryService is not registered. Please register it in Program.cs");
            return service;
        }

        /// <summary>
        /// Get cookie memory service from Controller
        /// </summary>
        public static ICookieMemoryService GetCookieMemory(this Controller controller)
        {
            return controller.HttpContext.GetCookieMemory();
        }

        /// <summary>
        /// Store user preferences with easy access
        /// </summary>
        public static void SetUserPreferences(this HttpContext context, int userId, UserPreferences preferences)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.SetUserPreferences(userId, preferences);
        }

        /// <summary>
        /// Get user preferences with easy access
        /// </summary>
        public static UserPreferences? GetUserPreferences(this HttpContext context, int userId)
        {
            var cookieMemory = context.GetCookieMemory();
            return cookieMemory.GetUserPreferences<UserPreferences>(userId);
        }

        /// <summary>
        /// Store session data with easy access
        /// </summary>
        public static void SetSessionData(this HttpContext context, SessionData sessionData)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Set("SessionData", sessionData);
        }

        /// <summary>
        /// Get session data with easy access
        /// </summary>
        public static SessionData? GetSessionData(this HttpContext context)
        {
            var cookieMemory = context.GetCookieMemory();
            return cookieMemory.Get<SessionData>("SessionData");
        }

        /// <summary>
        /// Store navigation memory with easy access
        /// </summary>
        public static void SetNavigationMemory(this HttpContext context, NavigationMemoryData navigationData)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Set("NavigationMemory", navigationData);
        }

        /// <summary>
        /// Get navigation memory with easy access
        /// </summary>
        public static NavigationMemoryData? GetNavigationMemory(this HttpContext context)
        {
            var cookieMemory = context.GetCookieMemory();
            return cookieMemory.Get<NavigationMemoryData>("NavigationMemory");
        }

        /// <summary>
        /// Store security memory with easy access
        /// </summary>
        public static void SetSecurityMemory(this HttpContext context, SecurityMemoryData securityData)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Set("SecurityMemory", securityData);
        }

        /// <summary>
        /// Get security memory with easy access
        /// </summary>
        public static SecurityMemoryData? GetSecurityMemory(this HttpContext context)
        {
            var cookieMemory = context.GetCookieMemory();
            return cookieMemory.Get<SecurityMemoryData>("SecurityMemory");
        }

        /// <summary>
        /// Remember current page for navigation
        /// </summary>
        public static void RememberCurrentPage(this HttpContext context, string pageName, string url)
        {
            var navigationData = context.GetNavigationMemory() ?? new NavigationMemoryData();
            
            // Add to recent URLs (keep only last 10)
            var sessionData = context.GetSessionData() ?? new SessionData();
            sessionData.RecentUrls.Insert(0, url);
            if (sessionData.RecentUrls.Count > 10)
                sessionData.RecentUrls = sessionData.RecentUrls.Take(10).ToList();
            
            context.SetSessionData(sessionData);
        }

        /// <summary>
        /// Remember search query
        /// </summary>
        public static void RememberSearch(this HttpContext context, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return;

            var sessionData = context.GetSessionData() ?? new SessionData();
            
            // Remove if already exists to avoid duplicates
            sessionData.SearchHistory.RemoveAll(s => s.Equals(searchQuery, StringComparison.OrdinalIgnoreCase));
            
            // Add to beginning of list
            sessionData.SearchHistory.Insert(0, searchQuery);
            
            // Keep only last 20 searches
            if (sessionData.SearchHistory.Count > 20)
                sessionData.SearchHistory = sessionData.SearchHistory.Take(20).ToList();
            
            context.SetSessionData(sessionData);
        }

        /// <summary>
        /// Get recent searches
        /// </summary>
        public static List<string> GetRecentSearches(this HttpContext context, int count = 10)
        {
            var sessionData = context.GetSessionData();
            return sessionData?.SearchHistory.Take(count).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Store form data for auto-save functionality
        /// </summary>
        public static void AutoSaveForm(this HttpContext context, string formId, Dictionary<string, string> formData)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.SetFormMemory(formId, formData, 120); // 2 hours
        }

        /// <summary>
        /// Get auto-saved form data
        /// </summary>
        public static Dictionary<string, string>? GetAutoSavedForm(this HttpContext context, string formId)
        {
            var cookieMemory = context.GetCookieMemory();
            return cookieMemory.GetFormMemory(formId);
        }

        /// <summary>
        /// Clear auto-saved form data
        /// </summary>
        public static void ClearAutoSavedForm(this HttpContext context, string formId)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.ClearFormMemory(formId);
        }

        /// <summary>
        /// Store selection memory (for bulk operations)
        /// </summary>
        public static void SetSelectionMemory(this HttpContext context, string context_name, List<int> selectedIds, Dictionary<string, string>? metadata = null)
        {
            var selectionData = new SelectionMemoryData
            {
                Context = context_name,
                SelectedIds = selectedIds,
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Set($"Selection_{context_name}", selectionData, 1); // 1 day
        }

        /// <summary>
        /// Get selection memory
        /// </summary>
        public static SelectionMemoryData? GetSelectionMemory(this HttpContext context, string context_name)
        {
            var cookieMemory = context.GetCookieMemory();
            return cookieMemory.Get<SelectionMemoryData>($"Selection_{context_name}");
        }

        /// <summary>
        /// Clear selection memory
        /// </summary>
        public static void ClearSelectionMemory(this HttpContext context, string context_name)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Remove($"Selection_{context_name}");
        }

        /// <summary>
        /// Store custom memory data
        /// </summary>
        public static void SetCustomMemory(this HttpContext context, string key, object data, string category = "", List<string>? tags = null, int expireDays = 30)
        {
            var customData = new CustomMemoryData
            {
                Category = category,
                Data = new Dictionary<string, object> { { "value", data } },
                Tags = tags ?? new List<string>()
            };

            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Set($"Custom_{key}", customData, expireDays);
        }

        /// <summary>
        /// Get custom memory data
        /// </summary>
        public static T? GetCustomMemory<T>(this HttpContext context, string key) where T : class
        {
            var cookieMemory = context.GetCookieMemory();
            var customData = cookieMemory.Get<CustomMemoryData>($"Custom_{key}");
            
            if (customData?.Data.ContainsKey("value") == true)
            {
                var value = customData.Data["value"];
                if (value is T directValue)
                    return directValue;
                
                // Try to convert from JSON if it's a string
                if (value is string jsonValue)
                {
                    try
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonValue);
                    }
                    catch
                    {
                        // Ignore conversion errors
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// Clear custom memory data
        /// </summary>
        public static void ClearCustomMemory(this HttpContext context, string key)
        {
            var cookieMemory = context.GetCookieMemory();
            cookieMemory.Remove($"Custom_{key}");
        }
    }
}

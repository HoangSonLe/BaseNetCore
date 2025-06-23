using Core.Extensions;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseWebsite.Controllers
{
    /// <summary>
    /// Test controller for demonstrating cookie memory functionality
    /// Remove this controller in production
    /// </summary>
    [Authorize]
    public class TestCookieMemoryController : Controller
    {
        private readonly ICookieMemoryService _cookieMemoryService;
        private readonly ILogger<TestCookieMemoryController> _logger;

        public TestCookieMemoryController(
            ICookieMemoryService cookieMemoryService,
            ILogger<TestCookieMemoryController> logger)
        {
            _cookieMemoryService = cookieMemoryService;
            _logger = logger;
        }

        /// <summary>
        /// Test page for cookie memory functionality
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.Message = "Cookie Memory Test Page";
            return View();
        }

        /// <summary>
        /// Test basic cookie operations
        /// </summary>
        [HttpPost]
        public IActionResult TestBasicOperations()
        {
            try
            {
                // Test string storage
                _cookieMemoryService.Set("test_string", "Hello World!", 1);
                var retrievedString = _cookieMemoryService.Get("test_string");
                
                // Test object storage
                var testObject = new { Name = "Test", Value = 123, Date = DateTime.Now };
                _cookieMemoryService.Set("test_object", testObject, 1);
                var retrievedObject = _cookieMemoryService.Get<object>("test_object");
                
                // Test existence check
                var exists = _cookieMemoryService.Exists("test_string");
                
                var result = new
                {
                    success = true,
                    stringTest = new { stored = "Hello World!", retrieved = retrievedString, match = retrievedString == "Hello World!" },
                    objectTest = new { stored = testObject, retrieved = retrievedObject },
                    existsTest = exists
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in basic operations test");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Test user preferences
        /// </summary>
        [HttpPost]
        public IActionResult TestUserPreferences()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID")?.Value ?? "1");
                
                // Create test preferences
                var preferences = new UserPreferences
                {
                    Language = "vi-VN",
                    Theme = "dark",
                    PageSize = 25,
                    ShowNotifications = true,
                    CustomSettings = new Dictionary<string, string>
                    {
                        { "sidebar_collapsed", "true" },
                        { "default_view", "grid" }
                    }
                };
                
                // Store preferences
                HttpContext.SetUserPreferences(userId, preferences);
                
                // Retrieve preferences
                var retrievedPrefs = HttpContext.GetUserPreferences(userId);
                
                var result = new
                {
                    success = true,
                    userId = userId,
                    stored = preferences,
                    retrieved = retrievedPrefs,
                    match = retrievedPrefs?.Language == preferences.Language
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in user preferences test");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Test search memory
        /// </summary>
        [HttpPost]
        public IActionResult TestSearchMemory()
        {
            try
            {
                // Add some test searches
                var searches = new[] { "test search 1", "test search 2", "test search 3" };
                
                foreach (var search in searches)
                {
                    HttpContext.RememberSearch(search);
                }
                
                // Get recent searches
                var recentSearches = HttpContext.GetRecentSearches(5);
                
                var result = new
                {
                    success = true,
                    addedSearches = searches,
                    retrievedSearches = recentSearches,
                    count = recentSearches.Count
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in search memory test");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Test form auto-save
        /// </summary>
        [HttpPost]
        public IActionResult TestFormAutoSave()
        {
            try
            {
                var formId = "test-form";
                var formData = new Dictionary<string, string>
                {
                    { "name", "John Doe" },
                    { "email", "john@example.com" },
                    { "message", "This is a test message" },
                    { "category", "support" }
                };
                
                // Save form data
                HttpContext.AutoSaveForm(formId, formData);
                
                // Retrieve form data
                var retrievedData = HttpContext.GetAutoSavedForm(formId);
                
                var result = new
                {
                    success = true,
                    formId = formId,
                    stored = formData,
                    retrieved = retrievedData,
                    match = retrievedData?.Count == formData.Count
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in form auto-save test");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Test login memory
        /// </summary>
        [HttpPost]
        public IActionResult TestLoginMemory()
        {
            try
            {
                var username = User.Identity?.Name ?? "testuser";
                var additionalData = new Dictionary<string, string>
                {
                    { "last_ip", "192.168.1.1" },
                    { "user_agent", "Test Browser" },
                    { "login_method", "password" }
                };
                
                // Set login memory
                _cookieMemoryService.SetLoginMemory(username, true, additionalData, 30);
                
                // Get login memory
                var loginMemory = _cookieMemoryService.GetLoginMemory();
                
                var result = new
                {
                    success = true,
                    stored = new { username, rememberMe = true, additionalData },
                    retrieved = loginMemory,
                    match = loginMemory?.Username == username
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in login memory test");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Test custom memory
        /// </summary>
        [HttpPost]
        public IActionResult TestCustomMemory()
        {
            try
            {
                var customData = new
                {
                    settings = new { theme = "blue", layout = "compact" },
                    preferences = new[] { "option1", "option2", "option3" },
                    metadata = new { version = "1.0", created = DateTime.Now }
                };
                
                // Store custom data
                HttpContext.SetCustomMemory("test_custom", customData, "test", new List<string> { "test", "demo" }, 1);
                
                // Retrieve custom data
                var retrievedData = HttpContext.GetCustomMemory<object>("test_custom");
                
                var result = new
                {
                    success = true,
                    stored = customData,
                    retrieved = retrievedData,
                    hasData = retrievedData != null
                };
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in custom memory test");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Clear all test cookies
        /// </summary>
        [HttpPost]
        public IActionResult ClearTestCookies()
        {
            try
            {
                _cookieMemoryService.Remove("test_string");
                _cookieMemoryService.Remove("test_object");
                _cookieMemoryService.ClearLoginMemory();
                HttpContext.ClearAutoSavedForm("test-form");
                HttpContext.ClearCustomMemory("test_custom");
                
                return Json(new { success = true, message = "Test cookies cleared" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing test cookies");
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get all cookie information for debugging
        /// </summary>
        [HttpGet]
        public IActionResult GetCookieInfo()
        {
            try
            {
                var cookies = Request.Cookies
                    .Where(c => c.Key.StartsWith("AppMemory_"))
                    .ToDictionary(c => c.Key, c => c.Value);
                
                return Json(new { success = true, cookies = cookies, count = cookies.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cookie info");
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}

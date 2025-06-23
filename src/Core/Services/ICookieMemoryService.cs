using Microsoft.AspNetCore.Http;

namespace Core.Services
{
    /// <summary>
    /// Interface for managing cookie-based memory storage
    /// Provides secure storage and retrieval of user preferences and data in cookies
    /// </summary>
    public interface ICookieMemoryService
    {
        /// <summary>
        /// Store a simple key-value pair in cookies
        /// </summary>
        /// <param name="key">The key to store</param>
        /// <param name="value">The value to store</param>
        /// <param name="expireDays">Number of days until cookie expires (default: 30)</param>
        /// <param name="encrypt">Whether to encrypt the value (default: true)</param>
        void Set(string key, string value, int expireDays = 30, bool encrypt = true);

        /// <summary>
        /// Store an object as JSON in cookies
        /// </summary>
        /// <typeparam name="T">Type of object to store</typeparam>
        /// <param name="key">The key to store</param>
        /// <param name="value">The object to store</param>
        /// <param name="expireDays">Number of days until cookie expires (default: 30)</param>
        /// <param name="encrypt">Whether to encrypt the value (default: true)</param>
        void Set<T>(string key, T value, int expireDays = 30, bool encrypt = true) where T : class;

        /// <summary>
        /// Retrieve a string value from cookies
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <param name="decrypt">Whether to decrypt the value (default: true)</param>
        /// <returns>The stored value or null if not found</returns>
        string? Get(string key, bool decrypt = true);

        /// <summary>
        /// Retrieve an object from cookies
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve</typeparam>
        /// <param name="key">The key to retrieve</param>
        /// <param name="decrypt">Whether to decrypt the value (default: true)</param>
        /// <returns>The stored object or null if not found</returns>
        T? Get<T>(string key, bool decrypt = true) where T : class;

        /// <summary>
        /// Check if a cookie exists
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if cookie exists, false otherwise</returns>
        bool Exists(string key);

        /// <summary>
        /// Remove a cookie
        /// </summary>
        /// <param name="key">The key to remove</param>
        void Remove(string key);

        /// <summary>
        /// Clear all cookies managed by this service
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Store user preferences
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="preferences">User preferences object</param>
        /// <param name="expireDays">Number of days until cookie expires (default: 365)</param>
        void SetUserPreferences(int userId, object preferences, int expireDays = 365);

        /// <summary>
        /// Retrieve user preferences
        /// </summary>
        /// <typeparam name="T">Type of preferences object</typeparam>
        /// <param name="userId">User ID</param>
        /// <returns>User preferences or null if not found</returns>
        T? GetUserPreferences<T>(int userId) where T : class;

        /// <summary>
        /// Store login memory data (username, remember me settings, etc.)
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="rememberMe">Whether to remember login</param>
        /// <param name="additionalData">Additional login-related data</param>
        /// <param name="expireDays">Number of days until cookie expires (default: 30)</param>
        void SetLoginMemory(string username, bool rememberMe, Dictionary<string, string>? additionalData = null, int expireDays = 30);

        /// <summary>
        /// Retrieve login memory data
        /// </summary>
        /// <returns>Login memory data or null if not found</returns>
        LoginMemoryData? GetLoginMemory();

        /// <summary>
        /// Clear login memory
        /// </summary>
        void ClearLoginMemory();

        /// <summary>
        /// Store form data temporarily (for form persistence across page reloads)
        /// </summary>
        /// <param name="formId">Unique form identifier</param>
        /// <param name="formData">Form data to store</param>
        /// <param name="expireMinutes">Number of minutes until cookie expires (default: 60)</param>
        void SetFormMemory(string formId, Dictionary<string, string> formData, int expireMinutes = 60);

        /// <summary>
        /// Retrieve form data
        /// </summary>
        /// <param name="formId">Unique form identifier</param>
        /// <returns>Form data or null if not found</returns>
        Dictionary<string, string>? GetFormMemory(string formId);

        /// <summary>
        /// Clear form memory
        /// </summary>
        /// <param name="formId">Unique form identifier</param>
        void ClearFormMemory(string formId);
    }

    /// <summary>
    /// Data model for login memory
    /// </summary>
    public class LoginMemoryData
    {
        public string Username { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration options for cookie memory service
    /// </summary>
    public class CookieMemoryOptions
    {
        /// <summary>
        /// Prefix for all cookie names managed by this service
        /// </summary>
        public string CookiePrefix { get; set; } = "AppMemory_";

        /// <summary>
        /// Default encryption key for cookie values
        /// </summary>
        public string EncryptionKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether cookies should be HTTP only by default
        /// </summary>
        public bool HttpOnly { get; set; } = true;

        /// <summary>
        /// Whether cookies should be secure by default
        /// </summary>
        public bool Secure { get; set; } = true;

        /// <summary>
        /// Default SameSite policy for cookies
        /// </summary>
        public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;

        /// <summary>
        /// Default domain for cookies
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// Default path for cookies
        /// </summary>
        public string Path { get; set; } = "/";
    }
}

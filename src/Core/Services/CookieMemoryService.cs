using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Core.Services
{
    /// <summary>
    /// Implementation of cookie-based memory storage service
    /// </summary>
    public class CookieMemoryService : ICookieMemoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CookieMemoryService> _logger;
        private readonly CookieMemoryOptions _options;

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;
        private IRequestCookieCollection? RequestCookies => HttpContext?.Request.Cookies;
        private IResponseCookies? ResponseCookies => HttpContext?.Response.Cookies;

        public CookieMemoryService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CookieMemoryService> logger,
            IOptions<CookieMemoryOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _options = options.Value;
        }

        public void Set(string key, string value, int expireDays = 30, bool encrypt = true)
        {
            try
            {
                if (ResponseCookies == null)
                {
                    _logger.LogWarning("Cannot set cookie - HttpContext or Response not available");
                    return;
                }

                var cookieName = GetCookieName(key);
                var cookieValue = encrypt ? EncryptValue(value) : value;

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(expireDays),
                    HttpOnly = _options.HttpOnly,
                    Secure = _options.Secure,
                    SameSite = _options.SameSite,
                    Domain = _options.Domain,
                    Path = _options.Path
                };

                ResponseCookies.Append(cookieName, cookieValue, cookieOptions);
                _logger.LogDebug("Cookie set: {CookieName}", cookieName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cookie: {Key}", key);
            }
        }

        public void Set<T>(string key, T value, int expireDays = 30, bool encrypt = true) where T : class
        {
            try
            {
                var json = JsonConvert.SerializeObject(value);
                Set(key, json, expireDays, encrypt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting object cookie: {Key}", key);
            }
        }

        public string? Get(string key, bool decrypt = true)
        {
            try
            {
                if (RequestCookies == null)
                {
                    _logger.LogWarning("Cannot get cookie - HttpContext or Request not available");
                    return null;
                }

                var cookieName = GetCookieName(key);
                var cookieValue = RequestCookies[cookieName];

                if (string.IsNullOrEmpty(cookieValue))
                    return null;

                return decrypt ? DecryptValue(cookieValue) : cookieValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cookie: {Key}", key);
                return null;
            }
        }

        public T? Get<T>(string key, bool decrypt = true) where T : class
        {
            try
            {
                var json = Get(key, decrypt);
                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting object cookie: {Key}", key);
                return null;
            }
        }

        public bool Exists(string key)
        {
            try
            {
                if (RequestCookies == null)
                    return false;

                var cookieName = GetCookieName(key);
                return RequestCookies.ContainsKey(cookieName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cookie existence: {Key}", key);
                return false;
            }
        }

        public void Remove(string key)
        {
            try
            {
                if (ResponseCookies == null)
                {
                    _logger.LogWarning("Cannot remove cookie - HttpContext or Response not available");
                    return;
                }

                var cookieName = GetCookieName(key);
                ResponseCookies.Delete(cookieName);
                _logger.LogDebug("Cookie removed: {CookieName}", cookieName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cookie: {Key}", key);
            }
        }

        public void ClearAll()
        {
            try
            {
                if (RequestCookies == null || ResponseCookies == null)
                {
                    _logger.LogWarning("Cannot clear cookies - HttpContext not available");
                    return;
                }

                var cookiesToRemove = RequestCookies.Keys
                    .Where(k => k.StartsWith(_options.CookiePrefix))
                    .ToList();

                foreach (var cookieName in cookiesToRemove)
                {
                    ResponseCookies.Delete(cookieName);
                }

                _logger.LogDebug("Cleared {Count} cookies", cookiesToRemove.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all cookies");
            }
        }

        public void SetUserPreferences(int userId, object preferences, int expireDays = 365)
        {
            var key = $"UserPrefs_{userId}";
            Set(key, preferences, expireDays);
        }

        public T? GetUserPreferences<T>(int userId) where T : class
        {
            var key = $"UserPrefs_{userId}";
            return Get<T>(key);
        }

        public void SetLoginMemory(string username, bool rememberMe, Dictionary<string, string>? additionalData = null, int expireDays = 30)
        {
            var loginData = new LoginMemoryData
            {
                Username = username,
                RememberMe = rememberMe,
                AdditionalData = additionalData ?? new Dictionary<string, string>(),
                CreatedAt = DateTime.UtcNow
            };

            Set("LoginMemory", loginData, expireDays);
        }

        public LoginMemoryData? GetLoginMemory()
        {
            return Get<LoginMemoryData>("LoginMemory");
        }

        public void ClearLoginMemory()
        {
            Remove("LoginMemory");
        }

        public void SetFormMemory(string formId, Dictionary<string, string> formData, int expireMinutes = 60)
        {
            var key = $"Form_{formId}";
            var expireDays = expireMinutes / (24.0 * 60.0); // Convert minutes to days
            Set(key, formData, (int)Math.Ceiling(expireDays));
        }

        public Dictionary<string, string>? GetFormMemory(string formId)
        {
            var key = $"Form_{formId}";
            return Get<Dictionary<string, string>>(key);
        }

        public void ClearFormMemory(string formId)
        {
            var key = $"Form_{formId}";
            Remove(key);
        }

        private string GetCookieName(string key)
        {
            return $"{_options.CookiePrefix}{key}";
        }

        private string EncryptValue(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(_options.EncryptionKey))
                {
                    _logger.LogWarning("No encryption key configured, storing value as plain text");
                    return value;
                }

                // Use Base64 encoding for simple obfuscation
                // In production, you might want to use proper AES encryption
                var bytes = Encoding.UTF8.GetBytes(value);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting cookie value");
                return value; // Fallback to plain text
            }
        }

        private string DecryptValue(string encryptedValue)
        {
            try
            {
                if (string.IsNullOrEmpty(_options.EncryptionKey))
                {
                    return encryptedValue; // Assume it's plain text
                }

                // Decode from Base64
                var bytes = Convert.FromBase64String(encryptedValue);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting cookie value, returning as-is");
                return encryptedValue; // Fallback to encrypted value
            }
        }
    }
}

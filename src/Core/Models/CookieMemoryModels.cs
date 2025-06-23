using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    /// <summary>
    /// Base class for all cookie memory data
    /// </summary>
    public abstract class CookieMemoryBase
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public string Version { get; set; } = "1.0";
    }

    /// <summary>
    /// User preferences that can be stored in cookies
    /// </summary>
    public class UserPreferences : CookieMemoryBase
    {
        /// <summary>
        /// User's preferred language/locale
        /// </summary>
        public string Language { get; set; } = "vi-VN";

        /// <summary>
        /// User's preferred theme (light, dark, auto)
        /// </summary>
        public string Theme { get; set; } = "light";

        /// <summary>
        /// User's timezone
        /// </summary>
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";

        /// <summary>
        /// Number of items to display per page
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// User's preferred date format
        /// </summary>
        public string DateFormat { get; set; } = "dd/MM/yyyy";

        /// <summary>
        /// User's preferred time format
        /// </summary>
        public string TimeFormat { get; set; } = "HH:mm";

        /// <summary>
        /// Whether to show notifications
        /// </summary>
        public bool ShowNotifications { get; set; } = true;

        /// <summary>
        /// Whether to auto-save forms
        /// </summary>
        public bool AutoSaveForms { get; set; } = true;

        /// <summary>
        /// Custom user settings as key-value pairs
        /// </summary>
        public Dictionary<string, string> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Session-specific data that can be stored in cookies
    /// </summary>
    public class SessionData : CookieMemoryBase
    {
        /// <summary>
        /// Current page or section user is working on
        /// </summary>
        public string CurrentSection { get; set; } = string.Empty;

        /// <summary>
        /// Last visited URLs for navigation history
        /// </summary>
        public List<string> RecentUrls { get; set; } = new();

        /// <summary>
        /// Search history
        /// </summary>
        public List<string> SearchHistory { get; set; } = new();

        /// <summary>
        /// Temporary data for the current session
        /// </summary>
        public Dictionary<string, string> TempData { get; set; } = new();

        /// <summary>
        /// User's current workflow state
        /// </summary>
        public string WorkflowState { get; set; } = string.Empty;
    }

    /// <summary>
    /// Form data that can be temporarily stored in cookies
    /// </summary>
    public class FormMemoryData : CookieMemoryBase
    {
        /// <summary>
        /// Form identifier
        /// </summary>
        public string FormId { get; set; } = string.Empty;

        /// <summary>
        /// Form field values
        /// </summary>
        public Dictionary<string, string> FieldValues { get; set; } = new();

        /// <summary>
        /// Form validation state
        /// </summary>
        public Dictionary<string, bool> FieldValidation { get; set; } = new();

        /// <summary>
        /// Current step in multi-step forms
        /// </summary>
        public int CurrentStep { get; set; } = 1;

        /// <summary>
        /// Total steps in multi-step forms
        /// </summary>
        public int TotalSteps { get; set; } = 1;
    }

    /// <summary>
    /// Shopping cart or temporary selections
    /// </summary>
    public class SelectionMemoryData : CookieMemoryBase
    {
        /// <summary>
        /// Selected item IDs
        /// </summary>
        public List<int> SelectedIds { get; set; } = new();

        /// <summary>
        /// Selection context (e.g., "users", "products", "orders")
        /// </summary>
        public string Context { get; set; } = string.Empty;

        /// <summary>
        /// Additional selection metadata
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Navigation and UI state
    /// </summary>
    public class NavigationMemoryData : CookieMemoryBase
    {
        /// <summary>
        /// Collapsed/expanded state of sidebar
        /// </summary>
        public bool SidebarCollapsed { get; set; } = false;

        /// <summary>
        /// Active menu items
        /// </summary>
        public List<string> ActiveMenus { get; set; } = new();

        /// <summary>
        /// Pinned or favorite pages
        /// </summary>
        public List<string> PinnedPages { get; set; } = new();

        /// <summary>
        /// Dashboard widget preferences
        /// </summary>
        public Dictionary<string, object> WidgetSettings { get; set; } = new();

        /// <summary>
        /// Table column visibility and order
        /// </summary>
        public Dictionary<string, List<string>> TableSettings { get; set; } = new();
    }

    /// <summary>
    /// Security and privacy related memory
    /// </summary>
    public class SecurityMemoryData : CookieMemoryBase
    {
        /// <summary>
        /// Last login timestamp
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Login attempt count
        /// </summary>
        public int LoginAttempts { get; set; } = 0;

        /// <summary>
        /// Security questions answered
        /// </summary>
        public List<string> AnsweredSecurityQuestions { get; set; } = new();

        /// <summary>
        /// Two-factor authentication preferences
        /// </summary>
        public bool TwoFactorEnabled { get; set; } = false;

        /// <summary>
        /// Privacy consent given
        /// </summary>
        public bool PrivacyConsentGiven { get; set; } = false;

        /// <summary>
        /// Cookie consent preferences
        /// </summary>
        public Dictionary<string, bool> CookieConsent { get; set; } = new();
    }

    /// <summary>
    /// Generic key-value storage for custom data
    /// </summary>
    public class CustomMemoryData : CookieMemoryBase
    {
        /// <summary>
        /// Data category or type
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Custom data as key-value pairs
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = new();

        /// <summary>
        /// Tags for categorization and searching
        /// </summary>
        public List<string> Tags { get; set; } = new();
    }
}

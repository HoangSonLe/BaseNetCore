# Cookie Memory System - Usage Guide

## Overview

The Cookie Memory System provides a comprehensive solution for storing user preferences, session data, form data, and other temporary information in cookies. It includes server-side services and client-side JavaScript helpers.

## Features

- **User Preferences**: Store user settings like theme, language, page size, etc.
- **Login Memory**: Remember login information and user choices
- **Form Auto-Save**: Automatically save form data to prevent data loss
- **Search Memory**: Remember recent searches for better UX
- **Session Data**: Store temporary session information
- **Navigation Memory**: Remember UI state like sidebar collapse, menu selections
- **Security Memory**: Track security-related information
- **Custom Memory**: Store any custom key-value data

## Server-Side Usage

### 1. Basic Cookie Operations

```csharp
// Inject the service in your controller
public class MyController : Controller
{
    private readonly ICookieMemoryService _cookieMemory;
    
    public MyController(ICookieMemoryService cookieMemory)
    {
        _cookieMemory = cookieMemory;
    }
    
    public IActionResult Example()
    {
        // Store simple string value
        _cookieMemory.Set("user_theme", "dark", expireDays: 365);
        
        // Store object as JSON
        var preferences = new UserPreferences { Theme = "dark", Language = "en" };
        _cookieMemory.Set("user_prefs", preferences, expireDays: 365);
        
        // Retrieve values
        var theme = _cookieMemory.Get("user_theme");
        var userPrefs = _cookieMemory.Get<UserPreferences>("user_prefs");
        
        // Check if exists
        if (_cookieMemory.Exists("user_theme"))
        {
            // Do something
        }
        
        // Remove cookie
        _cookieMemory.Remove("user_theme");
        
        return View();
    }
}
```

### 2. Using Extension Methods

```csharp
public IActionResult Example()
{
    // Store user preferences
    var preferences = new UserPreferences
    {
        Theme = "dark",
        Language = "vi-VN",
        PageSize = 50
    };
    HttpContext.SetUserPreferences(userId: 123, preferences);
    
    // Get user preferences
    var userPrefs = HttpContext.GetUserPreferences(userId: 123);
    
    // Remember search
    HttpContext.RememberSearch("search query");
    
    // Get recent searches
    var recentSearches = HttpContext.GetRecentSearches(count: 10);
    
    // Auto-save form
    var formData = new Dictionary<string, string>
    {
        { "name", "John Doe" },
        { "email", "john@example.com" }
    };
    HttpContext.AutoSaveForm("contact-form", formData);
    
    // Get auto-saved form
    var savedForm = HttpContext.GetAutoSavedForm("contact-form");
    
    return View();
}
```

### 3. Login Memory Integration

The AccountController automatically uses cookie memory for login functionality:

```csharp
// When user logs in with RememberMe = true
// The system automatically stores:
// - Username
// - RememberMe preference
// - Additional login data (AccountType, IsMobile, LastLoginTime)
// - User preferences

// When user logs out
// The system clears session data but keeps user preferences and login memory if RememberMe was enabled
```

## Client-Side Usage

### 1. Include the JavaScript Helper

```html
<script src="~/js/cookie-memory.js"></script>
```

### 2. Basic JavaScript Operations

```javascript
// Get user preferences
const preferences = await window.cookieMemory.getUserPreferences();
console.log(preferences);

// Update user preferences
const newPreferences = {
    theme: 'dark',
    language: 'vi-VN',
    pageSize: 25
};
const success = await window.cookieMemory.updateUserPreferences(newPreferences);

// Remember search
await window.cookieMemory.rememberSearch('my search query');

// Get recent searches
const recentSearches = await window.cookieMemory.getRecentSearches(5);

// Auto-save form
const formData = {
    'name': 'John Doe',
    'email': 'john@example.com'
};
await window.cookieMemory.autoSaveForm('my-form', formData);

// Get auto-saved form
const savedData = await window.cookieMemory.getAutoSavedForm('my-form');
```

### 3. Auto-Initialize Features

```html
<!-- Auto-save form -->
<form id="contact-form" data-auto-save data-form-id="contact-form">
    <input type="text" name="name" placeholder="Name">
    <input type="email" name="email" placeholder="Email">
    <textarea name="message" placeholder="Message"></textarea>
    <button type="submit">Submit</button>
</form>

<!-- Search with memory -->
<input type="search" id="search-input" placeholder="Search...">
<div id="search-suggestions" class="search-suggestions"></div>
```

### 4. Manual Initialization

```javascript
// Initialize auto-save for a specific form
window.cookieMemory.initAutoSave('#my-form', {
    interval: 30000, // Save every 30 seconds
    formId: 'my-custom-form-id',
    excludeFields: ['password', 'confirmPassword']
});

// Initialize search memory
window.cookieMemory.initSearchMemory('#search-input', '#search-suggestions');

// Apply user preferences to the page
await window.cookieMemory.applyUserPreferences();
```

## Configuration

### Server-Side Configuration (Program.cs)

```csharp
builder.Services.Configure<CookieMemoryOptions>(options =>
{
    options.CookiePrefix = "MyApp_";
    options.EncryptionKey = "your-secret-key";
    options.HttpOnly = true;
    options.Secure = true; // Set to true in production
    options.SameSite = SameSiteMode.Lax;
    options.Domain = ".yourdomain.com"; // Optional
});
```

### Configuration in appsettings.json

```json
{
  "CookieMemory": {
    "EncryptionKey": "YourSecretEncryptionKey123!@#"
  }
}
```

## Data Models

### UserPreferences
- Language
- Theme (light, dark, auto)
- TimeZone
- PageSize
- DateFormat
- TimeFormat
- ShowNotifications
- AutoSaveForms
- CustomSettings (Dictionary)

### SessionData
- CurrentSection
- RecentUrls
- SearchHistory
- TempData
- WorkflowState

### NavigationMemoryData
- SidebarCollapsed
- ActiveMenus
- PinnedPages
- WidgetSettings
- TableSettings

### SecurityMemoryData
- LastLogin
- LoginAttempts
- AnsweredSecurityQuestions
- TwoFactorEnabled
- PrivacyConsentGiven
- CookieConsent

## Best Practices

1. **Security**: Always encrypt sensitive data and use HTTPS in production
2. **Performance**: Don't store large amounts of data in cookies (4KB limit per cookie)
3. **Privacy**: Respect user privacy and provide clear cookie consent
4. **Expiration**: Set appropriate expiration times for different types of data
5. **Fallbacks**: Always provide fallback values when retrieving cookie data
6. **Cleanup**: Regularly clean up expired or unused cookies

## Examples

### Theme Switching
```javascript
// Change theme and save preference
async function changeTheme(theme) {
    document.body.setAttribute('data-theme', theme);
    
    const preferences = await window.cookieMemory.getUserPreferences() || {};
    preferences.theme = theme;
    await window.cookieMemory.updateUserPreferences(preferences);
}
```

### Form Auto-Save with Visual Feedback
```javascript
// Show save status
async function autoSaveWithFeedback(formId, formData) {
    const statusElement = document.getElementById('save-status');
    statusElement.textContent = 'Saving...';
    
    const success = await window.cookieMemory.autoSaveForm(formId, formData);
    
    if (success) {
        statusElement.textContent = 'Saved';
        setTimeout(() => statusElement.textContent = '', 2000);
    } else {
        statusElement.textContent = 'Save failed';
    }
}
```

### Search with Autocomplete
```html
<div class="search-container">
    <input type="search" id="search-input" placeholder="Search...">
    <div id="search-suggestions" class="search-suggestions"></div>
</div>

<style>
.search-suggestions {
    display: none;
    position: absolute;
    background: white;
    border: 1px solid #ccc;
    max-height: 200px;
    overflow-y: auto;
}

.search-suggestion-item {
    padding: 8px 12px;
    cursor: pointer;
}

.search-suggestion-item:hover {
    background-color: #f0f0f0;
}
</style>
```

This cookie memory system provides a robust foundation for storing user data and preferences while maintaining security and performance.

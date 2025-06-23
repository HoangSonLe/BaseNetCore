/**
 * Cookie Memory JavaScript Helper
 * Provides client-side functionality for interacting with the server-side cookie memory system
 */

class CookieMemoryHelper {
    constructor() {
        this.baseUrl = '/Account';
    }

    /**
     * Get user preferences from server
     */
    async getUserPreferences() {
        try {
            const response = await fetch(`${this.baseUrl}/GetUserPreferences`);
            if (response.ok) {
                return await response.json();
            }
            return null;
        } catch (error) {
            console.error('Error getting user preferences:', error);
            return null;
        }
    }

    /**
     * Update user preferences on server
     */
    async updateUserPreferences(preferences) {
        try {
            const response = await fetch(`${this.baseUrl}/UpdateUserPreferences`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(preferences)
            });
            
            if (response.ok) {
                const result = await response.json();
                return result.success;
            }
            return false;
        } catch (error) {
            console.error('Error updating user preferences:', error);
            return false;
        }
    }

    /**
     * Remember a search query
     */
    async rememberSearch(searchQuery) {
        if (!searchQuery || searchQuery.trim() === '') return false;

        try {
            const response = await fetch(`${this.baseUrl}/RememberSearch`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(searchQuery.trim())
            });
            
            if (response.ok) {
                const result = await response.json();
                return result.success;
            }
            return false;
        } catch (error) {
            console.error('Error remembering search:', error);
            return false;
        }
    }

    /**
     * Get recent searches
     */
    async getRecentSearches(count = 10) {
        try {
            const response = await fetch(`${this.baseUrl}/GetRecentSearches?count=${count}`);
            if (response.ok) {
                return await response.json();
            }
            return [];
        } catch (error) {
            console.error('Error getting recent searches:', error);
            return [];
        }
    }

    /**
     * Auto-save form data
     */
    async autoSaveForm(formId, formData) {
        try {
            const response = await fetch(`${this.baseUrl}/AutoSaveForm`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    formId: formId,
                    formData: formData
                })
            });
            
            if (response.ok) {
                const result = await response.json();
                return result.success;
            }
            return false;
        } catch (error) {
            console.error('Error auto-saving form:', error);
            return false;
        }
    }

    /**
     * Get auto-saved form data
     */
    async getAutoSavedForm(formId) {
        try {
            const response = await fetch(`${this.baseUrl}/GetAutoSavedForm?formId=${encodeURIComponent(formId)}`);
            if (response.ok) {
                return await response.json();
            }
            return {};
        } catch (error) {
            console.error('Error getting auto-saved form:', error);
            return {};
        }
    }

    /**
     * Get anti-forgery token from the page
     */
    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    /**
     * Initialize auto-save for a form
     */
    initAutoSave(formSelector, options = {}) {
        const form = document.querySelector(formSelector);
        if (!form) return;

        const settings = {
            interval: 30000, // 30 seconds
            formId: form.id || 'default-form',
            excludeFields: ['password', 'confirmPassword'],
            ...options
        };

        // Auto-save interval
        setInterval(() => {
            this.saveFormData(form, settings);
        }, settings.interval);

        // Save on form change
        form.addEventListener('change', () => {
            this.saveFormData(form, settings);
        });

        // Load saved data on page load
        this.loadFormData(form, settings);
    }

    /**
     * Save form data
     */
    async saveFormData(form, settings) {
        const formData = {};
        const formElements = form.querySelectorAll('input, select, textarea');
        
        formElements.forEach(element => {
            if (element.name && 
                !settings.excludeFields.includes(element.name.toLowerCase()) &&
                !settings.excludeFields.includes(element.type)) {
                
                if (element.type === 'checkbox' || element.type === 'radio') {
                    formData[element.name] = element.checked.toString();
                } else {
                    formData[element.name] = element.value;
                }
            }
        });

        await this.autoSaveForm(settings.formId, formData);
    }

    /**
     * Load form data
     */
    async loadFormData(form, settings) {
        const savedData = await this.getAutoSavedForm(settings.formId);
        
        Object.keys(savedData).forEach(fieldName => {
            const element = form.querySelector(`[name="${fieldName}"]`);
            if (element) {
                if (element.type === 'checkbox' || element.type === 'radio') {
                    element.checked = savedData[fieldName] === 'true';
                } else {
                    element.value = savedData[fieldName];
                }
            }
        });
    }

    /**
     * Initialize search functionality with memory
     */
    initSearchMemory(searchInputSelector, suggestionsContainerSelector) {
        const searchInput = document.querySelector(searchInputSelector);
        const suggestionsContainer = document.querySelector(suggestionsContainerSelector);
        
        if (!searchInput || !suggestionsContainer) return;

        // Show suggestions on focus
        searchInput.addEventListener('focus', async () => {
            const recentSearches = await this.getRecentSearches(5);
            this.showSearchSuggestions(suggestionsContainer, recentSearches);
        });

        // Hide suggestions on blur (with delay to allow clicking)
        searchInput.addEventListener('blur', () => {
            setTimeout(() => {
                suggestionsContainer.style.display = 'none';
            }, 200);
        });

        // Remember search on form submit
        const form = searchInput.closest('form');
        if (form) {
            form.addEventListener('submit', () => {
                this.rememberSearch(searchInput.value);
            });
        }
    }

    /**
     * Show search suggestions
     */
    showSearchSuggestions(container, suggestions) {
        container.innerHTML = '';
        
        if (suggestions.length === 0) {
            container.style.display = 'none';
            return;
        }

        suggestions.forEach(suggestion => {
            const item = document.createElement('div');
            item.className = 'search-suggestion-item';
            item.textContent = suggestion;
            item.addEventListener('click', () => {
                const searchInput = container.previousElementSibling;
                if (searchInput) {
                    searchInput.value = suggestion;
                    searchInput.form.submit();
                }
            });
            container.appendChild(item);
        });

        container.style.display = 'block';
    }

    /**
     * Apply user preferences to the page
     */
    async applyUserPreferences() {
        const preferences = await this.getUserPreferences();
        if (!preferences) return;

        // Apply theme
        if (preferences.theme) {
            document.body.setAttribute('data-theme', preferences.theme);
        }

        // Apply language
        if (preferences.language) {
            document.documentElement.lang = preferences.language;
        }

        // Apply page size for tables
        if (preferences.pageSize) {
            const pageSizeSelects = document.querySelectorAll('.page-size-select');
            pageSizeSelects.forEach(select => {
                select.value = preferences.pageSize;
            });
        }
    }
}

// Initialize global instance
window.cookieMemory = new CookieMemoryHelper();

// Auto-initialize on DOM ready
document.addEventListener('DOMContentLoaded', () => {
    // Apply user preferences
    window.cookieMemory.applyUserPreferences();
    
    // Initialize search memory if search input exists
    const searchInput = document.querySelector('#search-input, .search-input, input[type="search"]');
    if (searchInput) {
        const suggestionsContainer = document.querySelector('#search-suggestions, .search-suggestions');
        if (suggestionsContainer) {
            window.cookieMemory.initSearchMemory('#' + searchInput.id, '#' + suggestionsContainer.id);
        }
    }
    
    // Initialize auto-save for forms with data-auto-save attribute
    const autoSaveForms = document.querySelectorAll('form[data-auto-save]');
    autoSaveForms.forEach(form => {
        const formId = form.getAttribute('data-form-id') || form.id || 'auto-save-form';
        window.cookieMemory.initAutoSave('#' + form.id, { formId: formId });
    });
});

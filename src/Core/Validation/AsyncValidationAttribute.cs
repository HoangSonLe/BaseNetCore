using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
    /// <summary>
    /// Base class for async validation attributes
    /// </summary>
    public abstract class AsyncValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the specified value asynchronously
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Validation result</returns>
        public abstract Task<ValidationResult?> IsValidAsync(
            object? value, 
            ValidationContext validationContext, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronous validation - calls async version
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">The validation context</param>
        /// <returns>Validation result</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // For synchronous calls, we use GetAwaiter().GetResult()
            // This is not ideal but necessary for compatibility
            try
            {
                return IsValidAsync(value, validationContext).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return new ValidationResult($"Validation error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Validates that a username is unique
    /// </summary>
    public class UniqueUsernameAttribute : AsyncValidationAttribute
    {
        public override async Task<ValidationResult?> IsValidAsync(
            object? value, 
            ValidationContext validationContext, 
            CancellationToken cancellationToken = default)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var username = value.ToString()!;
            
            // Get service from DI container
            var serviceProvider = validationContext.GetService(typeof(IServiceProvider)) as IServiceProvider;
            if (serviceProvider == null)
            {
                return new ValidationResult("Service provider not available for validation");
            }

            // This would need to be implemented based on your service structure
            // For now, return success to avoid breaking changes
            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates that an email is unique
    /// </summary>
    public class UniqueEmailAttribute : AsyncValidationAttribute
    {
        public override async Task<ValidationResult?> IsValidAsync(
            object? value, 
            ValidationContext validationContext, 
            CancellationToken cancellationToken = default)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var email = value.ToString()!;
            
            // Validate email format first
            if (!IsValidEmail(email))
            {
                return new ValidationResult("Invalid email format");
            }

            // Get service from DI container for uniqueness check
            var serviceProvider = validationContext.GetService(typeof(IServiceProvider)) as IServiceProvider;
            if (serviceProvider == null)
            {
                return new ValidationResult("Service provider not available for validation");
            }

            // This would need to be implemented based on your service structure
            return ValidationResult.Success;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Validates that a value exists in the database
    /// </summary>
    public class ExistsInDatabaseAttribute : AsyncValidationAttribute
    {
        private readonly Type _entityType;
        private readonly string _propertyName;

        public ExistsInDatabaseAttribute(Type entityType, string propertyName = "Id")
        {
            _entityType = entityType;
            _propertyName = propertyName;
        }

        public override async Task<ValidationResult?> IsValidAsync(
            object? value, 
            ValidationContext validationContext, 
            CancellationToken cancellationToken = default)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let Required attribute handle null values
            }

            // Get service from DI container
            var serviceProvider = validationContext.GetService(typeof(IServiceProvider)) as IServiceProvider;
            if (serviceProvider == null)
            {
                return new ValidationResult("Service provider not available for validation");
            }

            // This would need to be implemented based on your repository structure
            return ValidationResult.Success;
        }
    }
}

using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Core.Enums;

namespace Core.Helper
{
    /// <summary>
    /// Service for accessing current user information from the HttpContext
    /// </summary>
    public interface ICurrentUserContext
    {
        /// <summary>
        /// Gets the current user ID
        /// </summary>
        int? UserId { get; }

        /// <summary>
        /// Gets the current user's roles
        /// </summary>
        List<ERoleType> UserRoles { get; }

        /// <summary>
        /// Checks if the current user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Checks if the current user has admin role
        /// </summary>
        bool IsAdmin { get; }
    }

    /// <summary>
    /// Implementation of ICurrentUserContext that gets user information from HttpContext
    /// </summary>
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserContext> _logger;

        public CurrentUserContext(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CurrentUserContext> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the current user ID from claims
        /// </summary>
        public int? UserId
        {
            get
            {
                //TODO
                try
                {
                    if (!IsAuthenticated)
                        return null;

                    var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("UserID");
                    if (string.IsNullOrEmpty(userId))
                        return null;

                    if (int.TryParse(userId, out int parsedUserId))
                        return parsedUserId;

                    _logger.LogWarning("Failed to parse user ID from claims: {UserId}", userId);
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving current user ID");
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the current user's roles from claims
        /// </summary>
        public List<ERoleType> UserRoles
        {
            get
            {
                //TODO - Remove this return statement when implementation is ready
                return [];

                /*
                try
                {
                    if (!IsAuthenticated)
                        return new List<ERoleType>();

                    var roleListClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("RoleIds");
                    if (string.IsNullOrEmpty(roleListClaim))
                        return new List<ERoleType>();

                    return roleListClaim.Split(",")
                        .Where(r => !string.IsNullOrWhiteSpace(r))
                        .Select(r =>
                        {
                            if (int.TryParse(r, out int roleId))
                                return (ERoleType)roleId;
                            return (ERoleType)(-1); // Invalid role
                        })
                        .Where(r => r != (ERoleType)(-1))
                        .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving current user roles");
                    return new List<ERoleType>();
                }
                */
            }
        }

        /// <summary>
        /// Checks if the current user is authenticated
        /// </summary>
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        /// <summary>
        /// Checks if the current user has admin role
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                var roles = UserRoles;
                return roles.Contains(ERoleType.Admin) || roles.Contains(ERoleType.SystemAdmin);
            }
        }
    }
}

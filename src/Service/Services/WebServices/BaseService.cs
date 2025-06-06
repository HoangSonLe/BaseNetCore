

using Application.Interfaces;
using Core.Enums;
using Infrastructure.DBContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace Application.Services.WebServices
{
    public abstract class BaseService<T> : IDisposable
    {
        public readonly ILogger<T> _logger;
        public readonly IUserRepository _userRepository;
        public readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public IConfiguration Configuration => _configuration;
        private readonly int _currentUserId;
        private readonly List<ERoleType> _currentUserRoleId;

        public BaseService(
            ILogger<T> logger,
            IConfiguration configuration,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            // Khởi tạo user context một lần duy nhất
            (_currentUserId, _currentUserRoleId) = InitializeUserContext();
        }

        /// <summary>
        /// Khởi tạo thông tin user hiện tại một cách tối ưu và an toàn
        /// </summary>
        private (int userId, List<ERoleType> roles) InitializeUserContext()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity == null)
                {
                    return (0, new List<ERoleType>());
                }

                var user = httpContext.User;
                var isAuthenticated = user.Identity.IsAuthenticated;

                if (!isAuthenticated)
                {
                    return (0, new List<ERoleType>());
                }

                // Lấy UserID
                var userIdClaim = user.FindFirst("UserID")?.Value;
                int userId = 0;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("Authenticated user missing UserID claim");
                }
                else if (!int.TryParse(userIdClaim, out userId))
                {
                    _logger.LogWarning("Invalid UserID claim format: {UserIdClaim}", userIdClaim);
                    userId = 0;
                }

                // Lấy RoleIds với error handling tốt hơn
                var roleListClaim = user.FindFirst("RoleIds")?.Value;
                var roles = ParseUserRoles(roleListClaim);

                return (userId, roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing current user context");
                return (0, new List<ERoleType>());
            }
        }

        /// <summary>
        /// Parse user roles một cách an toàn
        /// </summary>
        private List<ERoleType> ParseUserRoles(string? roleListClaim)
        {
            if (string.IsNullOrEmpty(roleListClaim))
            {
                return new List<ERoleType>();
            }

            try
            {
                return roleListClaim
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(roleStr => roleStr.Trim())
                    .Where(roleStr => int.TryParse(roleStr, out _))
                    .Select(roleStr => (ERoleType)int.Parse(roleStr))
                    .Where(role => Enum.IsDefined(typeof(ERoleType), role))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing user roles from claim: {RoleListClaim}", roleListClaim);
                return new List<ERoleType>();
            }
        }
        private SampleDBContext? _DbContext;
        public SampleDBContext DbContext
        {
            get
            {
                if (_DbContext == null)
                {
                    _DbContext = new SampleDBContext();
                }
                return _DbContext;
            }
        }

        /// <summary>
        /// Current User ID - readonly property, không thể override
        /// </summary>
        public int CurrentUserId => _currentUserId;

        /// <summary>
        /// Kiểm tra user có authenticated không
        /// </summary>
        public bool IsAuthenticated => _currentUserId > 0;

        /// <summary>
        /// Current User Roles - readonly property, không thể override
        /// </summary>
        public IReadOnlyList<ERoleType> CurrentUserRoles => _currentUserRoleId.AsReadOnly();


        public void Dispose()
        {
            DbContext.Dispose();
        }
        ~BaseService()
        {
            Dispose();
        }
        #region COMMON FUNC AUTHOR
        /// <summary>
        /// Kiểm tra user có quyền Admin không - Đã tối ưu hóa
        /// </summary>
        /// <returns>True nếu user có quyền Admin hoặc SystemAdmin</returns>
        public bool _IsHasAdminRole()
        {
            return IsAuthenticated &&
                   (CurrentUserRoles.Contains(ERoleType.Admin) ||
                    CurrentUserRoles.Contains(ERoleType.SystemAdmin));
        }

        /// <summary>
        /// Kiểm tra user có role cụ thể không
        /// </summary>
        /// <param name="role">Role cần kiểm tra</param>
        /// <returns>True nếu user có role đó</returns>
        public bool HasRole(ERoleType role)
        {
            return IsAuthenticated && CurrentUserRoles.Contains(role);
        }

        /// <summary>
        /// Kiểm tra user có bất kỳ role nào trong danh sách không
        /// </summary>
        /// <param name="roles">Danh sách roles cần kiểm tra</param>
        /// <returns>True nếu user có ít nhất một role trong danh sách</returns>
        public bool HasAnyRole(params ERoleType[] roles)
        {
            return IsAuthenticated && roles.Any(role => CurrentUserRoles.Contains(role));
        }

        /// <summary>
        /// Kiểm tra user có tất cả roles trong danh sách không
        /// </summary>
        /// <param name="roles">Danh sách roles cần kiểm tra</param>
        /// <returns>True nếu user có tất cả roles trong danh sách</returns>
        public bool HasAllRoles(params ERoleType[] roles)
        {
            return IsAuthenticated && roles.All(role => CurrentUserRoles.Contains(role));
        }
        #endregion

    }
}

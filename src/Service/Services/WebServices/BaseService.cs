

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
        public int _currentUserId;
        public List<ERoleType> _currentUserRoleId;
        public int? _currentTenantId;

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
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("UserID")?.Value;
            var roleListClaim = _httpContextAccessor.HttpContext.User.FindFirst("RoleIds")?.Value;
            if (_httpContextAccessor.HttpContext.User.Identity != null)
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == true && userId == null)
                {
                    throw new ArgumentException("Claim must have UserID.");
                }
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == true && !int.TryParse(userId, out _currentUserId))
                {
                    throw new ArgumentException("Claim UserID is correct format.");
                }
                if (!string.IsNullOrEmpty(roleListClaim))
                {
                    _currentUserRoleId = roleListClaim.Split(",").Select(i => (ERoleType)int.Parse(i)).ToList();
                }
            }
            
        }
        private SampleDBContext _DbContext;
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
        /// Tạm - Làm chức năng phân quyền sau
        /// </summary>
        /// <returns></returns>
        public bool _IsHasAdminRole()
        {
            if (_currentUserRoleId == null)
            {
                throw new NullReferenceException("Không tìm thấy roleId in context");
            }
            if (_currentUserRoleId.Contains(ERoleType.Admin) || _currentUserRoleId.Contains(ERoleType.SystemAdmin))
            {
                return true;
            }
            return false;
        }
        #endregion

    }
}

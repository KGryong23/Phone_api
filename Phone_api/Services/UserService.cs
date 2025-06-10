using Microsoft.Extensions.Caching.Memory;
using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Entities;
using Phone_api.Enums;
using Phone_api.Exceptions;
using Phone_api.Repositories;

namespace Phone_api.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ cho người dùng và phân quyền.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly JwtTokenService _jwtTokenService;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Khởi tạo UserService với các repository và dịch vụ JWT.
        /// </summary>
        /// <param name="userRepository">Repository cho User.</param>
        /// <param name="roleRepository">Repository cho Role.</param>
        /// <param name="permissionRepository">Repository cho Permission.</param>
        /// <param name="userRoleRepository">Repository cho UserRole.</param>
        /// <param name="cache">Repository cho MemoryCache.</param>
        /// <param name="jwtTokenService">Dịch vụ tạo JWT token.</param>
        public UserService(
            IUserRepository userRepository, IRoleRepository roleRepository,
            IPermissionRepository permissionRepository, JwtTokenService jwtTokenService,
            IUserRoleRepository userRoleRepository, IMemoryCache cache
            )
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _cache = cache;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        /// <returns>Danh sách người dùng dưới dạng DTO.</returns>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.FindAllAsync(u => true, u => u.UserRoles!);
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                RoleNames = u.UserRoles?.Select(ur => _roleRepository.GetById(ur.RoleId).Name ?? "N/A").ToList() ?? new List<string>(),
                Created = u.Created,
                LastModified = u.LastModified,
                ModerationStatus = u.ModerationStatus,
                ModerationStatusTxt = u.ModerationStatus switch
                {
                    ModerationStatus.Approved => AppConstants.Approved,
                    ModerationStatus.Rejected => AppConstants.Rejected,
                    _ => AppConstants.NotDetermined
                }
            }).ToList();
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <returns>Thông tin người dùng dưới dạng DTO.</returns>
        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            var userRoles = await _userRoleRepository.FindAllAsync(u => u.UserId == id, u => u.Role!);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleNames = userRoles.Select(u => u.Role?.Name ?? "N/A").ToList(),
                Created = user.Created,
                LastModified = user.LastModified,
                ModerationStatus = user.ModerationStatus,
                ModerationStatusTxt = user.ModerationStatus switch
                {
                    ModerationStatus.Approved => AppConstants.Approved,
                    ModerationStatus.Rejected => AppConstants.Rejected,
                    _ => AppConstants.NotDetermined
                }
            };
        }

        /// <summary>
        /// Thêm người dùng mới.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu để tạo người dùng.</param>
        /// <returns>True nếu thêm thành công, ngược lại là false.</returns>
        public async Task<bool> CreateAsync(CreateUserRequest request)
        {
            var existingUser = await _userRepository.FindFirstAsync(u => u.Email == request.Email);
            if (existingUser is not null)
                throw new ArgumentException("Email đã được sử dụng.", nameof(request.Email));

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.AddAsync(user);
            var result = await _userRepository.SaveChangesAsync();

            if (result && request.RoleIds?.Any() == true)
            {
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null)
                        throw new ArgumentException($"Vai trò {roleId} không tồn tại.", nameof(request.RoleIds));

                    await _userRoleRepository.AddAsync(new UserRole { UserId = user.Id, RoleId = roleId });
                }
                await _userRoleRepository.SaveChangesAsync();
            }

            return result;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="request">Thông tin yêu cầu để cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        public async Task<bool> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var existingUser = await _userRepository.FindFirstAsync(u => u.Email == request.Email && u.Id != id);
            if (existingUser != null)
                throw new ArgumentException("Email đã được sử dụng.", nameof(request.Email));

            var user = await _userRepository.GetByIdAsync(id);

            user.UserName = request.UserName;
            user.Email = request.Email;

            _userRepository.Update(user);
            var result = await _userRepository.SaveChangesAsync();

            if (result && request.RoleIds?.Any() == true)
            {
                // Xóa các vai trò cũ
                var existingRoles = await _userRoleRepository.FindAllAsync(ur => ur.UserId == id);
                foreach (var role in existingRoles)
                {
                    _userRoleRepository.Delete(role);
                }

                // Thêm vai trò mới
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role is null)
                        throw new ArgumentException($"Vai trò {roleId} không tồn tại.", nameof(request.RoleIds));

                    await _userRoleRepository.AddAsync(new UserRole { UserId = id, RoleId = roleId });
                }
                await _userRoleRepository.SaveChangesAsync();
                await RefreshUserRolesCacheAsync(id);
            }

            return result;
        }

        /// <summary>
        /// Xóa người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            _userRepository.Delete(user);
            var result = await _userRepository.SaveChangesAsync();
            if (result)
            {
                _cache.Remove($"user_{id}");
            }
            return result;
        }

        /// <summary>
        /// Đăng nhập và tạo JWT token cho người dùng.
        /// </summary>
        /// <param name="request">Thông tin đăng nhập (email và mật khẩu).</param>
        /// <returns>Thông tin token bao gồm thời hạn hết hạn, token, và thông tin người dùng.</returns>
        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _userRepository.FindFirstAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");
            var userRoles = await _userRoleRepository.FindAllAsync(ur => ur.UserId == user.Id, ur => ur.Role!);
            // Lấy danh sách RoleId
            var roleIds = userRoles?.Select(ur => ur.RoleId).ToList() ?? new List<Guid>();
            // Lưu RoleId vào cache
            _cache.Set($"user_{user.Id}", roleIds, new MemoryCacheEntryOptions
            {
                SlidingExpiration = _cacheExpiration
            });
            // Lưu quyền của từng vai trò vào cache
            foreach (var roleId in roleIds)
            {
                if (!_cache.TryGetValue($"role_{roleId}", out List<string>? _))
                {
                    var permissions = await GetRolePermissionsAsync(roleId);
                    _cache.Set($"role_{roleId}", permissions, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = _cacheExpiration
                    });
                }
            }
            // Tạo JWT token
            return _jwtTokenService.GenerateToken(user);
        }

        /// <summary>
        /// Gán vai trò cho người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="roleId">ID của vai trò.</param>
        /// <returns>True nếu gán thành công, ngược lại là false.</returns>
        public async Task<bool> AssignRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (role is null)
                throw new ArgumentException("Vai trò không tồn tại.", nameof(roleId));

            var existing = await _userRoleRepository.FindFirstAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (existing is not null)
                return true;

            await _userRoleRepository.AddAsync(new UserRole { UserId = userId, RoleId = roleId });
            var result = await _userRoleRepository.SaveChangesAsync();
            if (result)
            {
                await RefreshUserRolesCacheAsync(userId);
            }
            return result;
        }

        /// <summary>
        /// Gỡ bỏ vai trò khỏi người dùng.
        /// </summary>
        public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId)
        {
            var userRole = await _userRoleRepository.FindFirstAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (userRole is null)
                return false;

            _userRoleRepository.Delete(userRole);
            var result = await _userRoleRepository.SaveChangesAsync();
            if (result)
            {
                await RefreshUserRolesCacheAsync(userId);
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách quyền của vai trò từ cơ sở dữ liệu.
        /// </summary>
        private async Task<List<string>> GetRolePermissionsAsync(Guid roleId)
        {
            return (await _permissionRepository.FindAllAsync(
                p => p.RolePermissions!.Any(rp => rp.RoleId == roleId),
                p => p.RolePermissions!))
                .Select(p => p.Name)
                .ToList();
        }
        private async Task RefreshUserRolesCacheAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var userRoles = await _userRoleRepository.FindAllAsync(ur => ur.UserId == user.Id, ur => ur.Role!);
            if (user != null)
            {
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList() ?? new List<Guid>();
                _cache.Set($"user_{userId}", roleIds, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = _cacheExpiration
                });
            }
        }
    }
}

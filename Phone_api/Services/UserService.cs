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
        private readonly JwtTokenService _jwtTokenService;

        /// <summary>
        /// Khởi tạo UserService với các repository và dịch vụ JWT.
        /// </summary>
        /// <param name="userRepository">Repository cho User.</param>
        /// <param name="roleRepository">Repository cho Role.</param>
        /// <param name="permissionRepository">Repository cho Permission.</param>
        /// <param name="jwtTokenService">Dịch vụ tạo JWT token.</param>
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
            IPermissionRepository permissionRepository, JwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        /// <returns>Danh sách người dùng dưới dạng DTO.</returns>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                RoleId = u.RoleId,
                RoleName = u.RoleId is not null ? _roleRepository.GetById(!u.RoleId.HasValue ? Guid.Empty : u.RoleId.Value).Name : "N/A",
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
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role != null ? user.Role.Name : "N/A",
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
            // Kiểm tra email đã tồn tại
            var existingUser = await _userRepository.FindFirstAsync(u => u.Email == request.Email);
            if (existingUser != null)
                throw new ArgumentException("Email đã được sử dụng.", nameof(request.Email));

            // Kiểm tra RoleId hợp lệ
            if (request.RoleId.HasValue)
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
                if (role == null)
                    throw new ArgumentException("Vai trò không tồn tại.", nameof(request.RoleId));
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId,
            };

            await _userRepository.AddAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="request">Thông tin yêu cầu để cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        public async Task<bool> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            // Kiểm tra email đã tồn tại (ngoại trừ user hiện tại)
            var existingUser = await _userRepository.FindFirstAsync(u => u.Email == request.Email && u.Id != id);
            if (existingUser != null)
                throw new ArgumentException("Email đã được sử dụng.", nameof(request.Email));

            // Kiểm tra RoleId hợp lệ
            if (request.RoleId.HasValue)
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
                if (role == null)
                    throw new ArgumentException("Vai trò không tồn tại.", nameof(request.RoleId));
            }

            var user = await _userRepository.GetByIdAsync(id);

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.RoleId = request.RoleId;

            _userRepository.Update(user);
            return await _userRepository.SaveChangesAsync();
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
            return await _userRepository.SaveChangesAsync();
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

            var permissions = user.RoleId.HasValue
                ? (await _permissionRepository.FindAllAsync(
                    p => p.RolePermissions!.Any(rp => rp.RoleId == user.RoleId),
                    p => p.RolePermissions!)) 
                    .Select(p => p.Name)
                    .ToList()
                : new List<string>();

            // Tạo JWT token
            return _jwtTokenService.GenerateToken(user, permissions);
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

            if (role == null)
                throw new ArgumentException("Vai trò không tồn tại.", nameof(roleId));

            user.RoleId = roleId;

            _userRepository.Update(user);
            return await _userRepository.SaveChangesAsync();
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Phone_api.Dtos;
using Phone_api.Entities;
using Phone_api.Repositories;

namespace Phone_api.Services
{
    /// <summary>
    /// Dịch vụ quản lý vai trò và quyền, sử dụng repository cụ thể.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);
        private readonly IMemoryCache _cache;
        /// <summary>
        /// Khởi tạo RoleService với các repository.
        /// </summary>
        /// <param name="roleRepository">Repository cho Role.</param>
        /// <param name="permissionRepository">Repository cho Permission.</param>
        /// <param name="rolePermissionRepository">Repository cho RolePermission.</param>
        /// <param name="cache">Repository cho MemoryCache.</param>
        public RoleService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IRolePermissionRepository rolePermissionRepository,
            IMemoryCache cache
            )
        {
            _cache = cache;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        /// <summary>
        /// Lấy danh sách tất cả vai trò cùng các quyền liên quan.
        /// </summary>
        /// <returns>Danh sách RoleDto.</returns>
        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.FindAllAsync(r => true, r => r.RolePermissions!);

            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Permissions = r.RolePermissions.Select(
                    rp => _permissionRepository.GetById(string.IsNullOrEmpty(rp.PermissionId.ToString()) ? Guid.Empty : rp.PermissionId).Name
                ).ToList()
            });
        }

        /// <summary>
        /// Thêm vai trò mới.
        /// </summary>
        /// <param name="request">Thông tin vai trò cần thêm.</param>
        /// <returns>ID của vai trò vừa tạo.</returns>
        public async Task<Guid> AddAsync(CreateRoleRequest request)
        {
            var role = new Role
            {
                Name = request.Name
            };
            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();
            return role.Id;
        }

        /// <summary>
        /// Cập nhật thông tin vai trò.
        /// </summary>
        /// <param name="id">ID vai trò.</param>
        /// <param name="request">Thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công.</returns>
        public async Task<bool> UpdateAsync(Guid id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            role.Name = request.Name;
            _roleRepository.Update(role);
            return await _roleRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa vai trò.
        /// </summary>
        /// <param name="id">ID vai trò.</param>
        /// <returns>True nếu xóa thành công.</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            _roleRepository.Delete(role);
            var result = await _roleRepository.SaveChangesAsync();
            if (result)
            {
                _cache.Remove($"role_{id}");
            }
            return result;
        }

        /// <summary>
        /// Thêm quyền cho vai trò.
        /// </summary>
        /// <param name="roleId">ID vai trò.</param>
        /// <param name="permissionId">ID quyền.</param>
        /// <returns>True nếu thêm thành công hoặc quyền đã tồn tại.</returns>
        public async Task<bool> AddRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            var existing = await _rolePermissionRepository.FindFirstAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (existing != null)
                return true;

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };
            await _rolePermissionRepository.AddAsync(rolePermission);
            var result = await _rolePermissionRepository.SaveChangesAsync();
            if (result)
            {
                var permissions = await GetRolePermissionsAsync(roleId);
                _cache.Set($"role_{roleId}", permissions, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = _cacheExpiration
                });
            }
            return result;
        }

        /// <summary>
        /// Xóa quyền khỏi vai trò.
        /// </summary>
        /// <param name="roleId">ID vai trò.</param>
        /// <param name="permissionId">ID quyền.</param>
        /// <returns>True nếu xóa thành công.</returns>
        public async Task<bool> RemoveRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            var rolePermission = await _rolePermissionRepository.FindFirstAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (rolePermission == null)
                return false;

            _rolePermissionRepository.Delete(rolePermission);
            var result = await _rolePermissionRepository.SaveChangesAsync();
            if (result)
            {
                var permissions = await GetRolePermissionsAsync(roleId);
                _cache.Set($"role_{roleId}", permissions, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = _cacheExpiration
                });
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
    }
}

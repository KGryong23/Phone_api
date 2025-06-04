using Phone_api.Dtos;
using Phone_api.Repositories;

namespace Phone_api.Services
{
    /// <summary>
    /// Dịch vụ quản lý quyền, sử dụng IPermissionRepository.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        /// <summary>
        /// Khởi tạo PermissionService với repository.
        /// </summary>
        /// <param name="permissionRepository">Repository cho Permission.</param>
        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Lấy danh sách tất cả quyền.
        /// </summary>
        /// <returns>Danh sách PermissionDto.</returns>
        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name
            });
        }
    }
}

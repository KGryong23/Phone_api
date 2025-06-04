using Phone_api.Dtos;

namespace Phone_api.Services
{
    /// <summary>
    /// Giao diện định nghĩa các phương thức dịch vụ quản lý vai trò và quyền.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Lấy danh sách tất cả vai trò cùng các quyền liên quan.
        /// </summary>
        /// <returns>Danh sách RoleDto.</returns>
        Task<IEnumerable<RoleDto>> GetAllAsync();

        /// <summary>
        /// Thêm vai trò mới.
        /// </summary>
        /// <param name="request">Thông tin vai trò cần thêm.</param>
        /// <returns>ID của vai trò vừa tạo.</returns>
        Task<Guid> AddAsync(CreateRoleRequest request);

        /// <summary>
        /// Cập nhật thông tin vai trò.
        /// </summary>
        /// <param name="id">ID vai trò.</param>
        /// <param name="request">Thông tin cập nhật.</param>
        /// <returns>True nếu cập nhật thành công.</returns>
        Task<bool> UpdateAsync(Guid id, UpdateRoleRequest request);

        /// <summary>
        /// Xóa vai trò.
        /// </summary>
        /// <param name="id">ID vai trò.</param>
        /// <returns>True nếu xóa thành công.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Thêm quyền cho vai trò.
        /// </summary>
        /// <param name="roleId">ID vai trò.</param>
        /// <param name="permissionId">ID quyền.</param>
        /// <returns>True nếu thêm thành công.</returns>
        Task<bool> AddRolePermissionAsync(Guid roleId, Guid permissionId);

        /// <summary>
        /// Xóa quyền khỏi vai trò.
        /// </summary>
        /// <param name="roleId">ID vai trò.</param>
        /// <param name="permissionId">ID quyền.</param>
        /// <returns>True nếu xóa thành công.</returns>
        Task<bool> RemoveRolePermissionAsync(Guid roleId, Guid permissionId);
    }
}

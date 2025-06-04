using Phone_api.Dtos;

namespace Phone_api.Services
{
    /// <summary>
    /// Giao diện xử lý logic nghiệp vụ cho người dùng và phân quyền.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        /// <returns>Danh sách người dùng dưới dạng DTO.</returns>
        Task<IEnumerable<UserDto>> GetAllAsync();

        /// <summary>
        /// Lấy thông tin người dùng theo ID.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <returns>Thông tin người dùng dưới dạng DTO.</returns>
        Task<UserDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Thêm người dùng mới.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu để tạo người dùng.</param>
        /// <returns>True nếu thêm thành công, ngược lại là false.</returns>
        Task<bool> CreateAsync(CreateUserRequest request);

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="request">Thông tin yêu cầu để cập nhật.</param>
        /// <returns>True nếu cập nhật thành công, ngược lại là false.</returns>
        Task<bool> UpdateAsync(Guid id, UpdateUserRequest request);

        /// <summary>
        /// Xóa người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Đăng nhập và tạo JWT token cho người dùng.
        /// </summary>
        /// <param name="request">Thông tin đăng nhập (email và mật khẩu).</param>
        /// <returns>Thông tin token bao gồm thời hạn hết hạn, token, và thông tin người dùng.</returns>
        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);

        /// <summary>
        /// Gán vai trò cho người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="roleId">ID của vai trò.</param>
        /// <returns>True nếu gán thành công, ngược lại là false.</returns>
        Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    }
}

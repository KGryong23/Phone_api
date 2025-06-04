using Phone_api.Enums;

namespace Phone_api.Dtos
{
    /// <summary>
    /// DTO cho thông tin người dùng.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID của người dùng.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên người dùng.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email của người dùng.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// ID của vai trò được gán cho người dùng.
        /// </summary>
        public Guid? RoleId { get; set; }

        /// <summary>
        /// Tên của vai trò được gán.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian tạo bản ghi.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Thời gian chỉnh sửa cuối.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Trạng thái duyệt của bản ghi.
        /// </summary>
        public ModerationStatus ModerationStatus { get; set; }

        /// <summary>
        /// Mô tả trạng thái duyệt dưới dạng chuỗi.
        /// </summary>
        public string? ModerationStatusTxt { get; set; }
    }

    /// <summary>
    /// Yêu cầu tạo người dùng mới.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// Tên người dùng.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email của người dùng.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu của người dùng.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// ID của vai trò được gán (tùy chọn).
        /// </summary>
        public Guid? RoleId { get; set; }
    }

    /// <summary>
    /// Yêu cầu cập nhật thông tin người dùng.
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// Tên người dùng.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email của người dùng.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// ID của vai trò được gán (tùy chọn).
        /// </summary>
        public Guid? RoleId { get; set; }
    }

    /// <summary>
    /// Yêu cầu đăng nhập.
    /// </summary>
    public class UserLoginRequest
    {
        /// <summary>
        /// Email của người dùng.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu của người dùng.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Phản hồi khi đăng nhập thành công.
    /// </summary>
    public class UserLoginResponse
    {
        /// <summary>
        /// Thời gian hết hạn của token (Unix timestamp).
        /// </summary>
        public long Expire { get; set; }

        /// <summary>
        /// JWT token.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin người dùng (dưới dạng chuỗi hoặc JSON).
        /// </summary>
        public UserDto? User { get; set; }
    }
}

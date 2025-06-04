namespace Phone_api.Entities
{
    /// <summary>
    /// Đại diện cho bảng trung gian giữa Role và Permission, hỗ trợ mối quan hệ nhiều-nhiều.
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// ID của vai trò (Role) trong mối quan hệ.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Vai trò (Role) được gán quyền, tham chiếu đến bảng Role.
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// ID của quyền (Permission) trong mối quan hệ.
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Quyền (Permission) được gán cho vai trò, tham chiếu đến bảng Permission.
        /// </summary>
        public Permission Permission { get; set; } = null!;
    }
}

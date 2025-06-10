namespace Phone_api.Entities
{
    /// <summary>
    /// Đại diện cho mối quan hệ nhiều-nhiều giữa User và Role.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// ID của người dùng. 
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID của vai trò.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Người dùng liên quan.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Vai trò liên quan.
        /// </summary>
        public Role? Role { get; set; }
    }
}

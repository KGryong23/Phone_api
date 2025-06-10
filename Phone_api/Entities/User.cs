namespace Phone_api.Entities
{
    /// <summary>
    /// Đại diện cho một người dùng trong hệ thống.
    /// </summary>
    public class User : BaseDomainEntity
    {
        private string _userName = null!;

        /// <summary>
        /// Tên người dùng, dùng để đăng nhập hoặc nhận diện người dùng.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("UserName cannot be empty.");
                _userName = value;
            }
        }

        private string _email = null!;

        /// <summary>
        /// Địa chỉ email của người dùng, phải là duy nhất.
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty.");
                _email = value;
            }
        }

        private string _passwordHash = null!;

        /// <summary>
        /// Chuỗi băm của mật khẩu người dùng.
        /// </summary>
        public string PasswordHash
        {
            get => _passwordHash;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("PasswordHash cannot be empty.");
                _passwordHash = value;
            }
        }

        /// <summary>
        /// Danh sách vai trò của người dùng, đại diện cho mối quan hệ nhiều-nhiều.
        /// </summary>
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}

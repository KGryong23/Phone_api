namespace Phone_api.Entities
{
    /// <summary>
    /// Đại diện cho một quyền trong hệ thống, xác định endpoint và phương thức HTTP.
    /// </summary>
    public class Permission : BaseDomainEntity
    {

        private string _name = null!;

        /// <summary>
        /// Tên quyền (ví dụ: phone.read, phone.update).
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }
        /// <summary>
        /// Danh sách vai trò được gán quyền này, đại diện cho mối quan hệ nhiều-nhiều.
        /// </summary>
        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}

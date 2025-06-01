using Phone_api.Enums;

namespace Phone_api.Entities
{
    public abstract class BaseDomainEntity
    {
        /// <summary>
        /// ID cho bản ghi, sử dụng Guid làm khóa chính
        /// </summary>
        public Guid Id { get; protected init; }

        /// <summary>
        /// Thời gian tạo bản ghi
        /// </summary>
        private DateTime _created;
        public DateTime Created
        {
            get => _created;
            protected set => _created = value;
        }

        /// <summary>
        /// Thời gian chỉnh sửa cuối (lưu dưới dạng UTC)
        /// </summary>
        private DateTime _lastModified;
        public DateTime LastModified
        {
            get => _lastModified;
            protected set => _lastModified = value;
        }

        /// <summary>
        /// Trạng thái duyệt của bản ghi
        /// </summary>
        public ModerationStatus ModerationStatus { get; set; }

        /// <summary>
        /// Điền dữ liệu khi thêm mới
        /// </summary>
        public void FillDataForInsert()
        {
            Created = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            ModerationStatus = ModerationStatus.Rejected;
        }

        /// <summary>
        /// Điền dữ liệu khi cập nhật
        /// </summary>
        public void FillDataForUpdate()
        {
            LastModified = DateTime.UtcNow;
        }
    }
}

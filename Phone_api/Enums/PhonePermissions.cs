namespace Phone_api.Enums
{
    /// <summary>
    /// Các quyền liên quan đến Phone.
    /// </summary>
    public enum PhonePermissions
    {
        /// <summary>
        /// Quyền lấy thông tin phone theo ID.
        /// </summary>
        GetById,

        /// <summary>
        /// Quyền lấy danh sách phân trang phone.
        /// </summary>
        GetPaged,

        /// <summary>
        /// Quyền tạo phone mới.
        /// </summary>
        Create,

        /// <summary>
        /// Quyền cập nhật phone.
        /// </summary>
        Update,

        /// <summary>
        /// Quyền xóa phone.
        /// </summary>
        Delete,

        /// <summary>
        /// Quyền duyệt phone.
        /// </summary>
        Approve,

        /// <summary>
        /// Quyền hủy duyệt phone.
        /// </summary>
        Reject
    }
}

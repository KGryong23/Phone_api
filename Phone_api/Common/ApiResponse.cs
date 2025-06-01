namespace Phone_api.Common
{
    /// <summary>
    /// Đại diện cho response chuẩn của API.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của Data.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Trạng thái thành công của response.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông báo của response.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Dữ liệu trả về.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Danh sách lỗi validate (nếu có).
        /// </summary>
        public Dictionary<string, string[]>? Errors { get; set; }

        /// <summary>
        /// Tạo response thành công.
        /// </summary>
        /// <param name="data">Dữ liệu trả về.</param>
        /// <param name="message">Thông báo (tùy chọn).</param>
        public static ApiResponse<T> SuccessResult(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message };

        /// <summary>
        /// Tạo response lỗi.
        /// </summary>
        /// <param name="message">Thông báo lỗi.</param>
        /// <param name="errors">Danh sách lỗi validate (tùy chọn).</param>
        public static ApiResponse<T> ErrorResult(string message, Dictionary<string, string[]>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}

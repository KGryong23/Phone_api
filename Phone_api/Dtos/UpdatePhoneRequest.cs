using Phone_api.Common;
using System.ComponentModel.DataAnnotations;

namespace Phone_api.Dtos
{
    /// <summary>
    /// Đại diện cho yêu cầu cập nhật một điện thoại hiện có.
    /// </summary>
    public class UpdatePhoneRequest
    {
        /// <summary>
        /// Tên mẫu điện thoại (bắt buộc).
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = "EmptyModel")]
        public string Model { get; set; } = null!;

        /// <summary>
        /// Giá của điện thoại (phải lớn hơn 0).
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = "InvalidPrice")]
        public decimal Price { get; set; }

        /// <summary>
        /// Số lượng tồn kho của điện thoại (phải không âm).
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(AppResources), ErrorMessageResourceName = "InvalidStock")]
        public int Stock { get; set; }

        /// <summary>
        /// ID của thương hiệu liên kết với điện thoại (không bắt buộc).
        /// </summary>
        public Guid? BrandId { get; set; }
    }
}

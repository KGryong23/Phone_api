using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Phone_api.Common;

namespace Phone_api.Extensions
{
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// Trả về lỗi khi ID không hợp lệ (Guid.Empty) với mã 400 Bad Request.
        /// </summary>
        /// <param name="controller">Controller gọi phương thức.</param>
        /// <returns>Response chứa lỗi validate cho ID.</returns>
        public static IActionResult BadRequestForInvalidId(this ControllerBase controller)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Id", AppResources.InvalidId);
            var errors = GetValidationErrors(modelState);
            return new BadRequestObjectResult(ApiResponse<object>.ErrorResult("ID không hợp lệ.", errors));
        }

        /// <summary>
        /// Lấy danh sách lỗi validate từ ModelState.
        /// </summary>
        /// <param name="modelState">ModelState chứa lỗi validate.</param>
        /// <returns>Danh sách lỗi theo định dạng Dictionary.</returns>
        public static Dictionary<string, string[]> GetValidationErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(m => m.Value != null && m.Value.Errors != null && m.Value.Errors.Any())
                .ToDictionary(
                    m => m.Key.ToLowerInvariant(),
                    m => m.Value!.Errors.Select(e => e.ErrorMessage ?? "Lỗi không xác định").ToArray()
                );
        }
    }
}

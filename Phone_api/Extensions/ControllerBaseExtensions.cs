using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Phone_api.Extensions
{
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// Trả về lỗi validate với mã 400 Bad Request.
        /// </summary>
        /// <param name="controller">Controller gọi phương thức.</param>
        /// <param name="modelState">ModelState chứa lỗi validate.</param>
        /// <returns>Response chứa danh sách lỗi validate.</returns>
        public static IActionResult BadRequestFromModelState(this ControllerBase controller, ModelStateDictionary modelState)
        {
            var errors = GetValidationErrors(modelState);
            return new ObjectResult(new { errors }) { StatusCode = 400 };
        }

        /// <summary>
        /// Trả về lỗi từ ArgumentException với mã 400 Bad Request.
        /// </summary>
        /// <param name="controller">Controller gọi phương thức.</param>
        /// <param name="exception">ArgumentException chứa thông tin lỗi.</param>
        /// <returns>Response chứa lỗi với key là tên tham số hoặc 'General'.</returns>
        public static IActionResult BadRequestFromArgumentException(this ControllerBase controller, ArgumentException exception)
        {
            var paramName = string.IsNullOrEmpty(exception.ParamName) ? "General" : exception.ParamName;
            var errorMessage = exception.Message;
            if (!string.IsNullOrEmpty(exception.ParamName))
            {
                var paramSuffix = $" (Parameter '{exception.ParamName}')";
                errorMessage = errorMessage.Replace(paramSuffix, string.Empty);
            }
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(paramName, errorMessage);
            return controller.BadRequestFromModelState(modelState);
        }

        /// <summary>
        /// Lấy danh sách lỗi validate từ ModelState.
        /// </summary>
        /// <param name="modelState">ModelState chứa lỗi validate.</param>
        /// <returns>Danh sách lỗi theo định dạng Dictionary.</returns>
        private static Dictionary<string, string[]> GetValidationErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(m => m.Value != null && m.Value.Errors != null && m.Value.Errors.Any())
                .ToDictionary(
                    m => m.Key,
                    m => m.Value!.Errors.Select(e => e.ErrorMessage ?? "Lỗi không xác định").ToArray()
                );
        }
    }
}

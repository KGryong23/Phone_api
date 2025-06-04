using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Phone_api.Common;
using Phone_api.Extensions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Phone_api.Filters
{
    /// <summary>
    /// Attribute kiểm tra quyền dựa trên JWT token và PermissionMap.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Sử dụng attribute này để yêu cầu quyền truy cập cho các endpoint cụ thể.
        /// </summary>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // [Authorize] đã đảm bảo xác thực, nên không cần kiểm tra token ở đây

            // Chuẩn hóa endpoint
            var endpoint = NormalizeEndpoint(context.HttpContext.Request.Path.Value!);
            var method = context.HttpContext.Request.Method;
            var requiredPermission = $"{endpoint}:{method}";

            var permissionsJson = context.HttpContext.User.FindFirst("permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsJson))
            {
                context.Result = new JsonResult(ApiResponse<object>.ErrorResult("Access denied: No permissions found."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            var userPermissions = JsonSerializer.Deserialize<string[]>(permissionsJson);
            if (userPermissions == null)
            {
                context.Result = new JsonResult(ApiResponse<object>.ErrorResult("Access denied: Invalid permissions format."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Kiểm tra quyền
            var hasPermission = userPermissions.Any(perm =>
                PermissionMap.GetEndpoint(perm)?.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase) == true);

            if (!hasPermission)
            {
                context.Result = new JsonResult(ApiResponse<object>.ErrorResult("Access denied: Insufficient permissions."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }

        /// <summary>
        /// Chuẩn hóa endpoint, loại bỏ tham số path cho mọi controller.
        /// </summary>
        /// <param name="endpoint">Endpoint gốc (ví dụ: /api/Phone/GetById/123).</param>
        /// <returns>Endpoint chuẩn hóa (ví dụ: /api/Phone/GetById).</returns>
        private static string NormalizeEndpoint(string endpoint)
        {
            // Xử lý các route dạng /api/Controller/Action/{id}
            var pattern = @"^/api/(\w+)/(\w+)(?:/[^/]+)?$";
            var match = Regex.Match(endpoint, pattern);
            if (match.Success)
            {
                var controller = match.Groups[1].Value;
                var action = match.Groups[2].Value;
                return $"/api/{controller}/{action}";
            }

            // Trả về endpoint gốc nếu không khớp
            return endpoint;
        }
    }
}

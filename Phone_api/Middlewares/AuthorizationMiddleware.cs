using Phone_api.Common;
using Phone_api.Extensions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Phone_api.Middlewares
{
    /// <summary>
    /// Middleware kiểm tra quyền truy cập dựa trên JWT token.
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Khởi tạo AuthorizationMiddleware.
        /// </summary>
        /// <param name="next">Delegate tiếp theo trong pipeline.</param>
        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Xử lý request và kiểm tra quyền từ token JWT.
        /// </summary>
        /// <param name="context">HttpContext của request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // Chuẩn hóa endpoint, loại bỏ tham số path
            var endpoint = NormalizeEndpoint(context.Request.Path.Value!);
            var method = context.Request.Method;
            var requiredPermission = $"{endpoint}:{method}";

            var permissionsJson = context.User.FindFirst("permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsJson))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.ErrorResult("Access denied: No permissions found."));
                return;
            }

            var userPermissions = JsonSerializer.Deserialize<string[]>(permissionsJson);
            if (userPermissions == null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.ErrorResult("Access denied: Invalid permissions format."));
                return;
            }

            // Kiểm tra xem user có quyền phù hợp không
            var hasPermission = userPermissions.Any(perm =>
                PermissionMap.GetEndpoint(perm)?.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase) == true);

            if (!hasPermission)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.ErrorResult("Access denied: Insufficient permissions."));
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// Chuẩn hóa endpoint, loại bỏ tham số path.
        /// </summary>
        /// <param name="endpoint">Endpoint gốc (ví dụ: /api/Phone/GetById/123).</param>
        /// <returns>Endpoint chuẩn hóa (ví dụ: /api/Phone/GetById).</returns>
        private static string NormalizeEndpoint(string endpoint)
        {
            // Loại bỏ tham số path bằng regex
            var pattern = @"(/api/\w+/\w+)(?:/[^/]+)?";
            var match = Regex.Match(endpoint, pattern);
            return match.Success ? match.Groups[1].Value : endpoint;
        }
    }
}

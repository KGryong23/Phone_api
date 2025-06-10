using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Phone_api.Common;
using Phone_api.Extensions;
using Phone_api.Repositories;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Phone_api.Filters
{
    /// <summary>
    /// Attribute kiểm tra quyền dựa trên JWT token và PermissionMap.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequirePermissionAttribute() : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Sử dụng attribute này để yêu cầu quyền truy cập cho các endpoint cụ thể.
        /// </summary>
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            // Lấy userId từ JWT
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                context.Result = new JsonResult(ApiResponse<object>.ErrorResult("Access denied: Invalid user."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Lấy các service từ DI container
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var permissionRepository = context.HttpContext.RequestServices.GetRequiredService<IPermissionRepository>();
            var userRoleRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRoleRepository>();

            // Chuẩn hóa endpoint
            var endpoint = NormalizeEndpoint(context.HttpContext.Request.Path.Value!);
            var method = context.HttpContext.Request.Method;
            var requiredPermission = $"{endpoint}:{method}";

            // Lấy RoleId từ cache
            if (!cache.TryGetValue($"user_{userId}", out List<Guid>? roleIds))
            {
                // Cache miss: lấy từ database và cập nhật cache
                roleIds = (await userRoleRepository.FindAllAsync(ur => ur.UserId == userId))
                    .Select(ur => ur.RoleId)
                    .ToList();
                cache.Set($"user_{userId}", roleIds, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
            }

            if (roleIds is null || !roleIds.Any())
            {
                context.Result = new JsonResult(ApiResponse<object>.ErrorResult("Access denied: No roles assigned."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Kiểm tra quyền từ cache cho từng RoleId
            foreach (var roleId in roleIds)
            {
                if (!cache.TryGetValue($"role_{roleId}", out List<string>? rolePermissions))
                {
                    // Cache miss: lấy từ database và cập nhật cache
                    rolePermissions = await GetRolePermissionsAsync(permissionRepository, roleId);
                    cache.Set($"role_{roleId}", rolePermissions, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    });
                }

                var hasPermission = rolePermissions?.Any(perm =>
                    PermissionMap.GetEndpoint(perm)?.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase) == true);

                if (hasPermission is not null && hasPermission == true)
                    return; // Thoát nếu tìm thấy quyền
            }

            context.Result = new JsonResult(ApiResponse<object>.ErrorResult("Access denied: Insufficient permissions."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
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

        private async Task<List<string>> GetRolePermissionsAsync(IPermissionRepository permissionRepository, Guid roleId)
        {
            return (await permissionRepository.FindAllAsync(
                p => p.RolePermissions!.Any(rp => rp.RoleId == roleId),
                p => p.RolePermissions!))
                .Select(p => p.Name)
                .ToList();
        }
    }
}

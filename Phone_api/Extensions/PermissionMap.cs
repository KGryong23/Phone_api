using Phone_api.Enums;

namespace Phone_api.Extensions
{
    /// <summary>
    /// Quản lý ánh xạ quyền từ tên quyền sang endpoint và phương thức HTTP.
    /// </summary>
    public static class PermissionMap
    {
        private static readonly Dictionary<string, Dictionary<PhonePermissions, string>> _controllerMaps = new()
        {
            {
                "phone", new Dictionary<PhonePermissions, string>
                {
                    { PhonePermissions.GetById, "/api/Phone/GetById:GET" },
                    { PhonePermissions.GetPaged, "/api/Phone/GetPaged:GET" },
                    { PhonePermissions.Create, "/api/Phone/Create:POST" },
                    { PhonePermissions.Update, "/api/Phone/Update:PUT" },
                    { PhonePermissions.Delete, "/api/Phone/Delete:DELETE" },
                    { PhonePermissions.Approve, "/api/Phone/Approve:PATCH" },
                    { PhonePermissions.Reject, "/api/Phone/Reject:PATCH" }
                }
            }
            // ............ controller khác có thể được
        };

        /// <summary>
        /// Chuyển PhonePermissions thành tên quyền.
        /// </summary>
        /// <param name="controller">Tên controller (ví dụ: phone).</param>
        /// <param name="permission">Quyền PhonePermissions.</param>
        /// <returns>Tên quyền (ví dụ: phone.getbyid).</returns>
        public static string ToPermissionName(string controller, PhonePermissions permission)
        {
            return $"{controller}.{permission.ToString().ToLowerInvariant()}";
        }

        /// <summary>
        /// Lấy endpoint từ tên quyền.
        /// </summary>
        /// <param name="permissionName">Tên quyền (ví dụ: phone.getbyid).</param>
        /// <returns>Endpoint (ví dụ: /api/Phone/GetById:GET) hoặc null nếu không tìm thấy.</returns>
        public static string? GetEndpoint(string permissionName)
        {
            var parts = permissionName.Split('.');
            if (parts.Length != 2)
                return null;

            var controller = parts[0];
            var action = parts[1];

            if (_controllerMaps.TryGetValue(controller, out var permissionMap))
            {
                foreach (var kvp in permissionMap)
                {
                    if (kvp.Key.ToString().ToLowerInvariant() == action)
                        return kvp.Value;
                }
            }

            return null;
        }
    }
}

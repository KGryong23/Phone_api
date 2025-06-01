using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Phone_api.Filters
{
    /// <summary>
    /// Operation filter để thêm mô tả cho response code dựa trên [ProducesResponseType].
    /// </summary>
    public class DefaultResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var producesAttributes = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<ProducesResponseTypeAttribute>()
                .ToList();

            // Xóa các response mặc định để tránh chung chung
            operation.Responses.Clear();

            foreach (var attr in producesAttributes)
            {
                var statusCode = attr.StatusCode.ToString();
                var description = GetResponseDescription(attr.StatusCode, context.MethodInfo.Name);
                operation.Responses[statusCode] = new OpenApiResponse
                {
                    Description = description,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(attr.Type ?? typeof(void), context.SchemaRepository)
                        }
                    }
                };
            }
        }

        private string GetResponseDescription(int statusCode, string methodName)
        {
            return (statusCode, methodName.ToLower()) switch
            {
                (200, "getpaged") => "OK: Trả về danh sách điện thoại phân trang.",
                (200, "getbyid") => "OK: Trả về thông tin điện thoại.",
                (201, "create") => "Created: Tạo điện thoại thành công.",
                (200, "update") => "OK: Cập nhật điện thoại thành công.",
                (200, "delete") => "OK: Xóa điện thoại thành công.",
                (200, "approve") => "OK: Duyệt điện thoại thành công.",
                (200, "reject") => "OK: Từ chối điện thoại thành công.",
                (400, "getpaged") => "Bad Request: Skip phải lớn hơn hoặc bằng 0 và Take phải lớn hơn 0.",
                (400, _) => "Bad Request: Dữ liệu yêu cầu không hợp lệ.",
                (404, _) => "Not Found: Không tìm thấy điện thoại.",
                (500, _) => "Internal Server Error: Lỗi server xảy ra.",
                _ => $"{statusCode}: Không xác định"
            };
        }
    }

    /// <summary>
    /// Operation filter để thêm mô tả cho tham số 'id'.
    /// </summary>
    public class IdParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var idParameter = operation.Parameters?.FirstOrDefault(p => p.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
            if (idParameter != null)
            {
                idParameter.Description = context.MethodInfo.Name.ToLower() switch
                {
                    "delete" => "Định danh duy nhất của điện thoại cần xóa.",
                    "approve" => "Định danh duy nhất của điện thoại cần duyệt.",
                    "reject" => "Định danh duy nhất của điện thoại cần từ chối.",
                    "update" => "Định danh duy nhất của điện thoại cần cập nhật.",
                    _ => "Định danh duy nhất của điện thoại."
                };
            }
        }
    }
}
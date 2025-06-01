using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Phone_api.Common;
using Phone_api.Exceptions;

namespace Phone_api.Middlewares
{
    /// <summary>
    /// Middleware xử lý ngoại lệ toàn cục trong ứng dụng.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Khởi tạo một instance mới của <see cref="ExceptionHandlingMiddleware"/>.
        /// </summary>
        /// <param name="next">Delegate tiếp theo trong pipeline.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Xử lý request và bắt các ngoại lệ phát sinh.
        /// </summary>
        /// <param name="context">HttpContext của request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                await HandleArgumentExceptionAsync(context, ex);
            }
            catch (NotFoundException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleArgumentExceptionAsync(HttpContext context, ArgumentException exception)
        {
            var paramName = string.IsNullOrEmpty(exception.ParamName) ? "general" : exception.ParamName;
            var errorMessage = exception.Message;
            if (!string.IsNullOrEmpty(exception.ParamName))
            {
                var paramSuffix = $" (Parameter '{exception.ParamName}')";
                errorMessage = errorMessage.Replace(paramSuffix, string.Empty);
            }

            var errors = new Dictionary<string, string[]> { { paramName, new[] { errorMessage } } };
            var result = ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await context.Response.WriteAsync(json);
        }

        private static async Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
        {
            var result = ApiResponse<object>.ErrorResult(exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await context.Response.WriteAsync(json);
        }


        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = ApiResponse<object>.ErrorResult(exception.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await context.Response.WriteAsync(json);
        }

    }
}

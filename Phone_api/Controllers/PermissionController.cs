using Microsoft.AspNetCore.Mvc;
using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Services;

namespace Phone_api.Controllers
{
    /// <summary>
    /// Controller quản lý API lấy danh sách quyền.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// Khởi tạo PermissionController với dịch vụ quyền.
        /// </summary>
        /// <param name="permissionService">Dịch vụ quản lý quyền.</param>
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Lấy danh sách tất cả quyền.
        /// </summary>
        /// <returns>Danh sách PermissionDto với mã trạng thái 200.</returns>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<PermissionDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResult(permissions));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Extensions;
using Phone_api.Services;

namespace Phone_api.Controllers
{
    /// <summary>
    /// Controller quản lý các API liên quan đến vai trò.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        /// <summary>
        /// Khởi tạo RoleController với dịch vụ vai trò.
        /// </summary>
        /// <param name="roleService">Dịch vụ quản lý vai trò.</param>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Lấy danh sách tất cả vai trò.
        /// </summary>
        /// <returns>Danh sách RoleDto với mã trạng thái 200.</returns>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<RoleDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(roles));
        }

        /// <summary>
        /// Tạo vai trò mới .
        /// </summary>
        /// <param name="request">Thông tin vai trò mới.</param>
        /// <returns>ID vai trò với mã trạng thái 201 hoặc 400 nếu dữ liệu không hợp lệ.</returns>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var roleId = await _roleService.AddAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = roleId }, ApiResponse<object>.SuccessResult(request, "Tạo vai trò thành công."));
        }

        /// <summary>
        /// Cập nhật vai trò .
        /// </summary>
        /// <param name="id">ID vai trò.</param>
        /// <param name="request">Thông tin cập nhật.</param>
        /// <returns>Mã trạng thái 200 nếu thành công, 404 nếu không tìm thấy.</returns>
        [HttpPut("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var success = await _roleService.UpdateAsync(id, request);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy vai trò."));

            return Ok(ApiResponse<object>.SuccessResult(id, "Cập nhật vai trò thành công."));
        }

        /// <summary>
        /// Xóa vai trò .
        /// </summary>
        /// <param name="id">ID vai trò.</param>
        /// <returns>Mã trạng thái 200 nếu thành công, 404 nếu không tìm thấy.</returns>
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _roleService.DeleteAsync(id);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy vai trò."));

            return Ok(ApiResponse<object>.SuccessResult(id, "Xóa vai trò thành công."));
        }

        /// <summary>
        /// Thêm quyền cho vai trò .
        /// </summary>
        /// <param name="roleId">ID vai trò.</param>
        /// <param name="permissionId">ID quyền.</param>
        /// <returns>Mã trạng thái 200 nếu thành công, 404 nếu không tìm thấy.</returns>
        [HttpPost("AddPermission/{roleId}/{permissionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> AddPermission(Guid roleId, Guid permissionId)
        {
            if (roleId == Guid.Empty || permissionId == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _roleService.AddRolePermissionAsync(roleId, permissionId);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy vai trò hoặc quyền."));

            return Ok(ApiResponse<object>.SuccessResult(new { roleId, permissionId }, "Thêm quyền cho vai trò thành công."));
        }

        /// <summary>
        /// Xóa quyền khỏi vai trò .
        /// </summary>
        /// <param name="roleId">ID vai trò.</param>
        /// <param name="permissionId">ID quyền.</param>
        /// <returns>Mã trạng thái 200 nếu thành công, 404 nếu không tìm thấy.</returns>
        [HttpDelete("RemovePermission/{roleId}/{permissionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId)
        {
            if (roleId == Guid.Empty || permissionId == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _roleService.RemoveRolePermissionAsync(roleId, permissionId);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy mối quan hệ vai trò-quyền."));

            return Ok(ApiResponse<object>.SuccessResult(new { roleId, permissionId }, "Xóa quyền khỏi vai trò thành công."));
        }
    }
}

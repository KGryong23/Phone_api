using Microsoft.AspNetCore.Mvc;
using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Extensions;
using Phone_api.Services;

namespace Phone_api.Controllers
{
    /// <summary>
    /// Controller quản lý các thao tác liên quan đến người dùng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Khởi tạo một instance mới của <see cref="UserController"/>.
        /// </summary>
        /// <param name="userService">Dịch vụ xử lý logic nghiệp vụ người dùng.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng .
        /// </summary>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<UserDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var result = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của người dùng.</param>
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var user = await _userService.GetByIdAsync(id);
            if (user is null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy người dùng."));

            return Ok(ApiResponse<UserDto>.SuccessResult(user));
        }

        /// <summary>
        /// Tạo một người dùng mới.
        /// </summary>
        /// <param name="request">Yêu cầu chứa thông tin người dùng cần tạo.</param>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var userId = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = userId }, ApiResponse<object>.SuccessResult(request, "Tạo người dùng thành công."));
        }

        /// <summary>
        /// Cập nhật thông tin một người dùng hiện có.
        /// </summary>
        /// <param name="id">Định danh duy nhất của người dùng cần cập nhật.</param>
        /// <param name="request">Yêu cầu chứa thông tin người dùng cập nhật.</param>
        [HttpPut("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var success = await _userService.UpdateAsync(id, request);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy người dùng."));

            return Ok(ApiResponse<object>.SuccessResult(id, "Cập nhật người dùng thành công."));
        }

        /// <summary>
        /// Xóa một người dùng theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của người dùng cần xóa.</param>
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _userService.DeleteAsync(id);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy người dùng."));

            return Ok(ApiResponse<object>.SuccessResult(id, "Xóa người dùng thành công."));
        }
        /// <summary>
        /// Đăng nhập và tạo JWT token cho người dùng.
        /// </summary>
        /// <param name="request">Thông tin đăng nhập (email và mật khẩu).</param>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserLoginResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }
            var response = await _userService.LoginAsync(request);
            return Ok(ApiResponse<UserLoginResponse>.SuccessResult(response, "Đăng nhập thành công."));
        }

        /// <summary>
        /// Gán vai trò cho người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="roleId">ID của vai trò.</param>
        [HttpPatch("AssignRole/{userId}/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
        {
            if (userId == Guid.Empty || roleId == Guid.Empty)
                return this.BadRequestForInvalidId();


            var success = await _userService.AssignRoleAsync(userId, roleId);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy người dùng hoặc vai trò."));

            return Ok(ApiResponse<object>.SuccessResult(new { userId, roleId }, "Gán vai trò thành công."));
        }
    }
}

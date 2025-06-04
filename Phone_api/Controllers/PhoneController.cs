using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Extensions;
using Phone_api.Filters;
using Phone_api.Services;

namespace Phone_api.Controllers
{
    /// <summary>
    /// Controller quản lý các thao tác liên quan đến điện thoại.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RequirePermission]
    public class PhoneController : ControllerBase
    {
        private readonly IPhoneService _phoneService;

        /// <summary>
        /// Khởi tạo một instance mới của <see cref="PhoneController"/>.
        /// </summary>
        /// <param name="phoneService">Dịch vụ xử lý logic nghiệp vụ điện thoại.</param>
        public PhoneController(IPhoneService phoneService)
        {
            _phoneService = phoneService;
        }

        /// <summary>
        /// Lấy danh sách điện thoại phân trang với tùy chọn tìm kiếm và sắp xếp.
        /// </summary>
        /// <param name="query">Tham số truy vấn cho phân trang, tìm kiếm và sắp xếp.</param>
        [HttpGet("GetPaged")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PagedResult<PhoneDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetPaged([FromQuery] BaseQuery query)
        {
            if (!ModelState.IsValid)
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var result = await _phoneService.GetPagedAsync(query);
            return Ok(ApiResponse<PagedResult<PhoneDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Lấy thông tin điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại.</param>
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PhoneDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var phone = await _phoneService.GetByIdAsync(id);
            return Ok(ApiResponse<PhoneDto>.SuccessResult(phone));
        }

        /// <summary>
        /// Tạo một điện thoại mới.
        /// </summary>
        /// <param name="request">Yêu cầu chứa thông tin điện thoại cần tạo.</param>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Create([FromBody] CreatePhoneRequest request)
        {
            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            await _phoneService.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = Guid.NewGuid() }, ApiResponse<object>.SuccessResult(request, AppResources.AddPhoneSuccess));
        }

        /// <summary>
        /// Cập nhật thông tin một điện thoại hiện có.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần cập nhật.</param>
        /// <param name="request">Yêu cầu chứa thông tin điện thoại cập nhật.</param>
        [HttpPut("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePhoneRequest request)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            if (!TryValidateModel(request))
            {
                var errors = ControllerBaseExtensions.GetValidationErrors(ModelState);
                return BadRequest(ApiResponse<object>.ErrorResult("Dữ liệu không hợp lệ.", errors));
            }

            var success = await _phoneService.UpdateAsync(id, request);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult(AppResources.PhoneNotFound));

            return Ok(ApiResponse<object>.SuccessResult(id, AppResources.UpdatePhoneSuccess));
        }

        /// <summary>
        /// Xóa một điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần xóa.</param>
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _phoneService.DeleteAsync(id);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult(AppResources.PhoneNotFound));

            return Ok(ApiResponse<object>.SuccessResult(id, AppResources.DeletePhoneSuccess));
        }

        /// <summary>
        /// Duyệt một điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần duyệt.</param>
        [HttpPatch("Approve/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Approve(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _phoneService.Approve(id);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult(AppResources.PhoneNotFound));

            return Ok(ApiResponse<object>.SuccessResult(id, AppResources.ApprovePhoneSuccess));
        }

        /// <summary>
        /// Từ chối một điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần từ chối.</param>
        [HttpPatch("Reject/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<object>))]
        public async Task<IActionResult> Reject(Guid id)
        {
            if (id == Guid.Empty)
                return this.BadRequestForInvalidId();

            var success = await _phoneService.Reject(id);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResult(AppResources.PhoneNotFound));

            return Ok(ApiResponse<object>.SuccessResult(id, AppResources.RejectPhoneSuccess));
        }
    }
}
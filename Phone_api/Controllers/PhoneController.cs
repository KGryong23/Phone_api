using Microsoft.AspNetCore.Mvc;
using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Extensions;
using Phone_api.Services;

namespace Phone_api.Controllers
{
    /// <summary>
    /// Controller quản lý các thao tác liên quan đến điện thoại.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<PhoneDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetPaged([FromQuery] BaseQuery query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequestFromModelState(ModelState);

                var result = await _phoneService.GetPagedAsync(query);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse { Message = AppResources.InternalServerError });
            }
        }

        /// <summary>
        /// Lấy thông tin điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PhoneDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var phone = await _phoneService.GetByIdAsync(id);
                if (phone.Id == Guid.Empty)
                    return NotFound(new ErrorResponse { Message = AppResources.PhoneNotFound });
                return Ok(phone);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }

        /// <summary>
        /// Tạo một điện thoại mới.
        /// </summary>
        /// <param name="request">Yêu cầu chứa thông tin điện thoại cần tạo.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Create([FromBody] CreatePhoneRequest request)
        {
            try
            {
                if (!TryValidateModel(request))
                    return this.BadRequestFromModelState(ModelState);

                var success = await _phoneService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = Guid.NewGuid() }, new SuccessResponse { Message = AppResources.AddPhoneSuccess });
            }
            catch (ArgumentException ex)
            {
                return this.BadRequestFromArgumentException(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin một điện thoại hiện có.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần cập nhật.</param>
        /// <param name="request">Yêu cầu chứa thông tin điện thoại cập nhật.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePhoneRequest request)
        {
            try
            {
                if (!TryValidateModel(request))
                    return this.BadRequestFromModelState(ModelState);

                var success = await _phoneService.UpdateAsync(id, request);
                if (!success)
                    return NotFound(new ErrorResponse { Message = AppResources.PhoneNotFound });

                return Ok(new SuccessResponse { Message = AppResources.UpdatePhoneSuccess });
            }
            catch (ArgumentException ex)
            {
                return this.BadRequestFromArgumentException(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa một điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần xóa.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _phoneService.DeleteAsync(id);
                if (!success)
                    return NotFound(new ErrorResponse { Message = AppResources.PhoneNotFound });
                return Ok(new SuccessResponse { Message = AppResources.DeletePhoneSuccess });
            }
            catch (ArgumentException ex)
            {
                return this.BadRequestFromArgumentException(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }

        /// <summary>
        /// Duyệt một điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần duyệt.</param>
        [HttpPatch("{id}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                var success = await _phoneService.Approve(id);
                if (!success)
                    return NotFound(new ErrorResponse { Message = AppResources.PhoneNotFound });
                return Ok(new SuccessResponse { Message = AppResources.ApprovePhoneSuccess });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }

        /// <summary>
        /// Từ chối một điện thoại theo ID.
        /// </summary>
        /// <param name="id">Định danh duy nhất của điện thoại cần từ chối.</param>
        [HttpPatch("{id}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                var success = await _phoneService.Reject(id);
                if (!success)
                    return NotFound(new ErrorResponse { Message = AppResources.PhoneNotFound });
                return Ok(new SuccessResponse { Message = AppResources.RejectPhoneSuccess });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }
    }

    /// <summary>
    /// Đại diện cho response xấu với thông báo.
    /// </summary>
    public class BadResponse
    {
        /// <summary>
        /// Thông báo thành công.
        /// </summary>
        public Dictionary<string, string[]>? errors { get; set; }
    }

    /// <summary>
    /// Đại diện cho response thành công với thông báo.
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Thông báo thành công.
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// Đại diện cho response lỗi với thông báo.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Thông báo lỗi.
        /// </summary>
        public string? Message { get; set; }
    }
}
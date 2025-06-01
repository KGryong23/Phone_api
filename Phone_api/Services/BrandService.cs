using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Enums;
using Phone_api.Extensions;
using Phone_api.Repositories;

namespace Phone_api.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ cho Brand
    /// </summary>
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        /// <summary>
        /// Lấy danh sách Brand phân trang, tìm kiếm theo Name, sắp xếp theo Name
        /// </summary>
        public async Task<PagedResult<BrandDto>> GetPagedAsync(BaseQuery query)
        {
            var result = await _brandRepository.GetPagedAsync(query, "Name", "Name");
            var brands = result.Data.Select(b => b.ToDto()).ToList();
            return new PagedResult<BrandDto>(brands, result.TotalRecords);
        }

        /// <summary>
        /// Lấy Brand theo ID
        /// </summary>
        public async Task<BrandDto> GetByIdAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand.ToDto() ?? new();
        }

        /// <summary>
        /// Lấy tất cả Brand
        /// </summary>
        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return brands.Select(b => b.ToDto());
        }

        /// <summary>
        /// Thêm Brand mới với kiểm tra đầu vào
        /// </summary>
        public async Task<bool> AddAsync(BrandDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("Tên thương hiệu không thể để trống.", nameof(dto.Name));

            var brand = dto.ToEntity();
            await _brandRepository.AddAsync(brand);
            return await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật Brand với kiểm tra đầu vào
        /// </summary>
        public async Task<bool> UpdateAsync(BrandDto dto)
        {
            if (dto.Id == Guid.Empty)
                throw new ArgumentException("ID không hợp lệ.", nameof(dto.Id));
            if (string.IsNullOrEmpty(dto.Name))
                throw new ArgumentException("Tên thương hiệu không thể để trống.", nameof(dto.Name));

            var brand = await _brandRepository.GetByIdAsync(dto.Id);
            if (brand == null)
                throw new ArgumentException("Thương hiệu không tồn tại.", nameof(dto.Id));

            brand.Name = dto.Name;

            _brandRepository.Update(brand);
            return await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa Brand
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
                throw new ArgumentException("Thương hiệu không tồn tại.", nameof(id));

            _brandRepository.Update(brand);
            return await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Duyệt Brand
        /// </summary>
        public async Task<bool> Approve(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
                return false;

            brand.ModerationStatus = ModerationStatus.Approved;

            _brandRepository.Update(brand);
            return await _brandRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Từ chối Brand
        /// </summary>
        public async Task<bool> Reject(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
                return false;

            brand.ModerationStatus = ModerationStatus.Rejected;

            _brandRepository.Update(brand);
            return await _brandRepository.SaveChangesAsync();
        }
    }
}

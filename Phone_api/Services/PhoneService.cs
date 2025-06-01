using Phone_api.Common;
using Phone_api.Dtos;
using Phone_api.Enums;
using Phone_api.Extensions;
using Phone_api.Repositories;

namespace Phone_api.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ cho Phone
    /// </summary>
    public class PhoneService : IPhoneService
    {
        private readonly IPhoneRepository _phoneRepository;
        private readonly IBrandRepository _brandRepository;

        public PhoneService(IPhoneRepository phoneRepository, IBrandRepository brandRepository)
        {
            _phoneRepository = phoneRepository;
            _brandRepository = brandRepository;
        }

        /// <summary>
        /// Lấy danh sách Phone phân trang, tìm kiếm theo Model, sắp xếp theo Price
        /// </summary>
        public async Task<PagedResult<PhoneDto>> GetPagedAsync(BaseQuery query)
        {
            var result = await _phoneRepository.GetPagedAsync(query, "Model", "Price");
            var phones = result.Data.Select(p => new PhoneDto
            {
                Id = p.Id,
                Model = p.Model,
                Price = p.Price,
                Stock = p.Stock,
                Created = p.Created,
                LastModified = p.LastModified,
                ModerationStatus = p.ModerationStatus,
                ModerationStatusTxt = p.ModerationStatus switch
                {
                    ModerationStatus.Approved => AppConstants.Approved,
                    ModerationStatus.Rejected => AppConstants.Rejected,
                    _ => AppConstants.NotDetermined
                },
                BrandId = p.BrandId,
                BrandName = p.BrandId.HasValue ? (_brandRepository.GetById(p.BrandId.Value))?.Name ?? "N/A" : "N/A"
            }).ToList();
            return new PagedResult<PhoneDto>(phones, result.TotalRecords);
        }

        /// <summary>
        /// Lấy Phone theo ID
        /// </summary>
        public async Task<PhoneDto> GetByIdAsync(Guid id)
        {
            var phone = await _phoneRepository.GetByIdAsync(id);
            return phone.ToDto() ?? new PhoneDto();
        }

        /// <summary>
        /// Thêm Phone mới với kiểm tra đầu vào
        /// </summary>
        public async Task<bool> AddAsync(CreatePhoneRequest request)
        {
            if(request.BrandId.HasValue)
            {
                var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value);
                if (brand is null)
                    throw new ArgumentException(AppResources.InvalidBrand, nameof(request.BrandId));
            }
            
            var existingPhones = await _phoneRepository.FindAllAsync(x => x.Model == request.Model);
            if (existingPhones.Any())
                throw new ArgumentException(AppResources.DuplicateModel, nameof(request.Model));

            return await _phoneRepository.AddPhoneAsync(request.ToEntity());
        }

        /// <summary>
        /// Cập nhật Phone với kiểm tra đầu vào
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, UpdatePhoneRequest request)
        {
            if (request.BrandId.HasValue)
            {
                var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value);
                if (brand is null)
                    throw new ArgumentException(AppResources.InvalidBrand, nameof(request.BrandId));
            }

            var existingPhones = await _phoneRepository.FindAllAsync(x => x.Model == request.Model && x.Id != id);
            if (existingPhones.Any())
                throw new ArgumentException(AppResources.DuplicateModel, nameof(request.Model));

            var phone = await _phoneRepository.GetByIdAsync(id);
            if (phone is null)
                throw new ArgumentException(AppResources.PhoneNotFound, nameof(id));

            phone.Model = request.Model;
            phone.Price = request.Price;
            phone.Stock = request.Stock;
            phone.BrandId = request.BrandId;

            _phoneRepository.Update(phone);
            return await _phoneRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa Phone
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var phone = await _phoneRepository.GetByIdAsync(id);
            if (phone is null)
                throw new ArgumentException(AppResources.PhoneNotFound, nameof(id));

            _phoneRepository.Delete(phone);
            return await _phoneRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Duyệt Phone
        /// </summary>
        public async Task<bool> Approve(Guid id)
        {
            var phone = await _phoneRepository.GetByIdAsync(id);
            if (phone is null)
                return false;

            phone.ModerationStatus = ModerationStatus.Approved;

            _phoneRepository.Update(phone);
            return await _phoneRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Từ chối Phone
        /// </summary>
        public async Task<bool> Reject(Guid id)
        {
            var phone = await _phoneRepository.GetByIdAsync(id);
            if (phone is null)
                return false;

            phone.ModerationStatus = ModerationStatus.Rejected;
            _phoneRepository.Update(phone);

            return await _phoneRepository.SaveChangesAsync();
        }
    }
}

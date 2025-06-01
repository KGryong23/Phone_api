using Phone_api.Common;
using Phone_api.Dtos;

namespace Phone_api.Services
{
    /// <summary>
    /// Interface cho Phone Service
    /// </summary>
    public interface IPhoneService
    {
        Task<PagedResult<PhoneDto>> GetPagedAsync(BaseQuery query);
        Task<PhoneDto> GetByIdAsync(Guid id);
        Task<bool> AddAsync(CreatePhoneRequest request);
        Task<bool> UpdateAsync(Guid id,UpdatePhoneRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> Approve(Guid id);
        Task<bool> Reject(Guid id);
    }
}

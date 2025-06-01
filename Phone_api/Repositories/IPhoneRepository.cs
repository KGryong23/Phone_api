using Phone_api.Dtos;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    /// <summary>
    /// Interface cho Phone Repository
    /// </summary>
    public interface IPhoneRepository : IRepository<Phone>
    {
        Task<bool> AddPhoneAsync(Phone phone);
    }
}

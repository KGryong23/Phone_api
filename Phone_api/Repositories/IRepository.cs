using Phone_api.Common;
using System.Linq.Expressions;

namespace Phone_api.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(Guid id);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        T Update(T entity);
        void Delete(T entity);
        Task<bool> SaveChangesAsync();
        Task<PagedResult<T>> GetPagedAsync(BaseQuery query, string searchField, string sortField);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> exp);
    }
}

using Microsoft.EntityFrameworkCore;
using Phone_api.Common;
using Phone_api.Data;
using Phone_api.Exceptions;
using System.Linq.Expressions;

namespace Phone_api.Repositories
{
    /// <summary>
    /// Generic Repository triển khai các thao tác CRUD và phân trang
    /// </summary>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly PhoneContext _context;
        private readonly DbSet<T> _dbSet;

        protected Repository(PhoneContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Lấy bản ghi theo ID
        /// </summary>
        public T GetById(Guid id) => _dbSet.Find(id) ?? throw new NotFoundException("Bản ghi không tồn tại");

        /// <summary>
        /// Lấy bản ghi theo ID (async)
        /// </summary>
        public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id) ?? throw new NotFoundException("Bản ghi không tồn tại");

        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        /// <summary>
        /// Thêm bản ghi mới
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        public T Update(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Lấy danh sách phân trang, tìm kiếm theo tên, sắp xếp theo trường chỉ định
        /// </summary>
        public async Task<PagedResult<T>> GetPagedAsync(BaseQuery query, string searchField, string sortField)
        {
            var dataQuery = _dbSet.AsQueryable();

            // Tìm kiếm theo tên (Model hoặc Name)
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                dataQuery = dataQuery.Where(x => EF.Property<string>(x, searchField).ToLower().Contains(query.Keyword.ToLower()));
            }

            // Sắp xếp
            dataQuery = dataQuery.OrderByDescending(x => EF.Property<object>(x, sortField));

            // Đếm tổng số bản ghi
            int totalRecords = await dataQuery.CountAsync();

            // Phân trang
            dataQuery = dataQuery.Skip(query.Skip).Take(query.Take);

            var data = await dataQuery.ToListAsync();
            return new PagedResult<T>(data, totalRecords);
        }

        /// <summary>
        /// Hàm lưu các thay đổi vào cơ sở dữ liệu
        /// </summary>
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        /// <summary>
        /// Lấy các bản ghi theo điều kiện
        /// </summary>
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> exp) => await _dbSet.Where(exp).ToListAsync();
    }
}

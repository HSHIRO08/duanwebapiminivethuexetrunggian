using System.Linq.Expressions;

namespace Domain.Interfaces
{
    /// <summary>
    /// Generic Repository Interface
    /// Định nghĩa các operations cơ bản cho tất cả entities
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        // READ Operations
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // CREATE Operation
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // UPDATE Operation
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // DELETE Operation
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // COUNT Operations
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        // EXISTS Operation
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}

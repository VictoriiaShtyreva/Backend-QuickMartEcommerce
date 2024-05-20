using Ecommerce.Core.src.Common;

namespace Ecommerce.Core.src.Interfaces
{
    public interface IBaseRepository<T, QueryOptions> where T : class
    {
        Task<PaginatedResult<T>> GetAllAsync(QueryOptions options);
        Task<T> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T?> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(T entity);
    }
}
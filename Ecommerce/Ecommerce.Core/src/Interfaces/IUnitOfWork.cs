using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Core.src.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
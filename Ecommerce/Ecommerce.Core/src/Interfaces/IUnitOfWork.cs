using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Core.src.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync();
        void Rollback();
        IDbContextTransaction BeginTransaction();
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
    }
}
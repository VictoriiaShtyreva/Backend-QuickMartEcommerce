using Ecommerce.Core.src.Interfaces;
using Ecommerce.WebAPI.src.Data;
using Ecommerce.WebAPI.src.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.WebAPI.src.ExternalService
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        public IProductRepository ProductRepository => new ProductRepository(_context);
        public ICartRepository CartRepository => new CartRepository(_context);

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
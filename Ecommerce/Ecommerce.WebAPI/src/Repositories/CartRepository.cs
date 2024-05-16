using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Cart> _carts;
        public CartRepository(AppDbContext context)
        {
            _context = context;
            _carts = context.Carts;
        }
        public async Task<Cart> CreateAsync(Cart entity)
        {
            _carts.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cart = await _carts.FindAsync(id);
            if (cart == null) return false;
            _carts.Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Cart>> GetAllAsync(QueryOptions options)
        {
            IQueryable<Cart> query = _carts.Include(c => c.CartItems!);
            // Sorting
            query = SortCarts(query, options.SortBy, options.SortOrder);
            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            return await query.ToListAsync();
        }

        private IQueryable<Cart> SortCarts(IQueryable<Cart> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byPrice:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(c => c.CartItems!.Sum(ci => ci.Quantity * ci.Product!.Price)) :
                            query.OrderByDescending(c => c.CartItems!.Sum(ci => ci.Quantity * ci.Product!.Price));
                    break;
                case SortType.byTitle:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(c => c.CartItems!.Select(ci => ci.Product!.Title).FirstOrDefault()) :
                            query.OrderByDescending(c => c.CartItems!.Select(ci => ci.Product!.Title).FirstOrDefault());
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(c => c.CreatedAt) :
                            query.OrderByDescending(c => c.CreatedAt);
                    break;
            }
            return query;
        }

        public async Task<Cart> GetByIdAsync(Guid id)
        {
            return await _carts
               .Include(c => c.CartItems!)
               .ThenInclude(ci => ci.Product)
               .SingleOrDefaultAsync(c => c.Id == id) ?? throw AppException.NotFound();
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            return await _carts
                 .Include(c => c.CartItems!)
                 .FirstOrDefaultAsync(c => c.UserId == userId) ?? throw AppException.NotFound();
        }

        public async Task<Cart?> UpdateAsync(Cart entity)
        {
            _carts.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(Cart entity)
        {
            return await _carts.AnyAsync(e => e.Id == entity.Id);
        }
    }
}
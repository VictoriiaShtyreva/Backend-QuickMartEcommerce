using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class CartItemRepository : IBaseRepository<CartItem, QueryOptions>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<CartItem> _cartItems;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
            _cartItems = context.CartItems;
        }
        public async Task<CartItem> CreateAsync(CartItem entity)
        {
            _cartItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _cartItems.FindAsync(id);
            if (item == null) return false;
            _cartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync(QueryOptions options)
        {
            IQueryable<CartItem> query = _context.CartItems
           .Include(ci => ci.Product);

            // Sorting
            query = SortCartItems(query, options.SortBy, options.SortOrder);
            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);
            return await query.ToListAsync();
        }

        private IQueryable<CartItem> SortCartItems(IQueryable<CartItem> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byTitle:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(ci => ci.Product!.Title) :
                            query.OrderByDescending(ci => ci.Product!.Title);
                    break;
                case SortType.byPrice:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(ci => ci.Product!.Price) :
                            query.OrderByDescending(ci => ci.Product!.Price);
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending ?
                           query.OrderBy(ci => ci.CreatedAt) :
                           query.OrderByDescending(ci => ci.CreatedAt);
                    break;
            }
            return query;
        }

        public async Task<CartItem> GetByIdAsync(Guid id)
        {
            return await _cartItems
               .Include(ci => ci.Product)
               .ThenInclude(p => p!.Images)
               .SingleOrDefaultAsync(ci => ci.Id == id) ?? throw AppException.NotFound();
        }

        public async Task<CartItem?> UpdateAsync(CartItem entity)
        {
            var existingCartItem = await _cartItems.FindAsync(entity.Id) ?? throw AppException.NotFound();
            existingCartItem.Quantity = entity.Quantity;
            _cartItems.Update(existingCartItem);
            await _context.SaveChangesAsync();
            return existingCartItem;
        }

        public async Task<bool> ExistsAsync(CartItem entity)
        {
            return await _cartItems.AnyAsync(e => e.Id == entity.Id);
        }
    }
}
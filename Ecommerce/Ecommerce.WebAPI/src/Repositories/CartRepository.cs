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
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<CartItem> AddProductToCartAsync(Guid userId, Guid productId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Fetch the product and check inventory
                var product = await _context.Products.FindAsync(productId);
                if (product == null || product.Inventory < quantity)
                {
                    throw new InvalidOperationException("Product is unavailable or inventory is insufficient.");
                }
                // Fetch the cart
                var cart = await GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = new Cart(userId);
                    _carts.Add(cart);
                    await _context.SaveChangesAsync();
                }
                // Add item to cart
                var cartItem = cart.CartItems?.FirstOrDefault(i => i.ProductId == productId);
                if (cartItem == null)
                {
                    cartItem = new CartItem(cart.Id, productId, quantity)
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    cart.CartItems?.Add(cartItem);
                }
                else
                {
                    cartItem.AddQuantity(quantity);
                    cartItem.UpdatedAt = DateTime.UtcNow;
                    _context.Entry(cartItem).State = EntityState.Modified;
                }
                // Decrease product inventory
                product.Inventory -= quantity;
                _context.Products.Update(product);
                cart.UpdatedAt = DateTime.UtcNow;
                // Update the cart
                _carts.Update(cart);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                // Return the newly added cart item
                return cartItem!;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Failed to add product to cart due to a concurrency issue.", ex);
            }
        }

        public async Task<bool> ExistsAsync(Cart entity)
        {
            return await _carts.AnyAsync(e => e.Id == entity.Id);
        }
    }
}
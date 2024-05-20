using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class OrderItemRepository : IBaseRepository<OrderItem, QueryOptions>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<OrderItem> _orderItems;
        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
            _orderItems = _context.OrderItems;
        }
        public async Task<OrderItem> CreateAsync(OrderItem entity)
        {
            _orderItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _orderItems.FindAsync(id);
            if (item == null) return false;
            _orderItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<OrderItem>> GetAllAsync(QueryOptions options)
        {
            IQueryable<OrderItem> query = _context.OrderItems.Include(oi => oi.ProductSnapshot);

            // Get total count
            var totalCount = await query.CountAsync();

            // Sorting
            query = SortOrderItems(query, options.SortBy, options.SortOrder);

            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            var orderItems = await query.ToListAsync();

            return new PaginatedResult<OrderItem>(orderItems, totalCount);
        }


        private IQueryable<OrderItem> SortOrderItems(IQueryable<OrderItem> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byPrice:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(oi => oi.Price) :
                            query.OrderByDescending(oi => oi.Price);
                    break;
                case SortType.byTitle:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(oi => oi.ProductSnapshot!.Title) :
                            query.OrderByDescending(oi => oi.ProductSnapshot!.Title);
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(oi => oi.CreatedAt) :
                            query.OrderByDescending(oi => oi.CreatedAt);
                    break;
            }
            return query;
        }


        public async Task<OrderItem> GetByIdAsync(Guid id)
        {
            return await _orderItems.FindAsync(id) ?? throw AppException.NotFound();
        }

        public async Task<OrderItem?> UpdateAsync(OrderItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(OrderItem entity)
        {
            return await _orderItems.AnyAsync(e => e.Id == entity.Id);
        }
    }
}
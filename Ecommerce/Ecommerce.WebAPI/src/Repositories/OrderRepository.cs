using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Order> _orders;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
            _orders = _context.Orders;
        }

        // Interface compliant CreateAsync
        public async Task<Order> CreateAsync(Order entity)
        {
            return await CreateAsync(entity, null!);
        }

        // Overloaded CreateAsync to handle Order with Address
        public async Task<Order> CreateAsync(Order entity, Address address)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (address != null)
                {
                    _context.Addresses.Add(address);
                    await _context.SaveChangesAsync();
                    entity.AddressId = address.Id;
                }
                // Add the new order
                _orders.Add(entity);
                await _context.SaveChangesAsync();
                // Clear the cart after converting to an order
                await transaction.CommitAsync();
                return entity;
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _orders.FindAsync(id);
            if (order == null) return false;
            var orderAddress = await _context.Addresses.FindAsync(order.AddressId);
            if (orderAddress != null) _context.Addresses.Remove(orderAddress);
            _orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<Order>> GetAllAsync(QueryOptions options)
        {
            IQueryable<Order> query = _context.Orders;

            // Get total count
            var totalCount = await query.CountAsync();

            // Sorting
            query = SortOrders(query, options.SortBy, options.SortOrder);

            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);

            var orders = await query.ToListAsync();

            return new PaginatedResult<Order>(orders, totalCount);
        }

        private IQueryable<Order> SortOrders(IQueryable<Order> query, SortType sortBy, SortOrder sortOrder)
        {
            switch (sortBy)
            {
                case SortType.byName:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(o => o.User!.Name) :
                            query.OrderByDescending(o => o.User!.Name);
                    break;
                case SortType.byPrice:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(o => o.TotalPrice) :
                            query.OrderByDescending(o => o.TotalPrice);
                    break;
                case SortType.byDate:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(o => o.CreatedAt) :
                            query.OrderByDescending(o => o.CreatedAt);
                    break;
                default:
                    query = sortOrder == SortOrder.Ascending ?
                            query.OrderBy(o => o.CreatedAt) :
                            query.OrderByDescending(o => o.CreatedAt);
                    break;
            }
            return query;
        }


        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _orders.FindAsync(id) ?? throw AppException.NotFound();
        }

        public async Task<IEnumerable<Order>> GetOrderByUserIdAsync(Guid userId)
        {
            var order = await _orders.Include(o => o.OrderItems!).Where(x => x.UserId == userId).ToListAsync();
            return order;
        }

        public async Task<Order?> UpdateAsync(Order entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _orders.FindAsync(orderId);
            if (order == null) return false;
            order.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Order entity)
        {
            return await _orders.AnyAsync(e => e.Id == entity.Id);
        }

        public async Task<Order?> GetByStripeSessionIdAsync(string sessionId)
        {
            return await _orders.FirstOrDefaultAsync(o => o.StripeSessionId == sessionId);
        }

    }
}
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
        private readonly DbSet<ProductSnapshot> _productSnapshots;
        private readonly DbSet<OrderItem> _orderItems;
        private readonly ICartRepository _cartRepository;
        public OrderRepository(AppDbContext context, ICartRepository cartRepository)
        {
            _context = context;
            _orders = _context.Orders;
            _orderItems = _context.OrderItems;
            _productSnapshots = _context.ProductSnapshots;
            _cartRepository = cartRepository;
        }

        public async Task<Order> CreateAsync(Order entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _orders.FindAsync(id);
            if (order == null) return false;
            _orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetAllAsync(QueryOptions options)
        {
            IQueryable<Order> query = _context.Orders;
            // Sorting
            query = SortOrders(query, options.SortBy, options.SortOrder);
            // Pagination
            query = query.Skip((options.Page - 1) * options.PageSize).Take(options.PageSize);
            return await query.ToListAsync();
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
            var order = await _orders.Include(o => o.OrderItems).ThenInclude(oi => oi.ProductSnapshot).Where(x => x.UserId == userId).ToListAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order entity)
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
    }
}
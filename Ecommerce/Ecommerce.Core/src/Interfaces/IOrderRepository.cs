using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order, QueryOptions>
    {
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<IEnumerable<Order>> GetOrderByUserIdAsync(Guid userId);
        Task<Order?> GetByStripeSessionIdAsync(string sessionId);
    }
}
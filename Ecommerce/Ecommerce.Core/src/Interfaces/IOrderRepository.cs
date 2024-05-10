using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order, QueryOptions>
    {
        // Update the status of an order
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);

        Task<IEnumerable<Order>> GetOrderByUserIdAsync(Guid userId);

    }
}
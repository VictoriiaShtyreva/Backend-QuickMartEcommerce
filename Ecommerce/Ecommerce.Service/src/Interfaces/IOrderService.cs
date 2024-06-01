using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface IOrderService : IBaseService<OrderReadDto, OrderCreateDto, OrderUpdateDto, QueryOptions>
    {
        Task<IEnumerable<OrderReadDto>> GetOrdersByUserIdAsync(Guid userId);
        Task<bool> CancelOrderAsync(Guid orderId);
        Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderCreateDto);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<bool> UpdateOrderAsync(Guid orderId, OrderUpdateDto orderUpdateDto);
        Task MarkOrderAsPaid(string sessionId);
        Task MarkOrderAsFailed(string sessionId);
    }
}
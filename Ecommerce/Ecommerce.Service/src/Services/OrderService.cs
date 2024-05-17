using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class OrderService : BaseService<Order, OrderReadDto, OrderCreateDto, OrderUpdateDto, QueryOptions>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBaseRepository<Address, QueryOptions> _addressRepository;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, IBaseRepository<Address, QueryOptions> addressRepository, IProductRepository productRepository, IMapper mapper, IMemoryCache cache)
            : base(orderRepository, mapper, cache)
        {
            _orderRepository = orderRepository;
            _addressRepository = addressRepository;
            _productRepository = productRepository;
        }
        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;
            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Shipped)
            {
                throw new InvalidOperationException("Cannot cancel a completed or shipped order.");
            }
            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderCreateDto)
        {
            var shippingAddress = _mapper.Map<Address>(orderCreateDto.ShippingAddress) ?? throw new InvalidOperationException("Shipping address must be provided.");
            var savedAddress = await _addressRepository.CreateAsync(shippingAddress);

            var order = new Order(orderCreateDto.UserId, savedAddress.Id)
            {
                ShippingAddress = savedAddress
            };
            foreach (var item in orderCreateDto.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId) ?? throw new InvalidOperationException("Product not found.");

                if (product.Inventory < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient inventory for product: {product.Title}");
                }

                product.Inventory -= item.Quantity;
                await _productRepository.UpdateAsync(product);

                var productSnapshot = product.CreateSnapshot();
                order.AddOrderItem(productSnapshot, item.Quantity);
            }
            var createdOrder = await _orderRepository.CreateAsync(order);
            return _mapper.Map<OrderReadDto>(createdOrder);
        }

        public async Task<IEnumerable<OrderReadDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrderByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId) ?? throw AppException.NotFound();
            if (order.Status == newStatus) return false;
            order.Status = newStatus;
            await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
            return true;
        }
    }

}
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
        private readonly ICartRepository _cartRepository;
        private readonly IBaseRepository<Address, QueryOptions> _addressRepository;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IBaseRepository<Address, QueryOptions> addressRepository, IProductRepository productRepository, IMapper mapper, IMemoryCache cache)
            : base(orderRepository, mapper, cache)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
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

        public async Task<OrderReadDto> CreateOrderFromCartAsync(OrderCreateDto orderCreateDto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(orderCreateDto.UserId);
            if (cart == null || cart.CartItems!.Count == 0)
                throw new InvalidOperationException("Cart is empty or not found.");

            var shippingAddress = _mapper.Map<Address>(orderCreateDto.ShippingAddress) ?? throw new InvalidOperationException("Shipping address must be provided.");
            // Save the address first
            var savedAddress = await _addressRepository.CreateAsync(shippingAddress);
            // Create the order with the address
            var order = new Order(orderCreateDto.UserId)
            {
                ShippingAddress = savedAddress,
                AddressId = savedAddress.Id
            };
            foreach (var item in cart.CartItems!)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId) ?? throw new InvalidOperationException("Product details could not be found or rehydrated for order creation.");
                order.AddOrderItem(product, item.Quantity);
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
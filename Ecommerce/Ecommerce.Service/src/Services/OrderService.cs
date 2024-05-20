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
        private readonly IProductImageRepository _productImageRepository;
        public OrderService(IOrderRepository orderRepository, IBaseRepository<Address, QueryOptions> addressRepository, IProductRepository productRepository, IProductImageRepository productImageRepository, IMapper mapper, IMemoryCache cache)
            : base(orderRepository, mapper, cache)
        {
            _orderRepository = orderRepository;
            _addressRepository = addressRepository;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
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
                // Fetch product images
                var productSnapshot = await product.CreateSnapshotAsync(_productImageRepository);
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
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;
            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> UpdateOrderAsync(Guid orderId, OrderUpdateDto orderUpdateDto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            if (orderUpdateDto.Status.HasValue && order.Status != orderUpdateDto.Status.Value)
            {
                order.Status = orderUpdateDto.Status.Value;
            }

            if (orderUpdateDto.OrderItems != null)
            {
                foreach (var itemUpdate in orderUpdateDto.OrderItems)
                {
                    var existingItem = order.OrderItems!.FirstOrDefault(oi => oi.Id == itemUpdate.ItemId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity = itemUpdate.Quantity.HasValue ? itemUpdate.Quantity.Value : existingItem.Quantity;
                    }
                    else
                    {
                        var product = await _productRepository.GetByIdAsync(itemUpdate.ItemId);
                        if (product == null) continue;
                        var productSnapshot = await product.CreateSnapshotAsync(_productImageRepository);
                        order.AddOrderItem(productSnapshot, itemUpdate.Quantity.GetValueOrDefault());
                    }
                }
            }

            if (orderUpdateDto.ShippingAddress != null)
            {
                var newAddress = _mapper.Map<Address>(orderUpdateDto.ShippingAddress);
                var savedAddress = await _addressRepository.CreateAsync(newAddress);
                order.AddressId = savedAddress.Id;
                order.ShippingAddress = savedAddress;
            }

            await _orderRepository.UpdateAsync(order);
            return true;
        }
    }
}
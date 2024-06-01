using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Ecommerce.Service.src.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository = new Mock<IOrderRepository>();
        private readonly Mock<IBaseRepository<Address, QueryOptions>> _mockAddressRepository = new Mock<IBaseRepository<Address, QueryOptions>>();
        private readonly Mock<IProductRepository> _mockProductRepository = new Mock<IProductRepository>();
        private readonly Mock<IProductImageRepository> _mockProductImageRepository = new Mock<IProductImageRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();
        private readonly Mock<IStripeService> _mockStripeService = new Mock<IStripeService>();
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _service = new OrderService(_mockOrderRepository.Object, _mockAddressRepository.Object, _mockProductRepository.Object, _mockProductImageRepository.Object, _mockMapper.Object, _mockCache.Object, _mockStripeService.Object);
        }

        [Theory]
        [InlineData(OrderStatus.Completed, false)]
        [InlineData(OrderStatus.Shipped, false)]
        [InlineData(OrderStatus.Pending, true)]
        public async Task CancelOrderAsync_TestVariousStatuses(OrderStatus initialStatus, bool expectedResult)
        {
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, Status = initialStatus };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);

            if (initialStatus == OrderStatus.Completed || initialStatus == OrderStatus.Shipped)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelOrderAsync(orderId));
            }
            else
            {
                bool result = await _service.CancelOrderAsync(orderId);
                Assert.Equal(expectedResult, result);
                Assert.Equal(OrderStatus.Cancelled, order.Status);
            }
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsWhenShippingAddressIsInvalid()
        {
            var orderCreateDto = new OrderCreateDto { UserId = Guid.NewGuid(), ShippingAddress = null! };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(orderCreateDto));
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsWhenProductNotFound()
        {
            var orderCreateDto = new OrderCreateDto
            {
                UserId = Guid.NewGuid(),
                ShippingAddress = new AddressCreateDto { AddressLine = "123 Main St", City = "Testville", PostalCode = "12345", Country = "Testland" },
                OrderItems = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };

            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product)null!);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(orderCreateDto));
        }

        [Fact]
        public async Task CreateOrderAsync_CreatesOrderSuccessfully()
        {
            // Arrange
            var orderCreateDto = new OrderCreateDto
            {
                UserId = Guid.NewGuid(),
                ShippingAddress = new AddressCreateDto { AddressLine = "123 Main St", City = "Testville", PostalCode = "12345", Country = "Testland" },
                OrderItems = new List<OrderItemCreateDto>
        {
            new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 1 },
            new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 2 }
        }
            };

            var products = orderCreateDto.OrderItems.Select(item => new Product
            {
                Id = item.ProductId,
                Inventory = 10,
                Title = "Test Product",
                Description = "Test Description",
                Price = 100.00m,
                Images = new List<ProductImage> { new ProductImage { Url = "http://example.com/image.jpg" } }
            }).ToList();

            var address = new Address { Id = Guid.NewGuid() };

            foreach (var product in products)
            {
                _mockProductRepository.Setup(p => p.GetByIdAsync(product.Id)).ReturnsAsync(product);
                _mockProductRepository.Setup(p => p.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(product);
                _mockProductImageRepository.Setup(p => p.GetByIdAsync(product.Id)).ReturnsAsync(product.Images!.First());
            }

            _mockAddressRepository.Setup(x => x.CreateAsync(It.IsAny<Address>())).ReturnsAsync(address);
            _mockOrderRepository.Setup(x => x.CreateAsync(It.IsAny<Order>())).ReturnsAsync(new Order { UserId = orderCreateDto.UserId, StripeSessionId = "test_session_id" });
            _mockStripeService.Setup(x => x.CreateCheckoutSession(It.IsAny<decimal>(), It.IsAny<string>())).ReturnsAsync("https://example.com/checkout/test_session_id");

            _mockMapper.Setup(m => m.Map<Address>(It.IsAny<AddressCreateDto>())).Returns(address);
            _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>())).Returns(new OrderReadDto
            {
                UserId = orderCreateDto.UserId,
                ShippingAddress = new AddressReadDto { AddressLine = "123 Main St", City = "Testville", PostalCode = "12345", Country = "Testland" },
                OrderItems = orderCreateDto.OrderItems.Select(i => new OrderItemReadDto
                {
                    ProductSnapshot = new ProductSnapshotDto
                    {
                        ProductId = i.ProductId,
                        Title = products.First(p => p.Id == i.ProductId).Title,
                        Description = products.First(p => p.Id == i.ProductId).Description,
                        Price = products.First(p => p.Id == i.ProductId).Price,
                    },
                    Quantity = i.Quantity
                }).ToList(),
                CheckoutUrl = "https://example.com/checkout/test_session_id",
                StripeSessionId = "test_session_id"
            });

            // Act
            var result = await _service.CreateOrderAsync(orderCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderCreateDto.UserId, result.UserId);
            Assert.Equal("https://example.com/checkout/test_session_id", result.CheckoutUrl);
            Assert.Equal("test_session_id", result.StripeSessionId);
            Assert.Equal(orderCreateDto.OrderItems.Count, result.OrderItems.Count);
        }


        [Fact]
        public async Task GetOrdersByUserIdAsync_ReturnsOrders()
        {
            var userId = Guid.NewGuid();
            var orders = new List<Order> { new Order(), new Order() };

            _mockOrderRepository.Setup(x => x.GetOrderByUserIdAsync(userId)).ReturnsAsync(orders);
            _mockMapper.Setup(m => m.Map<IEnumerable<OrderReadDto>>(orders)).Returns(orders.Select(o => new OrderReadDto()).ToList());

            var result = await _service.GetOrdersByUserIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Theory]
        [InlineData(OrderStatus.Completed, OrderStatus.Shipped, true)]
        [InlineData(OrderStatus.Shipped, OrderStatus.Shipped, false)]
        public async Task UpdateOrderStatusAsync_UpdatesStatusCorrectly(OrderStatus originalStatus, OrderStatus newStatus, bool shouldUpdate)
        {
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, Status = originalStatus };

            _mockOrderRepository.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mockOrderRepository.Setup(x => x.UpdateOrderStatusAsync(orderId, It.IsAny<OrderStatus>())).ReturnsAsync(shouldUpdate);

            var result = await _service.UpdateOrderStatusAsync(orderId, newStatus);

            Assert.Equal(shouldUpdate, result);
            if (shouldUpdate)
                Assert.Equal(newStatus, order.Status);
        }


        [Fact]
        public async Task CreateOneAsync_ShouldReturnCreatedOrderReadDto()
        {
            var createDto = new OrderCreateDto { UserId = Guid.NewGuid() };
            var createdOrder = new Order { Id = Guid.NewGuid(), UserId = createDto.UserId };

            _mockMapper.Setup(m => m.Map<Order>(It.IsAny<OrderCreateDto>())).Returns(createdOrder);
            _mockOrderRepository.Setup(r => r.ExistsAsync(It.IsAny<Order>())).ReturnsAsync(false);
            _mockOrderRepository.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);
            _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>())).Returns(new OrderReadDto { Id = createdOrder.Id, UserId = createdOrder.UserId, TotalPrice = createdOrder.TotalPrice });

            var result = await _service.CreateOneAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(createDto.UserId, result.UserId);
        }

        [Fact]
        public async Task DeleteOneAsync_ShouldReturnTrueWhenSuccess()
        {
            var orderId = Guid.NewGuid();

            _mockOrderRepository.Setup(r => r.DeleteAsync(orderId)).ReturnsAsync(true);

            var result = await _service.DeleteOneAsync(orderId);

            Assert.True(result);
        }

        [Fact]
        public async Task GetOneByIdAsync_ShouldReturnOrderWhenExists()
        {
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId, UserId = Guid.NewGuid(), TotalPrice = 200 };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mockMapper.Setup(m => m.Map<OrderReadDto>(order)).Returns(new OrderReadDto { Id = order.Id, UserId = order.UserId });

            var result = await _service.GetOneByIdAsync(orderId);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        [Fact]
        public async Task UpdateOneAsync_ShouldUpdateAndReturnUpdatedDto()
        {
            var orderId = Guid.NewGuid();
            var updateDto = new OrderUpdateDto { Status = OrderStatus.Completed };
            var existingOrder = new Order { Id = orderId, Status = OrderStatus.Processing };

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
            _mockOrderRepository.Setup(r => r.ExistsAsync(existingOrder)).ReturnsAsync(false);
            _mockOrderRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(new Order { Id = orderId, Status = (OrderStatus)updateDto.Status });
            _mockMapper.Setup(m => m.Map<OrderUpdateDto, Order>(updateDto, existingOrder)).Callback((OrderUpdateDto src, Order dest) => dest.Status = (OrderStatus)src.Status!);
            _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>())).Returns(new OrderReadDto { Id = orderId, Status = (OrderStatus)updateDto.Status });

            var result = await _service.UpdateOneAsync(orderId, updateDto);

            Assert.NotNull(result);
            Assert.Equal(updateDto.Status, result.Status);
        }

    }
}
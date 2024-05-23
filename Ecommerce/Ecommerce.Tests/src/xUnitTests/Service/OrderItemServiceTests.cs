using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class OrderItemServiceTests
    {
        private readonly Mock<IBaseRepository<OrderItem, QueryOptions>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly OrderItemService _service;
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();

        public OrderItemServiceTests()
        {
            _mockRepository = new Mock<IBaseRepository<OrderItem, QueryOptions>>();
            _mockMapper = new Mock<IMapper>();
            _service = new OrderItemService(_mockRepository.Object, _mockMapper.Object, _mockCache.Object);
        }

        [Fact]
        public async Task CreateOneAsync_ShouldCreateOrderItem()
        {
            var orderItemCreateDto = new OrderItemCreateDto();
            var orderItem = new OrderItem();

            _mockMapper.Setup(m => m.Map<OrderItem>(orderItemCreateDto)).Returns(orderItem);
            _mockRepository.Setup(r => r.ExistsAsync(orderItem)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.CreateAsync(orderItem)).ReturnsAsync(orderItem);
            _mockMapper.Setup(m => m.Map<OrderItemReadDto>(orderItem)).Returns(new OrderItemReadDto());

            var result = await _service.CreateOneAsync(orderItemCreateDto);

            Assert.IsType<OrderItemReadDto>(result);
            _mockRepository.Verify(r => r.CreateAsync(orderItem), Times.Once);
        }

        [Fact]
        public async Task GetOneByIdAsync_ReturnsOrderItem()
        {
            var orderItemId = Guid.NewGuid();
            var orderItem = new OrderItem();
            var orderItemReadDto = new OrderItemReadDto();

            _mockRepository.Setup(r => r.GetByIdAsync(orderItemId)).ReturnsAsync(orderItem);
            _mockMapper.Setup(m => m.Map<OrderItemReadDto>(orderItem)).Returns(orderItemReadDto);

            var result = await _service.GetOneByIdAsync(orderItemId);
            Assert.Equal(orderItemReadDto, result);
        }

        [Fact]
        public async Task UpdateOneAsync_ShouldUpdateOrderItem()
        {
            var orderItemId = Guid.NewGuid();
            var orderItemUpdateDto = new OrderItemUpdateDto();
            var orderItem = new OrderItem();

            _mockRepository.Setup(r => r.GetByIdAsync(orderItemId)).ReturnsAsync(orderItem);
            _mockMapper.Setup(m => m.Map(orderItemUpdateDto, orderItem)).Returns(orderItem);
            _mockRepository.Setup(r => r.UpdateAsync(orderItem)).ReturnsAsync(orderItem);
            _mockMapper.Setup(m => m.Map<OrderItemReadDto>(orderItem)).Returns(new OrderItemReadDto());

            var result = await _service.UpdateOneAsync(orderItemId, orderItemUpdateDto);

            Assert.IsType<OrderItemReadDto>(result);
            _mockRepository.Verify(r => r.UpdateAsync(orderItem), Times.Once);
        }

        [Fact]
        public async Task DeleteOneAsync_ShouldReturnTrue()
        {
            var orderItemId = Guid.NewGuid();

            _mockRepository.Setup(r => r.DeleteAsync(orderItemId)).ReturnsAsync(true);

            var result = await _service.DeleteOneAsync(orderItemId);

            Assert.True(result);
        }
    }
}
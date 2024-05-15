using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class CartItemServiceTests
    {
        private readonly Mock<IBaseRepository<CartItem, QueryOptions>> _mockRepository = new Mock<IBaseRepository<CartItem, QueryOptions>>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly CartItemService _service;

        public CartItemServiceTests()
        {
            _service = new CartItemService(_mockRepository.Object, _mockMapper.Object);
        }

        [Theory]
        [InlineData(1, true)]  // Valid scenario
        [InlineData(0, false)] // Invalid scenario: Quantity is zero
        [InlineData(-1, false)]// Invalid scenario: Quantity is negative
        public async Task AddQuantityAsync_VariousScenarios(int quantity, bool shouldSucceed)
        {
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cartItem = new CartItem { Id = cartItemId, ProductId = productId, Quantity = 5 };

            _mockRepository.Setup(x => x.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);

            if (shouldSucceed)
            {
                _mockRepository.Setup(x => x.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);
                _mockMapper.Setup(x => x.Map<CartItemReadDto>(It.IsAny<CartItem>())).Returns(new CartItemReadDto());

                var result = await _service.AddQuantityAsync(cartItemId, productId, quantity);

                Assert.NotNull(result);
                Assert.Equal(5 + quantity, cartItem.Quantity);
            }
            else
            {
                await Assert.ThrowsAsync<ArgumentException>(() => _service.AddQuantityAsync(cartItemId, productId, quantity));
            }
        }

        [Theory]
        [InlineData(1, true)]  // Valid scenario
        [InlineData(0, false)] // Invalid scenario: Quantity is zero
        [InlineData(-1, false)]// Invalid scenario: Quantity is negative
        public async Task ReduceQuantityAsync_VariousScenarios(int quantity, bool shouldSucceed)
        {
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cartItem = new CartItem { Id = cartItemId, ProductId = productId, Quantity = 5 };

            _mockRepository.Setup(x => x.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);

            if (shouldSucceed)
            {
                _mockRepository.Setup(x => x.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);
                _mockMapper.Setup(x => x.Map<CartItemReadDto>(It.IsAny<CartItem>())).Returns(new CartItemReadDto());

                var result = await _service.ReduceQuantityAsync(cartItemId, productId, quantity);

                Assert.NotNull(result);
                Assert.Equal(5 - quantity, cartItem.Quantity);
            }
            else
            {
                await Assert.ThrowsAsync<ArgumentException>(() => _service.ReduceQuantityAsync(cartItemId, productId, quantity));
            }
        }
    }
}

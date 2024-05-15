using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class CartServiceTests
    {
        private readonly CartService _cartService;
        private readonly Mock<ICartRepository> _mockCartRepository = new Mock<ICartRepository>();
        private readonly Mock<IProductRepository> _mockProductRepository = new Mock<IProductRepository>();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();

        public CartServiceTests()
        {
            _cartService = new CartService(_mockCartRepository.Object, _mockMapper.Object, _mockUnitOfWork.Object, _mockProductRepository.Object);
        }

        [Theory]
        [InlineData(5)]
        public async Task AddProductToCartAsync_ShouldHandleQuantityCorrectly(int quantity)
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Inventory = 5 };

            _mockProductRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockCartRepository.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(new Cart(userId));

            if (quantity <= product.Inventory)
            {
                await _cartService.AddProductToCartAsync(userId, productId, quantity);
                Assert.True(product.Inventory == 5 - quantity);
            }
            else
            {
                await Assert.ThrowsAsync<InvalidOperationException>(() => _cartService.AddProductToCartAsync(userId, productId, quantity));
            }
        }

        [Fact]
        public async Task ClearCartAsync_ShouldClearCart_WhenCartExists()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = new Cart(Guid.NewGuid());
            _mockCartRepository.Setup(x => x.GetByIdAsync(cartId)).ReturnsAsync(cart);

            // Act
            var result = await _cartService.ClearCartAsync(cartId);

            // Assert
            Assert.True(result);
            Assert.Empty(cart.CartItems!);
            _mockCartRepository.Verify(x => x.UpdateAsync(cart), Times.Once);
        }

        [Fact]
        public async Task ClearCartAsync_ShouldThrow_WhenCartNotFound()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCartRepository.Setup(x => x.GetByIdAsync(cartId)).ReturnsAsync((Cart)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _cartService.ClearCartAsync(cartId));
        }

        [Fact]
        public async Task GetCartByUserIdAsync_ShouldReturnCart()
        {
            var userId = Guid.NewGuid();
            var cart = new Cart { UserId = userId };

            _mockCartRepository.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
            _mockMapper.Setup(m => m.Map<CartReadDto>(It.IsAny<Cart>())).Returns(new CartReadDto());

            var result = await _cartService.GetCartByUserIdAsync(userId);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCartByUserIdAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockCartRepository.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync((Cart)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _cartService.GetCartByUserIdAsync(userId));
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_ShouldThrow_WhenItemNotFound()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var cart = new Cart(Guid.NewGuid());
            _mockCartRepository.Setup(x => x.GetByIdAsync(cartId)).ReturnsAsync(cart);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cartService.RemoveItemFromCartAsync(cartId, itemId));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeleteCartAsync_ShouldReturnCorrectResult_WhenCartExists(bool expectedResult)
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = new Cart(cartId);
            _mockCartRepository.Setup(x => x.GetByIdAsync(cartId)).ReturnsAsync(cart);
            _mockCartRepository.Setup(x => x.DeleteAsync(cartId)).ReturnsAsync(expectedResult);

            // Act
            var result = await _cartService.DeleteOneAsync(cartId);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockCartRepository.Verify(x => x.DeleteAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task DeleteCartAsync_ShouldReturnFalse_WhenDeletionFails()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCartRepository.Setup(r => r.DeleteAsync(cartId)).ReturnsAsync(false);

            // Act
            var result = await _cartService.DeleteOneAsync(cartId);

            // Assert
            Assert.False(result);
            _mockCartRepository.Verify(r => r.DeleteAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task UpdateCartAsync_ThrowsKeyNotFoundException_WhenCartNotFound()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _mockCartRepository.Setup(r => r.GetByIdAsync(cartId)).ReturnsAsync((Cart)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _cartService.UpdateOneAsync(cartId, new CartUpdateDto()));
        }
    }
}
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class CartServiceTests
    {
        private readonly CartService _cartService;
        private readonly Mock<ICartRepository> _mockCartRepository = new Mock<ICartRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();

        public CartServiceTests()
        {
            _cartService = new CartService(_mockCartRepository.Object, _mockMapper.Object, _mockCache.Object);
        }

        [Theory]
        [InlineData("d0b2e9c7-83f4-4f7e-8b2b-3fabc4d56d1e", "62f8d962-7d6b-4c18-b742-1b3c47d6d49f", 2)]
        public async Task AddProductToCartAsync_ShouldAddProductAndInvalidateCache(Guid userId, Guid productId, int quantity)
        {
            // Arrange
            var cartItem = new CartItem(Guid.NewGuid(), productId, quantity);
            _mockCartRepository.Setup(repo => repo.AddProductToCartAsync(userId, productId, quantity)).ReturnsAsync(cartItem);
            _mockCache.Setup(cache => cache.Remove(It.IsAny<object>()));

            // Act
            var result = await _cartService.AddProductToCartAsync(userId, productId, quantity);

            // Assert
            Assert.Equal(cartItem, result);
            _mockCache.Verify(cache => cache.Remove($"Cart-{userId}"), Times.Once);
            _mockCartRepository.Verify(repo => repo.AddProductToCartAsync(userId, productId, quantity), Times.Once);
        }

        [Theory]
        [InlineData("d0b2e9c7-83f4-4f7e-8b2b-3fabc4d56d1e")]
        public async Task ClearCartAsync_ShouldClearCartAndInvalidateCache(Guid cartId)
        {
            // Arrange
            var cart = new Cart(cartId);
            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId)).ReturnsAsync(cart);
            _mockCache.Setup(cache => cache.Remove(It.IsAny<object>()));

            // Act
            var result = await _cartService.ClearCartAsync(cartId);

            // Assert
            Assert.True(result);
            _mockCartRepository.Verify(repo => repo.GetByIdAsync(cartId), Times.Once);
            _mockCartRepository.Verify(repo => repo.UpdateAsync(cart), Times.Once);
            _mockCache.Verify(cache => cache.Remove($"Cart-{cart.UserId}"), Times.Once);
        }

        [Theory]
        [InlineData("d0b2e9c7-83f4-4f7e-8b2b-3fabc4d56d1e", "d0b2e9c7-83f4-4f7e-8b2b-3fabc4d56d1e", 2)]
        public async Task RemoveItemFromCartAsync_ShouldRemoveItemAndInvalidateCache(Guid cartId, Guid productId, int quantity)
        {
            // Arrange
            var cartItem = new CartItem(cartId, productId, quantity);
            var cart = new Cart(cartId);
            cart.AddProduct(new Product { Id = productId }, quantity);
            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId)).ReturnsAsync(cart);
            _mockCache.Setup(cache => cache.Remove(It.IsAny<object>()));

            // Act
            var result = await _cartService.RemoveItemFromCartAsync(cartId, cartItem);

            // Assert
            Assert.True(result);
            _mockCartRepository.Verify(repo => repo.GetByIdAsync(cartId), Times.Once);
            _mockCartRepository.Verify(repo => repo.UpdateAsync(cart), Times.Once);
            _mockCache.Verify(cache => cache.Remove($"Cart-{cart.UserId}"), Times.Once);
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
        public async Task GetCartByUserIdAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockCartRepository.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync((Cart)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _cartService.GetCartByUserIdAsync(userId));
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
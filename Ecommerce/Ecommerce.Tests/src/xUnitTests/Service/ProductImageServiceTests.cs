using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class ProductImageServiceTests
    {
        private readonly ProductImageService _productImageService;
        private readonly Mock<IProductImageRepository> _mockProductImageRepository = new Mock<IProductImageRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();

        public ProductImageServiceTests()
        {
            _productImageService = new ProductImageService(_mockProductImageRepository.Object, _mockMapper.Object, _mockCache.Object);
        }

        [Fact]
        public async Task CreateOneAsync_CreatesAndReturnsProductImage_WhenNoDuplicateExists()
        {
            // Arrange
            var productImageCreateDto = new ProductImageCreateDto { Url = "http://example.com/image.jpg" };
            var productImage = new ProductImage { Id = Guid.NewGuid(), Url = "http://example.com/image.jpg" };
            var productImageReadDto = new ProductImageReadDto { Url = "http://example.com/image.jpg" };

            _mockMapper.Setup(m => m.Map<ProductImage>(productImageCreateDto)).Returns(productImage);
            _mockProductImageRepository.Setup(r => r.ExistsAsync(It.IsAny<ProductImage>())).ReturnsAsync(false);
            _mockProductImageRepository.Setup(r => r.CreateAsync(It.IsAny<ProductImage>())).ReturnsAsync(productImage);
            _mockMapper.Setup(m => m.Map<ProductImageReadDto>(productImage)).Returns(productImageReadDto);

            // Act
            var result = await _productImageService.CreateOneAsync(productImageCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("http://example.com/image.jpg", result.Url);
        }

        [Fact]
        public async Task DeleteOneAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductImageRepository.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _productImageService.DeleteOneAsync(productId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetOneByIdAsync_ReturnsProductImage_WhenFound()
        {
            // Arrange
            var productImageId = Guid.NewGuid();
            var productImage = new ProductImage { Id = productImageId, Url = "http://example.com/image.jpg" };
            var productImageReadDto = new ProductImageReadDto { Url = "http://example.com/image.jpg" };

            _mockProductImageRepository.Setup(r => r.GetByIdAsync(productImageId)).ReturnsAsync(productImage);
            _mockMapper.Setup(m => m.Map<ProductImageReadDto>(productImage)).Returns(productImageReadDto);

            // Act
            var result = await _productImageService.GetOneByIdAsync(productImageId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("http://example.com/image.jpg", result.Url);
        }

        [Fact]
        public async Task GetOneByIdAsync_ThrowsNotFoundException_WhenProductImageNotFound()
        {
            // Arrange
            var productImageId = Guid.NewGuid();
            _mockProductImageRepository.Setup(r => r.GetByIdAsync(productImageId)).ReturnsAsync((ProductImage)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _productImageService.GetOneByIdAsync(productImageId));
        }

        [Fact]
        public async Task UpdateOneAsync_UpdatesAndReturnsUpdatedProductImage_WhenSuccessful()
        {
            // Arrange
            var productImageId = Guid.NewGuid();
            var existingProductImage = new ProductImage { Id = productImageId, Url = "http://example.com/old_image.jpg" };
            var updateDto = new ProductImageUpdateDto { Url = "http://example.com/new_image.jpg" };
            var updatedProductImage = new ProductImage { Id = productImageId, Url = "http://example.com/new_image.jpg" };
            var updatedProductImageReadDto = new ProductImageReadDto { Url = "http://example.com/new_image.jpg" };

            _mockProductImageRepository.Setup(r => r.GetByIdAsync(productImageId)).ReturnsAsync(existingProductImage);
            _mockMapper.Setup(m => m.Map(updateDto, existingProductImage)).Returns(updatedProductImage);
            _mockProductImageRepository.Setup(r => r.UpdateAsync(updatedProductImage)).ReturnsAsync(updatedProductImage);
            _mockMapper.Setup(m => m.Map<ProductImageReadDto>(updatedProductImage)).Returns(updatedProductImageReadDto);

            // Act
            var result = await _productImageService.UpdateOneAsync(productImageId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("http://example.com/new_image.jpg", result.Url);
        }

        [Fact]
        public async Task UpdateOneAsync_ThrowsNotFoundException_WhenProductImageNotFound()
        {
            // Arrange
            var productImageId = Guid.NewGuid();
            var updateDto = new ProductImageUpdateDto { Url = "http://example.com/new_image.jpg" };

            _mockProductImageRepository.Setup(r => r.GetByIdAsync(productImageId)).ReturnsAsync((ProductImage)null!);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _productImageService.UpdateOneAsync(productImageId, updateDto));
        }
    }
}
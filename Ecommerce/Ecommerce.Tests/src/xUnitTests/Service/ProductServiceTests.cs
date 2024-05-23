using System.Net;
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Ecommerce.Service.src.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ecommerce.Tests.src.xUnitTests.Service
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _mockProductRepository = new Mock<IProductRepository>();
        private readonly Mock<IProductImageRepository> _mockProductImageRepository = new Mock<IProductImageRepository>();
        private readonly Mock<IMapper> _mockMapper = new Mock<IMapper>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();
        private readonly Mock<ICloudinaryImageService> _mockCloudinaryImageService = new Mock<ICloudinaryImageService>();

        public ProductServiceTests()
        {
            _productService = new ProductService(_mockProductRepository.Object, _mockMapper.Object, _mockProductImageRepository.Object, _mockCache.Object, _mockCloudinaryImageService.Object);
        }

        public static IEnumerable<object[]> UpdateProductDetailsData =>
           new List<object[]>
           {
                new object[] { null!, null!, null! },
                new object[] { "Updated Title", null!, null! },
                new object[] { null!, 99.99m, null! },
                new object[] { null!, null!, "Updated Description" }
           };

        [Theory]
        [MemberData(nameof(UpdateProductDetailsData))]
        public async Task UpdateProductDetailsAsync_UpdatesValues_WhenProductExists(string title, decimal? price, string description)
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Title = "Original Title", Price = 50.00M, Description = "Original Description" };
            _mockProductRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductReadDto>(It.IsAny<Product>())).Returns(new ProductReadDto { Title = title ?? product.Title });
            // Act
            var updateDto = new ProductUpdateDto { Title = title, Price = price, Description = description };
            var result = await _productService.UpdateOneAsync(productId, updateDto);
            // Assert
            Assert.Equal(title ?? "Original Title", result.Title); // Default to original if no update provided
        }


        [Fact]
        public async Task GetMostPurchased_ThrowsAppException_WhenNoProductsFound()
        {
            var topNumber = 5;
            _mockProductRepository.Setup(x => x.GetMostPurchasedProductsAsync(topNumber)).ReturnsAsync(new List<Product>());

            await Assert.ThrowsAsync<AppException>(() => _productService.GetMostPurchased(topNumber));
        }

        [Theory]
        [InlineData(true, true, null, null)] // Test case for successful deletion
        [InlineData(false, false, null, null)] // Test case for failed deletion (user not found)
        [InlineData(false, true, typeof(AppException), HttpStatusCode.NotFound)] // Test case for exception handling
        public async Task DeleteOneAsync_HandlesVariousScenarios_Correctly(bool expectedResult, bool repoSetupResult, Type? exceptionType, HttpStatusCode? httpStatusCode)
        {
            var productId = Guid.NewGuid();
            // Arrange
            if (exceptionType == null)
            {
                _mockProductRepository.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(repoSetupResult);
            }
            else
            {
                var exceptionInstance = (Exception)Activator.CreateInstance(exceptionType, new object[] { httpStatusCode!, "Simulated database error" })!;
                _mockProductRepository.Setup(r => r.DeleteAsync(productId)).ThrowsAsync(exceptionInstance);
            }

            // Act & Assert
            if (exceptionType == null)
            {
                var result = await _productService.DeleteOneAsync(productId);
                Assert.Equal(expectedResult, result);
            }
            else
            {
                var exception = await Assert.ThrowsAsync(exceptionType, () => _productService.DeleteOneAsync(productId));
                Assert.IsType<AppException>(exception);
                var appException = exception as AppException;
                Assert.Equal(httpStatusCode, appException?.StatusCode);
            }

            _mockProductRepository.Verify(r => r.DeleteAsync(productId), Times.Once);
        }


        [Fact]
        public async Task GetOneByIdAsync_ReturnsProduct_WhenValidIdProvided()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Title = "Test Product" };

            _mockProductRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductReadDto>(product)).Returns(new ProductReadDto { Title = product.Title });

            // Act
            var result = await _productService.GetOneByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Title);
        }

    }
}
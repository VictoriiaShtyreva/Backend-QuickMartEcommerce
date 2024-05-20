using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Interfaces;
using Moq;

namespace Ecommerce.Tests.src.Core
{
    public class ProductTests
    {
        private readonly Mock<IProductImageRepository> _mockProductImageRepository;

        public ProductTests()
        {
            _mockProductImageRepository = new Mock<IProductImageRepository>();
        }

        [Theory]
        [InlineData("Product A", 0.01, "Minimal price")]
        [InlineData("Product Z", 999999.99, "Maximum price")]
        [InlineData("Product &*()", 50.00, "Special characters in title")]
        public async Task CreateSnapshotAsync_HandlesVariousPropertyValues(string title, decimal price, string description)
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product(
                title: title,
                price: price,
                description: description,
                categoryId: Guid.NewGuid(),
                inventory: 50
            )
            {
                Id = productId,
                Images = new List<ProductImage>
                {
                    new ProductImage { Url = "http://example.com/image1.jpg" },
                    new ProductImage { Url = "http://example.com/image2.jpg" }
                }
            };

            _mockProductImageRepository.Setup(repo => repo.GetProductImagesByProductIdAsync(productId))
                .ReturnsAsync(product.Images.ToList());

            // Act
            var snapshot = await product.CreateSnapshotAsync(_mockProductImageRepository.Object);

            // Assert
            Assert.NotNull(snapshot);
            Assert.Equal(title, snapshot.Title);
            Assert.Equal(price, snapshot.Price);
            Assert.Equal(description, snapshot.Description);
            Assert.Equal(2, snapshot.ImageUrls!.Count);
            Assert.Contains("http://example.com/image1.jpg", snapshot.ImageUrls);
            Assert.Contains("http://example.com/image2.jpg", snapshot.ImageUrls);
        }
    }
}
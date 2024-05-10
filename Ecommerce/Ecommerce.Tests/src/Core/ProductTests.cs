using Ecommerce.Core.src.Entities;

namespace Ecommerce.Tests.src.Core
{
    public class ProductTests
    {
        [Theory]
        [InlineData("Product A", 0.01, "Minimal price")]
        [InlineData("Product Z", 999999.99, "Maximum price")]
        [InlineData("Product &*()", 50.00, "Special characters in title")]
        public void CreateSnapshot_HandlesVariousPropertyValues(string title, decimal price, string description)
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
                Id = productId
            };

            // Act
            var snapshot = product.CreateSnapshot();

            // Assert
            Assert.NotNull(snapshot);
            Assert.Equal(title, snapshot.Title);
            Assert.Equal(price, snapshot.Price);
            Assert.Equal(description, snapshot.Description);
        }
    }
}
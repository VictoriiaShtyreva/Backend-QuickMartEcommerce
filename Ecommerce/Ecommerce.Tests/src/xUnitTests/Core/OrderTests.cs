using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Tests.src.Core
{
    public class OrderTests
    {
        public static IEnumerable<object[]> OrderItemData()
        {
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var productSnapshot1 = new ProductSnapshot(productId1, "Test Product 1", 10.00m, "Description 1", new List<string> { "http://example.com/image1.jpg", "http://example.com/image2.jpg" });
            var productSnapshot2 = new ProductSnapshot(productId2, "Test Product 2", 20.00m, "Description 2", new List<string> { "http://example.com/image3.jpg" });

            yield return new object[] { productSnapshot1, 1, 10.00m };
            yield return new object[] { productSnapshot1, 2, 20.00m };
            yield return new object[] { productSnapshot2, 1, 20.00m };
            yield return new object[] { productSnapshot2, 5, 100.00m };
        }

        [Theory]
        [MemberData(nameof(OrderItemData))]
        public void AddOrderItem_CalculatesTotalCorrectly(ProductSnapshot productSnapshot, int quantity, decimal expectedTotal)
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), Guid.NewGuid());

            // Act
            order.AddOrderItem(productSnapshot, quantity);

            // Assert
            Assert.Equal(expectedTotal, order.TotalPrice);
        }
    }
}
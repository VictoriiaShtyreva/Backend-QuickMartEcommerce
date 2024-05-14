using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.OrderAggregate;

namespace Ecommerce.Tests.src.Core
{
    public class OrderTests
    {
        public static IEnumerable<object[]> OrderItemData()
        {
            var productId = Guid.NewGuid();
            var product1 = new Product("Test Product 1", 10.00m, "Description 1", Guid.NewGuid(), 10) { Id = productId };
            var product2 = new Product("Test Product 2", 20.00m, "Description 2", Guid.NewGuid(), 20) { Id = productId };

            yield return new object[] { product1, 1, 10.00m };
            yield return new object[] { product1, 2, 20.00m };
            yield return new object[] { product2, 1, 20.00m };
            yield return new object[] { product2, 5, 100.00m };
        }

        [Theory]
        [MemberData(nameof(OrderItemData))]
        public void AddOrderItem_CalculatesTotalCorrectly(Product product, int quantity, decimal expectedTotal)
        {
            var address = new Address("Test Street", "Test City", 00180, "Test Zip");
            // Arrange
            var order = new Order(Guid.NewGuid(), address);

            // Act
            order.AddOrderItem(product, quantity);

            // Assert
            Assert.Equal(expectedTotal, order.TotalPrice);
        }
    }
}